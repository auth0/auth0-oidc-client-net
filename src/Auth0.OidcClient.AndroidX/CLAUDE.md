# AI Agent Guidelines for Auth0.OidcClient.AndroidX

## Project Structure

```
src/Auth0.OidcClient.AndroidX/
└── Auth0.OidcClient.AndroidX.csproj   # Android browser integration via Chrome Custom Tabs (AndroidX)
test/Android/
└── Android.csproj                      # Interactive test app (not automated unit tests)
```

---

## Boundaries

### ✅ Always Do

- Run tests before committing: build the solution then `dotnet test **\bin\**\*UnitTests.dll`
- Make surgical changes — touch only what the request requires
- PascalCase for public types/members; `_camelCase` for private fields
- Update `README.md` in the same PR when changing public API or configuration
- When adding a new outbound Auth0 request path: route through `AppendTelemetry()` in `Auth0ClientBase`
- Keep `<Version>` in the `.csproj` in sync with `nuget/Auth0.OidcClient.AndroidX.nuspec`
- Prefer AndroidX over the legacy `Auth0.OidcClient.Android` package for any Android work

### ⚠️ Ask First

- **Any breaking change — always ask first**
- Adding new NuGet or Android dependencies
- Changing Chrome Custom Tabs redirect/callback handling
- Modifying `Directory.Build.props` or `global.json`
- Raising minimum Android API level

### 🚫 Never Do

- Commit secrets, API keys, or tokens
- Log ID tokens, access tokens, or refresh tokens
- Modify `obj/` or `bin/` output by hand
- Add new features to the deprecated `Auth0.OidcClient.Android` package

---

## Security Considerations

- **PKCE** — authentication flows use PKCE via `Auth0ClientBase`; do not bypass
- **Token logging** — never log tokens in debug output or exceptions
- **Custom URL scheme / app links** — callback URI must match the Auth0 dashboard allowed callback URLs; changes require updating both the Android manifest and Auth0 configuration

---

## Commands

```bash
# Requires Windows + Android SDK (CI sets ANDROID_SDK_ROOT)
nuget restore Auth0.OidcClient.All.sln
msbuild Auth0.OidcClient.All.sln -t:rebuild -property:Configuration=Release -property:AndroidSdkDirectory=%ANDROID_SDK_ROOT%
dotnet test **\bin\**\*UnitTests.dll
dotnet format --verify-no-changes
```

---

## Testing

No automated unit tests for this package. Validate via the `test/Android/` interactive test app on a device or emulator.

---

## Code Style

PascalCase public members, `_camelCase` private fields, async methods end in `Async`. XML doc comments on public members.

---

## Common Pitfalls

- Build requires Windows with the Android SDK installed; `ANDROID_SDK_ROOT` must be set and `platforms;android-31` + `build-tools;31.0.0` installed
- `Auth0.OidcClient.Android` (legacy) cannot target .NET 6+; do not add features there — use AndroidX
- Chrome Custom Tabs requires the intent filter in the host app's `AndroidManifest.xml`

---

## Docs Update Rules

| File | What it covers | Exists |
|------|---------------|--------|
| `README.md` (repo root) | Installation (`Auth0.OidcClient.AndroidX`), Android quickstart link | ✅ present |

Update `README.md` in the same PR when changing the public API or Android-specific configuration.
