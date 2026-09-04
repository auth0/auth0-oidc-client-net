# AI Agent Guidelines for Auth0.OidcClient.MAUI

## Project Structure

```
src/Auth0.OidcClient.MAUI/
├── Auth0.OidcClient.MAUI.csproj   # MAUI multi-platform targets
├── Auth0Client.cs                  # Platform-specific Auth0Client subclass
└── README.md                       # Package-specific install/usage notes
src/Auth0.OidcClient.MAUI.Platforms.Windows/
└── Auth0.OidcClient.MAUI.Platforms.Windows.csproj  # Windows-specific MAUI implementation
```

---

## Boundaries

### ✅ Always Do

- Run tests before committing: `dotnet test **\bin\**\*UnitTests.dll` (build the solution first)
- Make surgical changes — touch only what the request requires
- Follow PascalCase for public types/members; `_camelCase` for private fields
- Update `README.md` (repo root) and the package-level `README.md` in the same PR when changing the public API or configuration
- When adding a **new outbound request path to Auth0**: route it through `AppendTelemetry()` in `Auth0ClientBase` so it carries the `auth0Client` parameter — preserve `Auth0ClientOptions.EnableTelemetry` opt-out
- Keep `<Version>` in `Auth0.OidcClient.MAUI.csproj` in sync with `nuget/Auth0.OidcClient.MAUI.nuspec`
- Maintain existing minimum MAUI platform targets

### ⚠️ Ask First

- **Any breaking change — always ask first**
- Adding new NuGet dependencies
- Changing the `IBrowser` implementation in a way that alters redirect/callback handling
- Raising minimum platform targets
- Modifying `Directory.Build.props` or `global.json` (affects all packages)
- Bumping dependency versions

### 🚫 Never Do

- Commit secrets, API keys, or tokens
- Log ID tokens, access tokens, or refresh tokens
- Remove or skip failing tests
- Modify `obj/` or `bin/` output by hand
- Break backward compatibility without approval

---

## Security Considerations

- **PKCE** — authentication flows use PKCE via `Auth0ClientBase`; do not bypass
- **Token logging** — never log tokens in debug output or exceptions
- **Redirect URI** — MAUI apps use custom URI schemes for callbacks; do not change the scheme without verifying allowed callback URLs in the Auth0 dashboard

---

## Commands

Build the full solution (requires Windows with Android/iOS/MAUI workloads):

```bash
nuget restore Auth0.OidcClient.All.sln
msbuild Auth0.OidcClient.All.sln -t:rebuild -property:Configuration=Release
dotnet test **\bin\**\*UnitTests.dll
dotnet format --verify-no-changes
```

---

## Testing

No dedicated unit test project exists for this package. Test changes via `test/Auth0.OidcClient.Core.UnitTests/` (Core logic) and manually on a MAUI device/emulator.

---

## Code Style

PascalCase public members, `_camelCase` private fields, async methods end in `Async`. XML doc comments on public members. See Core's `references/code-style.md` for full conventions.

---

## Common Pitfalls

- Full solution build requires Windows; MAUI workloads (`dotnet workload install android ios maui`) must be installed
- MAUI Windows platform-specific code lives in `Auth0.OidcClient.MAUI.Platforms.Windows` — check both packages when making Windows-specific changes

---

## Docs Update Rules

| File | What it covers | Exists |
|------|---------------|--------|
| `README.md` (repo root) | Installation, platform quickstart links | ✅ present |
| `src/Auth0.OidcClient.MAUI/README.md` | MAUI-specific install/usage | ✅ present |

Update both READMEs in the same PR when changing the public API or MAUI-specific configuration.
