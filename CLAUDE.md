# AI Agent Guidelines for auth0-oidc-client-net

## Your Role

You are a C# SDK engineer working on auth0-oidc-client-net. This repo provides Auth0 OIDC authentication for .NET desktop and mobile platforms via per-platform NuGet packages.

---

## Workspace Packages

This is a .NET solution (MSBuild/NuGet) monorepo. **Build and test commands run at the solution root (`Auth0.OidcClient.All.sln`), not per package directory** — MSBuild resolves cross-project references automatically.

| Package | Path | Purpose |
|---------|------|---------|
| Auth0.OidcClient.Core | `src/Auth0.OidcClient.Core` | Shared OIDC client logic, base class, token validation |
| Auth0.OidcClient.AndroidX | `src/Auth0.OidcClient.AndroidX` | Android browser integration (Chrome Custom Tabs) |
| Auth0.OidcClient.Android | `src/Auth0.OidcClient.Android` | Android legacy (deprecated; use AndroidX for new work) |
| Auth0.OidcClient.iOS | `src/Auth0.OidcClient.iOS` | iOS browser integration (ASWebAuthenticationSession) |
| Auth0.OidcClient.WPF | `src/Auth0.OidcClient.WPF` | WPF desktop browser integration |
| Auth0.OidcClient.WinForms | `src/Auth0.OidcClient.WinForms` | WinForms desktop browser integration |
| Auth0.OidcClient.UWP | `src/Auth0.OidcClient.UWP` | UWP Web Authentication Broker integration |
| Auth0.OidcClient.MAUI | `src/Auth0.OidcClient.MAUI` | MAUI cross-platform browser integration |
| Auth0.OidcClient.MAUI.Platforms.Windows | `src/Auth0.OidcClient.MAUI.Platforms.Windows` | MAUI Windows-specific platform implementation |

`Auth0.OidcClient.Core` is the base; all platform packages take a `<ProjectReference>` on it — changes to Core can break all dependents.

`Directory.Build.props` and `global.json` apply to every package; changing either affects the whole repo.

---

## Git Workflow

See [references/git-workflow.md](references/git-workflow.md) for branch naming, commit messages, and PR conventions. Read when creating branches, writing commits, or opening PRs.
