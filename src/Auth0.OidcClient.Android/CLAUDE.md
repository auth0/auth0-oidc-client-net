# AI Agent Guidelines for Auth0.OidcClient.Android

> ⚠️ **This package is deprecated.** It relies on Android support libraries deprecated by Google since 2019 and cannot target .NET 6+. For new Android work, use `Auth0.OidcClient.AndroidX`. Only accept bug fixes here; do not add new features.

## Project Structure

```
src/Auth0.OidcClient.Android/
└── Auth0.OidcClient.Android.csproj   # Legacy Android browser integration
```

---

## Boundaries

### ✅ Always Do

- Make surgical changes — touch only what the request requires
- Keep `<Version>` in the `.csproj` in sync with `nuget/Auth0.OidcClient.Android.nuspec`
- Update `README.md` if changing anything visible to consumers

### ⚠️ Ask First

- **Any breaking change — always ask first**
- Any change at all — this package is deprecated; confirm the fix cannot be applied to AndroidX instead

### 🚫 Never Do

- Add new features (deprecated; direct new work to AndroidX)
- Commit secrets, API keys, or tokens
- Log ID tokens, access tokens, or refresh tokens

---

## Security Considerations

- **PKCE** — authentication flows use PKCE via `Auth0ClientBase`; do not bypass
- **Token logging** — never log tokens

---

## Commands

```bash
nuget restore Auth0.OidcClient.All.sln
msbuild Auth0.OidcClient.All.sln -t:rebuild -property:Configuration=Release -property:AndroidSdkDirectory=%ANDROID_SDK_ROOT%
dotnet format --verify-no-changes
```

---

## Docs Update Rules

| File | What it covers | Exists |
|------|---------------|--------|
| `README.md` (repo root) | Installation note (deprecated, prefer AndroidX) | ✅ present |
