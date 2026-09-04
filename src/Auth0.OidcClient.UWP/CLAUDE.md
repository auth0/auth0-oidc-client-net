# AI Agent Guidelines for Auth0.OidcClient.UWP

## Project Structure

```
src/Auth0.OidcClient.UWP/
└── Auth0.OidcClient.UWP.csproj   # UWP Web Authentication Broker integration
test/UWP/
├── UWP.csproj
└── project.json                   # Interactive test app (not automated unit tests)
```

---

## Boundaries

### ✅ Always Do

- Run tests before committing: build the solution then `dotnet test **\bin\**\*UnitTests.dll`
- Make surgical changes — touch only what the request requires
- PascalCase for public types/members; `_camelCase` for private fields
- Update `README.md` in the same PR when changing public API or configuration
- When adding a new outbound Auth0 request path: route through `AppendTelemetry()` in `Auth0ClientBase`
- Keep `<Version>` in the `.csproj` in sync with `nuget/Auth0.OidcClient.UWP.nuspec`

### ⚠️ Ask First

- **Any breaking change — always ask first**
- Adding new NuGet or Windows UWP dependencies
- Changing Web Authentication Broker redirect/callback behavior
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
- **Web Authentication Broker** — UWP uses `WebAuthenticationBroker`; callback URI registration must match Auth0 allowed callback URLs

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

No automated unit tests for this package. Validate via the `test/UWP/` interactive test app.

---

## Code Style

PascalCase public members, `_camelCase` private fields, async methods end in `Async`. XML doc comments on public members.

---

## Common Pitfalls

- UWP builds require Windows and UWP workload support; `project.json` in `test/UWP/` is a legacy format
- UWP's `WebAuthenticationBroker` has specific URI scheme requirements — test on a real device before release

---

## Docs Update Rules

| File | What it covers | Exists |
|------|---------------|--------|
| `README.md` (repo root) | Installation (`Auth0.OidcClient.UWP`), UWP quickstart link | ✅ present |

Update `README.md` in the same PR when changing public API or UWP-specific configuration.
