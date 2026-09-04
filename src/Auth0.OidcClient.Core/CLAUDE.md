# AI Agent Guidelines for Auth0.OidcClient.Core

## Project Structure

```
src/Auth0.OidcClient.Core/
├── Auth0ClientBase.cs          # Abstract base class; LoginAsync, LogoutAsync, GetUserInfoAsync, RefreshTokenAsync
├── Auth0ClientOptions.cs       # Configuration options (Domain, ClientId, Scope, EnableTelemetry, …)
├── IAuth0Client.cs             # Public interface
├── AssemblyInfo.cs             # Strong-name key reference
├── Tokens/
│   ├── IdTokenValidator.cs     # Validates ID token against IdTokenRequirements
│   ├── IdTokenRequirements.cs  # Expected issuer, audience, nonce, org, max_age
│   ├── IdTokenValidationException.cs
│   ├── AsymmetricSignatureVerifier.cs
│   ├── ISignatureVerifier.cs
│   └── Auth0ClaimNames.cs      # Auth0-specific JWT claim constants
test/Auth0.OidcClient.Core.UnitTests/
├── Tokens/
│   ├── IdTokenValidatorTests.cs  # xUnit tests for token validation
│   ├── JwtTokenFactory.cs        # Test helper for building JWTs
│   └── NoSignatureVerifier.cs    # Test stub
└── Data/
    └── mockJsonWebKeySet.json    # Fixture for JWKS tests
```

---

## Boundaries

### ✅ Always Do

- Run tests before committing: `dotnet test **\bin\**\*UnitTests.dll` (Windows) or build first then run
- Make surgical changes — touch only what the request requires; do not refactor or reformat adjacent code
- Follow PascalCase for public types and members; camelCase with underscore prefix for private fields (`_options`, `_userAgent`)
- Add xUnit unit tests for new Core functionality in `test/Auth0.OidcClient.Core.UnitTests/`
- Use typed exceptions following the project's error hierarchy (`IdTokenValidationException` for token errors)
- Update `README.md` in the same PR when changing the public API, configuration options, or supported platforms
- When adding a **new outbound request path to Auth0** (login, logout, userinfo, refresh, etc.): route it through the existing `AppendTelemetry()` method in `Auth0ClientBase.cs` so it carries the `auth0Client` parameter — preserve the `Auth0ClientOptions.EnableTelemetry` opt-out
- Keep the version number in `Auth0.OidcClient.Core.csproj` (`<Version>`) in sync with the corresponding `nuget/Auth0.OidcClient.Core.nuspec`
- Maintain the existing minimum .NET framework support (`netstandard2.0` for Core)

### ⚠️ Ask First

- **Any breaking change — always ask first.** Never remove or rename public API members, change method signatures, or alter token validation behavior without explicit approval
- Adding new NuGet dependencies
- Raising the minimum `<TargetFrameworks>` or `netstandard` target
- Modifying ID token validation logic in `Tokens/` (security-critical)
- Changes to the telemetry payload structure in `Auth0ClientBase.cs`
- Changes to `Directory.Build.props` or `global.json` (affects all packages)
- Bumping dependency versions in `*.csproj` (especially `Duende.IdentityModel.OidcClient`, `Microsoft.IdentityModel.*`)

### 🚫 Never Do

- Commit secrets, API keys, tokens, or the strong-name `.snk` key (already gitignored)
- Log, print, or write ID tokens, access tokens, or refresh tokens to any output stream
- Remove or skip failing tests without fixing the underlying issue
- Modify `obj/`, `bin/` build output by hand
- Break backward compatibility without explicit approval (see Ask First)
- Hand-edit `nuget/*.nuspec` version numbers without also updating the corresponding `*.csproj`

---

## Security Considerations

- **PKCE** — all authentication flows use PKCE via `Duende.IdentityModel.OidcClient`; do not bypass or disable it
- **ID token validation** — `IdTokenValidator` validates issuer, audience, expiry, nonce, and signature on every login; do not skip or weaken these checks
- **Token logging** — never log or expose access tokens, refresh tokens, or ID tokens in debug output, exceptions, or responses
- **No token storage** — Core does not persist tokens; it returns them to the caller. Do not add persistent storage in Core
- **Strong-name signing** — assemblies are strong-named via `build/Auth0OidcClientStrongName.snk`; do not disable signing on release builds
- **Secret scanning** — do not commit `.env` files, credentials, or the `.snk` key file

---

## Commands

See [references/commands.md](references/commands.md) for the full command reference. Read when you need to build, test, or format.

Core commands (run from repo root on Windows — CI uses `windows-2022`):

```bash
# Restore NuGet packages
nuget restore Auth0.OidcClient.All.sln

# Build full solution
msbuild Auth0.OidcClient.All.sln -t:rebuild -property:Configuration=Release

# Run unit tests (after build)
dotnet test **\bin\**\*UnitTests.dll

# Format check
dotnet format --verify-no-changes
```

---

## Testing

See [references/testing.md](references/testing.md) for testing conventions and mock patterns. Read when writing or reviewing tests.

- **Framework:** xUnit 2.x + Moq 4.x
- **Test location:** `test/Auth0.OidcClient.Core.UnitTests/`
- The default `dotnet test **\bin\**\*UnitTests.dll` suite is unit-only — no credentials required

---

## Code Style

PascalCase for public types, methods, and properties. Private fields use `_camelCase`. See [references/code-style.md](references/code-style.md) for naming conventions and patterns. Read when adding new types or reviewing style.

CI-enforced: `dotnet build /warnaserror` treats warnings as errors — fix all warnings before committing.

---

## Common Pitfalls

See [references/pitfalls.md](references/pitfalls.md) for platform and build gotchas. Read before making environment or dependency changes.

---

## Docs Update Rules

See [references/docs-update.md](references/docs-update.md) for the tracked-docs inventory and code-to-docs mapping. Read before marking a PR complete.

Update `README.md` in the same PR when changing the public API, configuration options, or installation instructions — do not defer.
