## Refresh Tokens

You can request a refresh token by requesting the `offline_access` scope. The login result will contain the refresh token in the `RefreshToken` property:

```csharp
var client = new Auth0Client(new Auth0ClientOptions
{
    Domain = "YOUR_AUTH0_DOMAIN",
    ClientId = "YOUR_AUTH0_CLIENT_ID",
    Scope = "openid offline_access"
});
var loginResult = await client.LoginAsync();

if (!loginResult.IsError)
{
    Debug.WriteLine($"refresh_token: {loginResult.RefreshToken}");
}
```

### Requesting a Refresh Token

You can request a Refresh Token by calling @Auth0.OidcClient.Auth0Client.RefreshTokenAsync(System.String), passing along the refresh token which was previously returned in the login result as the 

```csharp
var client = new Auth0Client(new Auth0ClientOptions
{
    Domain = "YOUR_AUTH0_DOMAIN",
    ClientId = "YOUR_AUTH0_CLIENT_ID",
    Scope = "openid offline_access"
});
var loginResult = await client.LoginAsync();

if (!loginResult.IsError)
{
    string refreshToken = loginResult.RefreshToken;

    // Save the refresh token somewhere...
}


// Later on elsewhere in your code you can retrieve the refresh token from where you have saved it, and call RefreshTokenAsync
var refreshTokenResult = await client.RefreshTokenAsync(refreshToken);
```

> [!Note]
> Please note that of your use Auth0 API Authorization, your API should allow offline access. This is configured via the **Allow Offline Access** switch on the [API Settings](https://manage.auth0.com/#/apis)
