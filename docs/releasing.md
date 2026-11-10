# Releasing

Releases follow the same model as WebsocketClientLite.PCL: GitVersion sets the version, a change to
`GitVersion.yml` on a `releases/**` branch starts the publish workflow, and the release notes come from the README.

## Steps

1. On a branch `releases/<x.y.z>`, set `next-version: <x.y.z>` in `GitVersion.yml`.
2. Add a `## New in Version <x.y.z>` section to `README.md`. The workflow stops before building anything if it is
   missing. Update `<releaseNotes>` in `src/build/Plugin.SegmentedControl.Netstandard.nuspec` as well.
3. Push the branch. `.github/workflows/build.yml` ("Publish Package") then:
   - works out the version with GitVersion and extracts the README section;
   - builds and tests the shared library on a GitHub-hosted Ubuntu runner;
   - builds the Android and UWP renderers on the self-hosted Windows runner, and the iOS and macOS renderers on
     the self-hosted macOS runner;
   - packs `Plugin.SegmentedControl.Netstandard` from the nuspec. A missing platform fails the pack (NU5128);
   - pushes the package to GitHub Packages and to NuGet.org, then creates a **draft** GitHub release `v<x.y.z>`
     with the package attached.
4. Check the draft release and publish it. Publishing creates the `v<x.y.z>` tag.

The workflow can also be started by hand (Actions → Publish Package → Run workflow) on a release branch.

## One-time setup

### NuGet.org Trusted Publishing

The workflow has no API-key secret. On NuGet.org, signed in as the package owner (`1iveowl`), open
**Trusted Publishing** and add a GitHub Actions policy for repository `1iveowl/Plugin.SegmentedControl` and workflow
file `build.yml`. Without it the NuGet.org push fails.

### Self-hosted runners

GitHub-hosted runners no longer include the Xamarin SDKs, so the renderers are built on two self-hosted runners
registered to this repository (Settings → Actions → Runners):

| Runner | Labels | Needs |
|---|---|---|
| Windows | `self-hosted`, `Windows`, `xamarin` | Visual Studio 2022 with the *Mobile development with .NET* (Xamarin) and *Universal Windows Platform* workloads, Windows SDK 10.0.19041, the .NET 10 SDK, Git |
| macOS | `self-hosted`, `macOS`, `xamarin` | Mono with MSBuild, the Xamarin.iOS and Xamarin.Mac SDKs, a matching Xcode, the .NET 10 SDK, Git |

The renderer projects reference the shared library, so each runner also compiles it. That needs the .NET 10 SDK
and its C# 14 compiler. Check this first on a new runner; it is the part most likely to fail with older MSBuild.

The publish workflow has no `pull_request` trigger, and CI (`ci.yml`) only uses GitHub-hosted runners. Keep it that
way: a self-hosted runner on a public repository must never run code from a pull request.
