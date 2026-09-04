# Common Pitfalls

## 1. Full solution build requires Windows

The solution includes Android, iOS, and MAUI projects that require mobile workloads only available on Windows CI (`windows-2022`). Running `msbuild Auth0.OidcClient.All.sln` on macOS/Linux will fail for mobile packages. Build Core only (`dotnet build src/Auth0.OidcClient.Core/…`) if you're on a non-Windows machine.

## 2. `nuget restore` before `msbuild`

Use `nuget restore Auth0.OidcClient.All.sln` (the legacy NuGet CLI), not `dotnet restore`, before calling `msbuild`. The solution uses `packages.config`-style restore for some legacy projects; `dotnet restore` does not handle these.

## 3. Version number in two places

Each package has `<Version>` in its `*.csproj` and a matching `<version>` in its `nuget/*.nuspec`. The release workflow reads from the nuspec; both must stay in sync. Updating one without the other causes a version mismatch on the published NuGet package.

## 4. `Auth0.OidcClient.Android` is deprecated

New Android integrations should use `Auth0.OidcClient.AndroidX`. The legacy `Auth0.OidcClient.Android` package relies on deprecated Android support libraries and cannot target .NET 6+. Don't add features to the Android package — direct them to AndroidX.

## 5. ID token validation is security-critical

`IdTokenValidator` performs issuer, audience, nonce, expiry, and (optionally) signature verification. Weakening or bypassing any check — even in tests — must go through review. Use `NoSignatureVerifier` only in unit tests, never in production code paths.

## 6. `Directory.Build.props` suppresses EOL workload warnings

The `CheckEolWorkloads=false` setting in `Directory.Build.props` intentionally suppresses end-of-life workload warnings in CI. Don't remove it — it prevents CI noise for legacy platform targets that are still supported.
