# Repository guidance for AI agents

This file is the canonical guidance for every AI coding assistant working in this
repository — Claude Code, Codex, GitHub Copilot, and any other agent. `CLAUDE.md`
points here so the rules do not drift.

## Attribution: an agent is never an author

Everything an agent produces here is authored by the person who asked for it. An
agent must never attribute any part of that work to itself, in git or anywhere the
history is read.

- Never add a `Co-Authored-By:` / `Co-authored-by:` trailer naming an AI agent,
  model, or vendor to a commit message.
- Never add a "generated with", 🤖, or tool-advertisement line to a commit message,
  a pull request title, or a pull request body.
- Never set `user.name`, `user.email`, `GIT_AUTHOR_*`, or `GIT_COMMITTER_*` to an
  agent identity. Commit and push with the repository user's own git identity.
- Never name an agent as an author in release notes, changelog entries, or code
  comments describing who wrote something.
- This applies equally to amended commits, squashes, rebases, cherry-picks, and to
  any commit created by a script or workflow an agent writes.

The requirement is about the record, not about a single command: `git shortlog -sne`
and this repository's contributor list must show people only.

Before pushing, confirm nothing was added:

```bash
# The commits about to be pushed; use -n 20 instead when the branch has no upstream yet.
git log '@{upstream}..HEAD' --format='%an <%ae>'
git log '@{upstream}..HEAD' --format='%B' | grep -Ei 'co-authored-by|generated with|🤖'
```

The first command must show human identities only; the second must print nothing.

If either one flags something, rewrite the offending commit rather than adding a
follow-up commit — a trailer already pushed is part of the history the rule exists
to keep clean.

## Herdr sessions

The dev container installs Herdr and its version-matched control skill. Before
controlling panes, require `HERDR_ENV=1` and read that skill. An agent outside
Herdr must not control the focused session. Use explicit returned pane IDs or
unique agent names, preserve the working directory and avoid stealing focus.

Do not stop the Herdr server, rebuild the container or update agent CLIs while
an agent chain is running. Do not close or send input to panes you did not
create unless the owner explicitly requests it. The human handles startup trust and permission dialogs. Detaching a client keeps
processes alive; restoring conversations after a rebuild does not restore an
interrupted turn or check. Verify task state before continuing.

## Project-specific guidance

- Library: `src/main/SegCtlr.Netstandard` (`netstandard2.0`, Xamarin.Forms 5). Build it with
  `dotnet build src/main/SegCtlr.Netstandard/SegCtlr.Netstandard.csproj`.
- `src/test/Test.SegCtrl.netstandard` is the shared sample app; it builds on Linux too.
- The platform projects under `src/crossplatform` and the platform test apps under `src/test`
  are legacy Xamarin.iOS, Xamarin.Android, Xamarin.Mac and UWP projects. They do not build in
  the dev container; do not try to restore or build `src/main/SegCtlr.Netstandard.sln` there.
- Packaging scripts and the nuspec live in `src/build`.
