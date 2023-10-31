# Auth0.OidcClient.MAUI

Integrate Auth0 in a MAUI application targetting iOS, macOS or Android by using the `Auth0.OIdcClient.MAUI` SDK.

> ℹ️ This SDK is available in **Beta**! Try it out today, any feedback is appreciated. Just as with any beta release, **using this in production is highly discouraged**.

## Install the SDK

The SDK can be installed through Nuget:

```sh
Install-Package Auth0.OIdcClient.MAUI -IncludePrerelease
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

## Platform specific configuration

In order to use the SDK with Android and Windows, you need some platform specific configuration.

### Android
Create a new Activity that extends `WebAuthenticatorCallbackActivity`:

```csharp
[Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
[IntentFilter(new[] { Intent.ActionView },
              Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
              DataScheme = CALLBACK_SCHEME)]
public class WebAuthenticatorActivity : Microsoft.Maui.Authentication.WebAuthenticatorCallbackActivity
{
    const string CALLBACK_SCHEME = "myapp";
}
```

The above activity will ensure the application can handle the `myapp://callback` URL when Auth0 redirects back to the Android application after logging in.

### Windows

To make sure it can properly reactivate your application after being redirected back go Auth0, you need to do two things:

- Add the corresponding protocol to the `Package.appxmanifest`. In this case, this is set to `myapp`, but you can change this to whatever you like (ensure to update all relevant Auth0 URLs as well).
  ```xml
  <Applications>
    <Application Id="App" Executable="$targetnametoken$.exe" EntryPoint="$targetentrypoint$">
      <Extensions>
        <uap:Extension Category="windows.protocol">
          <uap:Protocol Name="myapp"/>
        </uap:Extension>
      </Extensions>
    </Application>
  </Applications>
  ```
- Call `Activator.Default.CheckRedirectionActivation()` in the Windows specific App.xaml.cs file.
  ```csharp
  public App()
  {
    if (Auth0.OidcClient.Platforms.Windows.Activator.Default.CheckRedirectionActivation())
      return;
  
    this.InitializeComponent();
  }
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
