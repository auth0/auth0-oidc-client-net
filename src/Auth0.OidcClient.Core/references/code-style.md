# Code Style

## Naming Conventions

| Element | Convention | Example |
|---------|-----------|---------|
| Classes, interfaces, enums | PascalCase | `Auth0ClientBase`, `IAuth0Client` |
| Public methods, properties | PascalCase | `LoginAsync`, `EnableTelemetry` |
| Private fields | `_camelCase` | `_options`, `_userAgent`, `_oidcClient` |
| Parameters, local variables | camelCase | `options`, `extraParameters` |
| Async methods | suffix `Async` | `LoginAsync`, `LogoutAsync` |
| Interfaces | prefix `I` | `IAuth0Client`, `ISignatureVerifier` |

## Good vs Bad

**✅ Good — follows project patterns:**

```csharp
private readonly Auth0ClientOptions _options;

public async Task<LoginResult> LoginAsync(
    object extraParameters = null,
    CancellationToken cancellationToken = default)
{
    var finalExtraParameters = AppendTelemetry(extraParameters);
    // ...
    return await OidcClient.LoginAsync(loginRequest, cancellationToken);
}
```

**❌ Bad — violates conventions:**

```csharp
private Auth0ClientOptions options; // missing underscore prefix

public async Task<LoginResult> Login(  // missing Async suffix
    object ExtraParameters = null)     // PascalCase parameter
{
    // ...
}
```

## Patterns Used

- **Template method** — `Auth0ClientBase` is abstract; platform packages subclass it and inject an `IBrowser` implementation
- **Options object** — `Auth0ClientOptions` collects all configuration; passed to the constructor
- **Async/await throughout** — all public methods are async; use `CancellationToken` as the last parameter
- **XML doc comments** on all public members (`/// <summary>…`)
- Strong-name signing on all assemblies (key in `build/Auth0OidcClientStrongName.snk`)
