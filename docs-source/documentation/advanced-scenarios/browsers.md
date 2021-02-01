# Browsers

The SDK is configured to use a platform-specific browser implementation to handle the redirect to Auth0. Depending on the platform, one or more browser implementations might be available.
An example is the `Auth0.OidcClient.iOS` SDK, which auto-selects the browser based on the version of iOS used.

Even though we do not recommend changing this, you could ensure all versions of iOS use the same browser by passing an explicit browser instance to the `Browser` property of the `Auth0ClientOptions`:

```
var client = new Auth0Client(new Auth0ClientOptions {
    Domain = "YOUR_AUTH0_DOMAIN",
    ClientId = "YOUR_AUTH0_CLIENT_ID",
    Browser = new SFSafariViewControllerBrowser()
});
```

In case the default implementations that each SDK provides does not suit the needs for your application, each SDK allows you to specify your own custom browser implementation by implementing the `IBrowser` interface.

```
public class CustomBrowser : IBrowser
{
    protected override Task<BrowserResult> Launch(
        BrowserOptions options,
        CancellationToken cancellationToken = default
    )  {
        var tcs = new TaskCompletionSource<BrowserResult>();

        // Implement custom browser and navigate to options.StartUrl.
        // Set the BrowserResult with the callbackUrl that Auth0 redirects back to,
        // this URL is the URL that contains state and code query parameters.
        tcs.SetResult(new BrowserResult {
            ResultType = BrowserResultType.Success,
            Response = callbackUrl
        });

        return tcs.Task;
    }
}
```

With the `CustomBrowser` implementation in place, instantiate `Auth0Client` with the appropriate `Auth0ClientOptions`:

```
var client = new Auth0Client(new Auth0ClientOptions {
    Domain = "YOUR_AUTH0_DOMAIN",
    ClientId = "YOUR_AUTH0_CLIENT_ID",
    Browser = new CustomBrowser()
});
```

The SDK will automatically call `Browser.Launch()` and use the result that's being set as part of `tcs.SetResult()`.
