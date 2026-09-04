# AI Agent Guidelines for Auth0.OidcClient.MAUI.Platforms.Windows

## Project Structure

```
src/Auth0.OidcClient.MAUI.Platforms.Windows/
└── Auth0.OidcClient.MAUI.Platforms.Windows.csproj   # Windows-specific MAUI browser integration
test/Auth0.OidcClient.MAUI.Platforms.Windows.UnitTests/
└── Auth0.OidcClient.MAUI.Platforms.Windows.UnitTests.csproj
```

---

## Boundaries

### ✅ Always Do

- Run tests before committing: build the solution then `dotnet test **\bin\**\*UnitTests.dll`
- Make surgical changes — touch only what the request requires
- PascalCase for public types/members; `_camelCase` for private fields
- Update `README.md` in the same PR when changing the public API
- When adding a new outbound Auth0 request path: route through `AppendTelemetry()` in `Auth0ClientBase`
- Keep `<Version>` in the `.csproj` in sync with the corresponding `.nuspec`

### ⚠️ Ask First

- **Any breaking change — always ask first**
- Adding new NuGet dependencies
- Changing Windows-specific redirect/callback handling
- Modifying `Directory.Build.props` or `global.json`

### 🚫 Never Do

- Commit secrets, API keys, or tokens
- Log ID tokens, access tokens, or refresh tokens
- Modify `obj/` or `bin/` output by hand
- Break backward compatibility without approval

---

## Security Considerations

- **PKCE** — authentication flows use PKCE via `Auth0ClientBase`; do not bypass
- **Token logging** — never log tokens in debug output or exceptions
- **Windows WebAuthenticationBroker** — redirect URI registration must match the Auth0 dashboard allowed callback URLs

---

## Commands

```bash
nuget restore Auth0.OidcClient.All.sln
msbuild Auth0.OidcClient.All.sln -t:rebuild -property:Configuration=Release
dotnet test **\bin\**\*UnitTests.dll
dotnet format --verify-no-changes
```

---

## Testing

Unit tests live in `test/Auth0.OidcClient.MAUI.Platforms.Windows.UnitTests/`.

---

## Code Style

PascalCase public members, `_camelCase` private fields, async methods end in `Async`. XML doc comments on public members.

---

## Common Pitfalls

- Build requires Windows; MAUI workloads must be installed (`dotnet workload install android ios maui`)
- Coordinate changes with `Auth0.OidcClient.MAUI` — they share the MAUI user experience

---

## Docs Update Rules

| File | What it covers | Exists |
|------|---------------|--------|
| `README.md` (repo root) | Installation, MAUI platform docs | ✅ present |

Update `README.md` in the same PR when changing public API or Windows-specific behavior.
