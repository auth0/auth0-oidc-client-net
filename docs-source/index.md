# Auth0 OIDC Client

The Auth0 OIDC Client allows you to add authentication for your .NET Client applications. The Auth0 OIDC Client supports the following platforms:

* Universal Windows Platform
* Windows Presentation Foundation (.NET 4.52 and higher)
* Windows Forms (.NET 4.52 and higher)
* iOS (uses Xamarin)
* Android (uses Xamarin)

## Quick Start

> Before you start, ensure that you have configured the Callback URL in the Clients Settings section in the [Auth0 dashboard](https://manage.auth0.com/#/). See the [Callback URLs documentation](documentation/getting-started/callbacks.md) for more information on what Callback URL you need to use for your particular platform.

> [!Note]
> You will also need to ensure that the JWT Signature Algorithm for your client is set to RS256

### 1. Install the NuGet Package for your platform

**Universal Windows Platform (UWP)**

```text
Install-Package Auth0.OidcClient.UWP
```

**Windows Presentation Foundation (WPF)**

```text
Install-Package Auth0.OidcClient.WPF
```

**Windows Forms**

```text
Install-Package Auth0.OidcClient.WinForms
```

**iOS (uses Xamarin)**

```text
Install-Package Auth0.OidcClient.iOS
```

**Android (uses Xamarin)**

```text
Install-Package Auth0.OidcClient.Android
```


### 2. Initialize 

```csharp
using Auth0.OidcClient;

var client = new Auth0Client(new Auth0ClientOptions
{
    Domain = "YOUR_AUTH0_DOMAIN",
    ClientId = "YOUR_AUTH0_CLIENT_ID"
});
```

### 3. Authenticate

For Windows clients, you can simply call @Auth0.OidcClient.Auth0Client.LoginAsync(System.Object)

```csharp
var loginResult = await client.LoginAsync();
```

For Android and iOS clients, you need to do some manual work to log a user in. The basic flow is as follows:

1. Obtain an `AuthorizeState` by calling @Auth0.OidcClient.Auth0Client.PrepareLoginAsync(System.Object). You will need to save the instance of `AuthorizeState` as you will need it later on.
2. Launch the system browser and direct the user to the `StartUrl` in the returned `AuthorizeState`. After the user has logged in, Auth0 will call back to your application on the Callback URL.
3. Your application will need to handle the callback. The means either creating an `Intent` or Android, or handling `OpenUrl` in your `AppDelegate` on iOS.

For a more detailed walkthrough, please see the relevant [Quickstarts](https://auth0.com/docs/quickstart/native).

## Further Reading

For more information please refer to the [Documentation](documentation/intro.md) section, or to the [API Documentation](api/index.md)