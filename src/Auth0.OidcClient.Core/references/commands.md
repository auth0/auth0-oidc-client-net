# Commands

All commands run from the **repo root** on Windows (CI uses `windows-2022`). Mobile workloads (Android, iOS, MAUI) are not available on macOS/Linux via the full solution build.

## Prerequisites

```bash
# Install .NET SDK (version from global.json)
# Then install mobile workloads (required for Android/iOS/MAUI packages)
dotnet workload install android ios maui
dotnet workload update
```

## Build

```bash
# Restore NuGet packages (MSBuild/NuGet, not dotnet restore)
nuget restore Auth0.OidcClient.All.sln

# Build full solution (Release)
msbuild Auth0.OidcClient.All.sln -t:rebuild -property:Configuration=Release

# Build with verbose output (for CI debugging)
msbuild Auth0.OidcClient.All.sln -t:rebuild -verbosity:diag -property:Configuration=Release

# Build Core only (no mobile workloads needed)
dotnet build src/Auth0.OidcClient.Core/Auth0.OidcClient.Core.csproj -c Release
```

## Test

```bash
# Run unit tests (after a full solution build)
dotnet test **\bin\**\*UnitTests.dll

# Run Core unit tests directly (after dotnet build on Core)
dotnet test test/Auth0.OidcClient.Core.UnitTests/Auth0.OidcClient.Core.UnitTests.csproj
```

## Format / Lint

```bash
# Check formatting (CI-equivalent)
dotnet format --verify-no-changes

# Auto-fix formatting
dotnet format

# Build with warnings-as-errors (lint equivalent)
dotnet build /warnaserror src/Auth0.OidcClient.Core/Auth0.OidcClient.Core.csproj
```

## Clean

```bash
dotnet clean Auth0.OidcClient.All.sln
# Or manually remove bin/ and obj/ from each project directory
```
