# Auth0.OidcClient.MAUI

Integrate Auth0 in a MAUI application targetting iOS, macOS or Android by using the `Auth0.OIdcClient.MAUI` SDK.

## Install the SDK

The SDK can be installed through Nuget:

```sh
Install-Package Auth0.OIdcClient.MAUI
```

## Configuring the SDK

Once the SDK has been installed, you can integrate it by instantiating the Auth0Client:

```cs
var client = new Auth0Client(new Auth0ClientOptions()
{
  Domain = "YOUR_AUTH0_DOMAIN",
  ClientId = "YOUR_AUTH0_CLIENT_ID",
  RedirectUri = "myapp://callback",
  PostLogoutRedirectUri = "myapp://callback"
});
```

## Add login to your application

In order to add login, you can call `LoginAsync` on the Auth0Client instance.

```cs
var loginResult = await client.LoginAsync();
```

The returned LoginResult indicates whether or not the request was succesful through the `IsError` property. Incase it was succesful, you can retrieve the user using the `LoginResult.User` property.

```cs
if (loginResult.IsError == false)
{
    var user = loginResult.User
}
```

## Add logout to your application

Logging out of the SDK comes down to calling LogoutAsync on the `Auth0Client` instance.

```cs
await client.LogoutAsync();
```
