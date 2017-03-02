# Authentication

> Before you start, ensure that you have set the correct Callback URL. Go to the Clients Settings section in the [Auth0 dashboard](https://manage.auth0.com/#/) and make sure that **Allowed Callback URLs** contains the mobile application callback URL, i.e. `https://YOUR_AUTH0_DOMAIN/mobile`

To authenticate a user in your application, your need to create a new instance of @Auth0.OidcClient.Auth0Client, passing your Auth0 **Domain** and **Client ID** for your Client. Please see the [Clients Documentation](https://auth0.com/docs/clients) on the Auth0 website for more information.

Once you have instantiated an instance of @Auth0.OidcClient.Auth0Client, you can authenticate a user by calling @Auth0.OidcClient.Auth0Client.LoginAsync(System.String,System.Object).

```csharp
using Auth0.OidcClient;

var client = new Auth0Client("YOUR_AUTH0_DOMAIN", "YOUR_AUTH0_CLIENT_ID");

var loginResult = await client.LoginAsync();
```

## The Login Result

The returned login result will indicate whether authentication was successful, and if so contain the tokens and claims of the user.

### Authentication Error

You can check the `IsError` property of the result to see whether the login has failed. The `ErrorMessage` will contain more information regarding the error which occurred.

```csharp
var loginResult = await client.LoginAsync();

if (loginResult.IsError)
{
    Debug.WriteLine($"An error occurred during login: {loginResult.Error}")
}
```

### Accessing the tokens

On successful login, the login result will contain the `id_token` and `access_token` in the `IdentityToken` and `AccessToken` properties respectively.

```csharp
var loginResult = await client.LoginAsync();

if (!loginResult.IsError)
{
    Debug.WriteLine($"id_token: {loginResult.IdentityToken}");
    Debug.WriteLine($"access_token: {loginResult.AccessToken}");
}
```

### Obtaining the User Information

On successful login, the login result will contain the user information in the `User` property, which is a [ClaimsPrincipal](https://msdn.microsoft.com/en-us/library/system.security.claims.claimsprincipal(v=vs.110).aspx).

To obtain information about the user, you can query the claims. You can for example obtain the user's name and email address from the `name` and `email` claims:

```csharp
if (!loginResult.IsError)
{
    Debug.WriteLine($"name: {loginResult.User.FindFirst(c => c.Type == "name")?.Value}");
    Debug.WriteLine($"email: {loginResult.User.FindFirst(c => c.Type == "email")?.Value}");
}
```

> [!Note]
> The exact claims returned will depend on the scopes that were requested. For more information see @scopes.

You can obtain a list of all the claims contained in the `id_token` by iterating through the `Claims` collection:

```csharp
if (!loginResult.IsError)
{
    foreach (var claim in loginResult.User.Claims)
    {
        Debug.WriteLine($"{claim.Type} = {claim.Value}");
    }
}
```

