# Authentication

> [!Note]
> You need to ensure that the JWT Signature Algorithm for your Auth0 Application is set to RS256

## Initialize Auth0Client

Create a new instance of @Auth0.OidcClient.Auth0Client, passing the Auth0 **Domain** and **Client ID** for your Auth0 Application. Please see the [Applications Documentation](https://auth0.com/docs/applications) on the Auth0 website for more information.

```csharp
using Auth0.OidcClient;

var client = new Auth0Client(new Auth0ClientOptions
{
    Domain = "YOUR_AUTH0_DOMAIN",
    ClientId = "YOUR_AUTH0_CLIENT_ID"
});
```

## Log the user in

Initiate the authentication flow by calling @Auth0.OidcClient.Auth0Client.LoginAsync(System.Object). There are slight nuances to this on some of the platforms, as discussed below.

### For Windows Applications (UWP, WPF and Windows Forms)

For Windows applications, you can authenticate a user by calling @Auth0.OidcClient.Auth0Client.LoginAsync(System.Object).

```csharp
var loginResult = await client.LoginAsync();
```

### For iOS

For iOS applications, the process is similar, but you also need to register the URL Scheme.

1. Register the URL Scheme as follows:

    * Open your application's `Info.plist` file in Visual Studio for Mac, and go to the **Advanced** tab.
    * Under **URL Types**, click the **Add URL Type** button
    * Set the **Identifier** as `Auth0`, the **URL Schemes** the same as your application's **Bundle Identifier**, and the **Role** as `None`

    This is an example of the XML representation of your `info.plist` file after you have added the URL Type:

    ```xml
    <key>CFBundleURLTypes</key>
    <array>
        <dict>
            <key>CFBundleTypeRole</key>
            <string>None</string>
            <key>CFBundleURLName</key>
            <string>Auth0</string>
            <key>CFBundleURLSchemes</key>
            <array>
                <string>YOUR_BUNDLE_IDENTIFIER</string>
            </array>
        </dict>
    </array>
    ```

2. Initiate the authentication process the same way as for Windows applications, by calling @Auth0.OidcClient.Auth0Client.LoginAsync(System.Object) inside your view controller:

    ```csharp
    var loginResult = await client.LoginAsync();
    ```

### For Android

For Android, the entire login process is a bit more manual but still fairly simple.

1. First you need to call @Auth0.OidcClient.Auth0Client.PrepareLoginAsync(System.Object). This will return an `AuthorizeState` instance:

    ```
    authorizeState = await _client.PrepareLoginAsync();
    ```

    You will need to store the instance of `AuthorizeState`, as you will need it in step 3 below.

2. The `AuthorizeState` instance will contain a `StartUrl` property with the authorization URL where the user should be redirected to log in. You will need to create a new intent which will open the system browser and send the user to this URL:

    ```csharp
    var uri = Android.Net.Uri.Parse(authorizeState.StartUrl);
    var intent = new Intent(Intent.ActionView, uri);
    intent.AddFlags(ActivityFlags.NoHistory);
    StartActivity(intent);
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

## The Login Result

The returned login result will indicate whether authentication was successful and if so contain the tokens and claims of the user.

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

To obtain information about the user, you can query the claims. You can, for example, obtain the user's name and email address from the `name` and `email` claims:

```csharp
if (!loginResult.IsError)
{
    Debug.WriteLine($"name: {loginResult.User.FindFirst(c => c.Type == "name")?.Value}");
    Debug.WriteLine($"email: {loginResult.User.FindFirst(c => c.Type == "email")?.Value}");
}
```

> [!Note]
> The exact claims returned will depend on the scopes requested. For more information see @scopes.

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

