# Testing

## Framework

- **xUnit** 2.x — test runner and assertions
- **Moq** 4.x — mocking framework
- Tests live in `test/Auth0.OidcClient.Core.UnitTests/`

## Running Tests

```bash
# Run unit tests (after building the solution)
dotnet test **\bin\**\*UnitTests.dll

# Run Core tests directly (no full solution build required)
dotnet test test/Auth0.OidcClient.Core.UnitTests/Auth0.OidcClient.Core.UnitTests.csproj
```

The default test suite is unit-only — no credentials or live Auth0 tenant required.

## Test Conventions

- Test class names end in `Tests` (e.g., `IdTokenValidatorTests`)
- Test method names describe the scenario: `[MethodName]_Should[Behavior]_When[Condition]` or plain descriptive names
- Use `[Theory]` + `[InlineData]` for parameterised cases, `[Fact]` for single assertions
- Arrange/Act/Assert structure — one assertion per logical concern

## Mocking & Test Utilities

- `JwtTokenFactory` — builds signed/unsigned JWTs for test scenarios
- `NoSignatureVerifier` — stub `ISignatureVerifier` that skips cryptographic verification
- `Data/mockJsonWebKeySet.json` — fixture JWKS for JWKS-endpoint tests; loaded via `CopyToOutputDirectory: PreserveNewest`
- Use `Mock<T>` from Moq for interface dependencies; `mock.Setup(…).Returns(…)` pattern is standard in this codebase

## Coverage

No coverage threshold is enforced in CI. Focus coverage on `Tokens/` (the security-critical path) and `Auth0ClientBase` login/logout flows.
