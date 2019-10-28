# API Authorization

Auth0's API authorization features allow you to manage the authorization requirements for server-to-server and client-to-server applications.

For more information you can refer to the [Auth0 API Authorization documentation](https://auth0.com/docs/api-auth)

Using the Auth0 OIDC Client, you can request an `access_token` for an API by passing the `audience` in the `extraParameters` parameter when calling @Auth0.OidcClient.Auth0Client.LoginAsync(System.Object)

```csharp
var client = new Auth0Client(new Auth0ClientOptions {
    Domain = "YOUR_AUTH0_DOMAIN",
    ClientId = "YOUR_AUTH0_CLIENT_ID"
});

var loginResult = await client.LoginAsync(new { audience = "https://your-api-identifier" });
```
