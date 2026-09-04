# AI Agent Guidelines for Auth0.OidcClient.iOS

## Project Structure

```
src/Auth0.OidcClient.iOS/
└── Auth0.OidcClient.iOS.csproj   # iOS browser integration (ASWebAuthenticationSession)
test/iOS/
└── iOS.csproj                     # Interactive test app (not automated unit tests)
```

---

## Boundaries

### ✅ Always Do

- Run tests before committing: build the solution then `dotnet test **\bin\**\*UnitTests.dll`
- Make surgical changes — touch only what the request requires
- PascalCase for public types/members; `_camelCase` for private fields
- Update `README.md` in the same PR when changing public API or configuration
- When adding a new outbound Auth0 request path: route through `AppendTelemetry()` in `Auth0ClientBase`
- Keep `<Version>` in the `.csproj` in sync with `nuget/Auth0.OidcClient.iOS.nuspec`

### ⚠️ Ask First

- **Any breaking change — always ask first**
- Adding new NuGet or iOS framework dependencies
- Changing `ASWebAuthenticationSession` / `SFSafariViewController` redirect handling
- Modifying `Directory.Build.props` or `global.json`
- Raising minimum iOS deployment target

### 🚫 Never Do

- Commit secrets, API keys, or tokens
- Log ID tokens, access tokens, or refresh tokens
- Modify `obj/` or `bin/` output by hand
- Break backward compatibility without approval

---

## Security Considerations

- **PKCE** — authentication flows use PKCE via `Auth0ClientBase`; do not bypass
- **Token logging** — never log tokens in debug output or exceptions
- **Custom URL scheme** — callback URI must match Auth0 dashboard allowed callback URLs; changes require updating both the iOS `Info.plist` and Auth0 configuration

---

## Commands

```bash
# Requires Windows with iOS workload
nuget restore Auth0.OidcClient.All.sln
msbuild Auth0.OidcClient.All.sln -t:rebuild -property:Configuration=Release
dotnet test **\bin\**\*UnitTests.dll
dotnet format --verify-no-changes
```

---

## Testing

No automated unit tests for this package. Validate via the `test/iOS/` interactive test app on simulator or device.

---

## Code Style

PascalCase public members, `_camelCase` private fields, async methods end in `Async`. XML doc comments on public members.

---

## Common Pitfalls

- iOS build requires the iOS workload (`dotnet workload install ios`); full solution build requires Windows CI
- `ASWebAuthenticationSession` requires the custom URL scheme registered in `Info.plist` of the host app
- Coordinating `ephemeralWebBrowserSession` behavior changes with the iOS Xamarin quickstart docs

---

## Docs Update Rules

| File | What it covers | Exists |
|------|---------------|--------|
| `README.md` (repo root) | Installation (`Auth0.OidcClient.iOS`), iOS/Xamarin quickstart link | ✅ present |

Update `README.md` in the same PR when changing public API or iOS-specific configuration.
