## Refresh Tokens

You can request a refresh token by requesting the `offline_access` scope. The login result will contain the refresh token in the `RefreshToken` property:

```csharp
var loginResult = await client.LoginAsync("openid offline_access");

if (!loginResult.IsError)
{
    Debug.WriteLine($"refresh_token: {loginResult.RefreshToken}");
}
```

### Refresh Token handler

