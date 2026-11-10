#!/usr/bin/env bash
set -euo pipefail

# The solution (src/main/SegCtlr.Netstandard.sln) also holds the legacy Xamarin.iOS, Xamarin.Android, Xamarin.Mac
# and UWP projects, which cannot restore on Linux. Restore the netstandard test app instead: it references the
# library, so both netstandard projects restore.
solution_path="src/test/Test.SegCtrl.netstandard/Test.SegCtrl.netstandard.csproj"
repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

# Git on a bind-mounted checkout. See references/pattern.md, "Git status is slow on a bind-mounted checkout".
# The one .git/index is read by host git and by container git. Git caches dev/ino/uid/gid per entry, those differ
# between the two views, so every entry looks dirty and git re-reads and hashes every tracked file on each status.
# checkStat=minimal compares only mtime, size and mode, which is stable across both views. Global on purpose: it is
# a property of the mount, not of one repository, so worktrees and any nested clone inherit it.
git config --global core.checkStat minimal
git config --global core.trustctime false

# The untracked-file walk is the other half of a slow status. Enable the cache only where git confirms that this
# filesystem reports directory mtime reliably; a false positive here would make git miss new files.
if git -C "${repo_root}" update-index --test-untracked-cache >/dev/null 2>&1; then
	git -C "${repo_root}" config core.untrackedCache true
else
	echo "Untracked cache unsupported in ${repo_root}; leaving it off." >&2
fi

# Docker creates named volumes as root. Make persistent tool state available to
# the non-root account required for Claude's bypass-permissions mode.
sudo chown -R "$(id -u):$(id -g)" "${CLAUDE_CONFIG_DIR}" "${CODEX_HOME}" "${COPILOT_HOME}" "${HOME}/.config/herdr" "${HOME}/.config/gh" "${HOME}/.azure" "${HOME}/.tokeneconomizer" "${HOME}/.herdr-core-state"

# herdr-core, which the Herdr desktop app copies into the container, keeps its Spaces, terminal records and identity in
# ~/.local/state/herdr-core. Link that to its volume, so after a rebuild the app finds the same machine and Spaces.
# The volume is mounted outside ~/.local because the image has no ~/.local and Docker would create it as root.
link_herdr_core_state() {
	local state="${HOME}/.local/state/herdr-core" volume="${HOME}/.herdr-core-state"
	mkdir -p "${HOME}/.local/state" || return 1
	if [[ -d "${state}" && ! -L "${state}" ]]; then
		# A core reached this container before post-create ran: set its files aside instead of mixing two cores.
		mv "${state}" "${state}.before-volume.$(date +%s)" || return 1
	fi
	ln -sfn "${volume}" "${state}"
}
if ! link_herdr_core_state; then
	echo "Could not link herdr-core's state to its volume; the Herdr app will not recognise this container after a rebuild." >&2
fi

skills_installer="/opt/ai-skill/scripts/install-macos.sh"
if [[ -f "${skills_installer}" ]]; then
	bash "${skills_installer}" --no-prompt
else
	echo "Shared skills installer is unavailable at ${skills_installer}; skipping skill installation." >&2
fi

npm install -g --allow-scripts=@anthropic-ai/claude-code @anthropic-ai/claude-code @openai/codex

claude_settings="${CLAUDE_CONFIG_DIR}/settings.json"
mkdir -p "${CLAUDE_CONFIG_DIR}" "${CODEX_HOME}"

# The JavaScript template literal is intentionally protected from shell expansion.
# shellcheck disable=SC2016
node -e '
const fs = require("fs");
const path = process.argv[1];
let settings = {};
try { settings = JSON.parse(fs.readFileSync(path, "utf8")); } catch (error) {
  if (error.code !== "ENOENT") throw error;
}
settings.defaultMode = "bypassPermissions";
fs.writeFileSync(path, `${JSON.stringify(settings, null, 2)}\n`);
' "${claude_settings}"

codex_config="${CODEX_HOME}/config.toml"
touch "${codex_config}"
sed -i -E '/^(approval_policy|sandbox_mode)[[:space:]]*=/d' "${codex_config}"
# The two keys are top-level, so they go first: appended after a table such as [projects."..."] or the [features]
# the herdr integration adds, TOML would read them as keys of that table. Leading blank lines are dropped.
{
	printf 'approval_policy = "never"\nsandbox_mode = "danger-full-access"\n'
	sed '/./,$!d' "${codex_config}"
} > "${codex_config}.tmp"
mv "${codex_config}.tmp" "${codex_config}"

# herdr, a terminal multiplexer for running several coding agent sessions side by side; it is a convenience for
# the developer, not something the build or the app needs, so any failure here logs and the setup continues.
install_herdr() {
	# The installer verifies the binary's checksum and puts it in ~/.local/bin, which ~/.profile adds to the PATH
	# only when the directory existed as the login shell started, and a fresh container has none. The link puts it
	# on the PATH of the first session, and "herdr update" still replaces the binary it points to.
	if ! curl -fsSL https://herdr.dev/install.sh | sh; then
		echo "Could not install herdr; continuing without it." >&2
		return
	fi
	sudo ln -sf "${HOME}/.local/bin/herdr" /usr/local/bin/herdr || return 1

	# The agent integrations' hooks report a session to herdr with python3, and the image has only python3-minimal,
	# which lacks the json module; without the full standard library they exit early and a restart cannot resume.
	if ! { sudo apt-get update -qq \
		&& sudo DEBIAN_FRONTEND=noninteractive apt-get install -y -qq --no-install-recommends python3; }; then
		echo "Could not install python3; herdr will not resume agent sessions after a restart." >&2
	fi

	# The integrations let herdr resume a Claude Code or Codex conversation in its pane after the server restarts,
	# a rebuild included. They write a hook script and its entry into the persisted homes and are idempotent.
	local agent
	for agent in claude codex; do
		if ! herdr integration install "${agent}"; then
			echo "Could not install the herdr ${agent} integration; continuing." >&2
		fi
	done

	# The skill teaches an agent running in a herdr pane to drive herdr. The binary prints the copy matching its own
	# version, so it is rewritten on every create instead of being kept with the shared skills in /opt/ai-skill.
	local skill_root
	for skill_root in "${CLAUDE_CONFIG_DIR}/skills" "${CODEX_HOME}/skills" "${COPILOT_HOME}/skills"; do
		mkdir -p "${skill_root}/herdr" || return 1
		herdr --skill > "${skill_root}/herdr/SKILL.md.tmp" || return 1
		mv "${skill_root}/herdr/SKILL.md.tmp" "${skill_root}/herdr/SKILL.md" || return 1
	done

	local completions="${HOME}/.local/share/bash-completion/completions"
	mkdir -p "${completions}" || return 1
	herdr completion bash > "${completions}/herdr"
}
if ! install_herdr; then
	echo "The herdr setup did not finish; continuing." >&2
fi

# TokenEconomizer (https://github.com/microsoft/TokenEconomizer) lowers what Claude Code and Codex sessions cost in
# this repository: it remembers repository facts between sessions, leaves out tools that go unused, and redirects a
# turn that is stuck in a failure loop. Like herdr it is a convenience, so any failure logs and the setup continues.
# Its hooks run on every agent session in a container without permission prompts, so the clone is pinned to a
# reviewed commit; to update, review the upstream changes and move the pin.
tokeneconomizer_commit="264e7a7f32f0112a06aa83a97b9c2c4681563968"
install_tokeneconomizer() {
	# The installed plugins run from the clone, so it must stay where it is. It lives outside the persisted homes and
	# is cloned again on each create; what the plugin learns is kept in ~/.tokeneconomizer, a volume of its own.
	local clone="${HOME}/.local/share/TokenEconomizer"
	if [[ ! -d "${clone}/.git" ]]; then
		mkdir -p "$(dirname "${clone}")" || return 1
		git clone -q https://github.com/microsoft/TokenEconomizer.git "${clone}" || return 1
	fi
	git -C "${clone}" fetch -q origin "${tokeneconomizer_commit}" || return 1
	git -C "${clone}" -c advice.detachedHead=false checkout -q "${tokeneconomizer_commit}" || return 1

	# The hooks need Python 3.10+ with its full standard library. The herdr setup installs it, but may have stopped
	# before it got that far.
	if ! python3 -c 'import json' >/dev/null 2>&1; then
		sudo apt-get update -qq \
			&& sudo DEBIAN_FRONTEND=noninteractive apt-get install -y -qq --no-install-recommends python3 || return 1
	fi

	# Both installs are idempotent: on a rebuild the registrations in the persisted homes already exist.
	claude plugin marketplace add "${clone}" || return 1
	claude plugin install tokeneconomizer@tokeneconomizer || return 1
	codex plugin marketplace add "${clone}" || return 1
	codex plugin add tokeneconomizer@tokeneconomizer || return 1
}
if ! install_tokeneconomizer; then
	echo "The TokenEconomizer setup did not finish; continuing without it." >&2
fi

# Shortcuts that start an agent without permission prompts in a terminal, as the VS Code extension already does:
# yolo for Claude Code, yolo-codex for Codex. Only interactive shells read these files, so nothing started without a
# shell (scripts, Herdr's agent integrations) picks them up. Written once; running the script again changes nothing.
ensure_shell_aliases() {
	local rc line
	local -a lines=(
		"alias yolo='claude --dangerously-skip-permissions'"
		"alias yolo-codex='codex --dangerously-bypass-approvals-and-sandbox'"
	)
	for rc in "${HOME}/.bashrc" "${HOME}/.zshrc"; do
		if [[ "${rc}" == *.zshrc && ! -f "${rc}" ]]; then
			continue
		fi
		for line in "${lines[@]}"; do
			if ! grep -qxF "${line}" "${rc}" 2>/dev/null; then
				printf '\n%s\n' "${line}" >>"${rc}"
			fi
		done
	done
}
ensure_shell_aliases

# Restore last so a feed or project failure cannot abort assistant configuration.
# A failure here still fails postCreateCommand, but the container stays usable.
if [[ -f "${solution_path}" ]]; then
	dotnet restore "${solution_path}"
else
	echo "${solution_path} has not been created yet; skipping restore."
fi
