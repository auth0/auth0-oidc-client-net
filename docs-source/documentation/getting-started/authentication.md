# Authentication

## Initializing Auth0Client and logging the user in

To authenticate a user in your application, your need to create a new instance of @Auth0.OidcClient.Auth0Client, passing your Auth0 **Domain** and **Client ID** for your Client. Please see the [Clients Documentation](https://auth0.com/docs/clients) on the Auth0 website for more information.

Once you have instantiated an instance of @Auth0.OidcClient.Auth0Client, you can use it to authenticate a user. 

```csharp
using Auth0.OidcClient;

var client = new Auth0Client(new Auth0ClientOptions
{
    Domain = "YOUR_AUTH0_DOMAIN",
    ClientId = "YOUR_AUTH0_CLIENT_ID"
});
```

The way in which you authenticate the user will be different for each platform.

### For Windows Applications (UWP, WPF and Windows Forms)

For Windows applications, you can authenticate a user by calling @Auth0.OidcClient.Auth0Client.LoginAsync(System.Object).

```csharp
var loginResult = await client.LoginAsync();
```

### For Android

1. First you need to call @Auth0.OidcClient.Auth0Client.PrepareLoginAsync(System.Object). This will return an `AuthorizeState` instance:

    ```
    authorizeState = await _client.PrepareLoginAsync();
    ```

    You will need to store the instance of `AuthorizeState`, as you will need it in step 3 below.

2. The `AuthorizeState` instance will contain a `StartUrl` property with the authorization URL where the user should be redirected to log in. You can use the [Chrome Custom Tabs Manager](https://developer.chrome.com/multidevice/android/customtabs) to achieve this.

    Ensure that you have installed the Custom Tabs Support Library:

    ```text
    Install-Package Xamarin.Android.Support.CustomTabs
    ```

    Then create a custom tabs intent, and launch the authorization URL which was returned in the `AuthorizeState`:

    ```
    var customTabs = new CustomTabsActivityManager(this); // this == your Activity

    // build custom tab
    var builder = new CustomTabsIntent.Builder(customTabs.Session)
        .SetToolbarColor(Color.Argb(255, 52, 152, 219))
        .SetShowTitle(true)
        .EnableUrlBarHiding();

    var customTabsIntent = builder.Build();
    customTabsIntent.Intent.AddFlags(ActivityFlags.NoHistory);

    customTabsIntent.LaunchUrl(this, Android.Net.Uri.Parse(authorizeState.StartUrl));
    ```

3. Finally, after the user has authenticated, they will be redirected back to your application at the **Callback URL** that was registered before. You will need to register an intent which will handle this callback URL.

    ```csharp
    [Activity(Label = "AndroidSample", MainLauncher = true, Icon = "@drawable/icon",
        LaunchMode = LaunchMode.SingleTask)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "YOUR_ANDROID_PACKAGE_NAME",
        DataHost = "YOUR_AUTH0_DOMAIN",
        DataPathPrefix = "/android/YOUR_ANDROID_PACKAGE_NAME/callback")]
    public class MainActivity : Activity
    {
        // Code omitted
    }
    ```

    Now write code to handle the intent. You can do this by overriding the `OnNewIntent` method. Inside the method you need to call @Auth0.OidcClient.Auth0Client.ProcessResponseAsync(System.String,IdentityModel.OidcClient.AuthorizeState), passing along the `DataString` from the intent, as well as the `AuthorizeState` which was previously stored when you called `PrepareLoginAsync`:

    ```csharp
    protected override async void OnNewIntent(Intent intent)
    {
        base.OnNewIntent(intent);

        var loginResult = await client.ProcessResponseAsync(intent.DataString, authorizeState);
    }
    ```

### For iOS

TBD

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

