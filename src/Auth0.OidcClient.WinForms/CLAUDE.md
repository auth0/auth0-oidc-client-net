# AI Agent Guidelines for Auth0.OidcClient.WinForms

## Project Structure

```
src/Auth0.OidcClient.WinForms/
└── Auth0.OidcClient.WinForms.csproj   # WinForms desktop browser integration
test/WinForms/
└── WinForms.csproj                     # Interactive test app (not automated unit tests)
```

---

## Boundaries

### ✅ Always Do

- Run tests before committing: build the solution then `dotnet test **\bin\**\*UnitTests.dll`
- Make surgical changes — touch only what the request requires
- PascalCase for public types/members; `_camelCase` for private fields
- Update `README.md` in the same PR when changing public API or configuration
- When adding a new outbound Auth0 request path: route through `AppendTelemetry()` in `Auth0ClientBase`
- Keep `<Version>` in the `.csproj` in sync with `nuget/Auth0.OidcClient.WinForms.nuspec`

### ⚠️ Ask First

- **Any breaking change — always ask first**
- Adding new NuGet or Windows dependencies
- Changing browser redirect/callback handling
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
- **Redirect URI** — WinForms uses a local loopback or custom URI scheme; changes must be reflected in Auth0 allowed callback URLs

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

No automated unit tests for this package. Validate via the `test/WinForms/` interactive test app.

---

## Code Style

PascalCase public members, `_camelCase` private fields, async methods end in `Async`. XML doc comments on public members.

---

## Common Pitfalls

- WinForms targets Windows only; the `test/WinForms/` app requires a Windows environment
- WPF and WinForms share the same quickstart docs — coordinate changes that affect both

---

## Docs Update Rules

| File | What it covers | Exists |
|------|---------------|--------|
| `README.md` (repo root) | Installation (`Auth0.OidcClient.WinForms`), WPF/WinForms quickstart link | ✅ present |

Update `README.md` in the same PR when changing public API or WinForms-specific configuration.
