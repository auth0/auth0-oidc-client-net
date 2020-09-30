# Callback URLs

Callback URLs are the URLs that Auth0 invokes after the authentication process. Auth0 redirects back to this URL and appends additional parameters to it, including an access code which will be exchanged for an `id_token`, `access_token` and `refresh_token`. 

Since callback URLs can be manipulated, you will need to add your application's URL to your client's *Allowed Callback URLs* for security. This will enable Auth0 to recognize these URLs as valid. If omitted, authentication will not be successful.

The format of the URL will depend on the platform. In the sample callback URLs below, replace `YOUR_AUTH0_DOMAIN` with your actual Auth0 domain. You can find your domain on the Settings tab for your Client inside the [Auth0 dashboard](https://manage.auth0.com/#/): 

![](/images/dashboard-domain.png)

## Universal Windows Platform (UWP)

Use the following callback URL:

```text
https://YOUR_AUTH0_DOMAIN/mobile
```

For example:

```text
https://contoso.auth0.com/mobile
```

## Windows Presentation Foundation (WPF)

Use the following callback URL:

```text
https://YOUR_AUTH0_DOMAIN/mobile
```

For example:

```text
https://contoso.auth0.com/mobile
```

## Windows Forms (WinForms)

Use the following callback URL:

```text
https://YOUR_AUTH0_DOMAIN/mobile
```

For example:

```text
https://contoso.auth0.com/mobile
```

## iOS (uses Xamarin)

For iOS your Callback URL needs to be in the following format:

```text
YOUR_BUNDLE_IDENTIFIER://YOUR_AUTH0_DOMAIN/ios/YOUR_BUNDLE_IDENTIFIER/callback
```

Where `YOUR_BUNDLE_IDENTIFIER` is the Bundle Identifier of your application. So given a Bundle Identifier of `com.contoso.myapp`, and an Auth0 Domain of `contoso.auth0.com`, your callback URL will be the following:

```text
com.contoso.myapp://contoso.auth0.com/ios/com.contoso.myapp/callback
```

## Android (uses Xamarin)

For Android your Callback URL needs to be in the following format:

```text
YOUR_ANDROID_PACKAGE_NAME://YOUR_AUTH0_DOMAIN/android/YOUR_ANDROID_PACKAGE_NAME/callback
```

Where `YOUR_ANDROID_PACKAGE_NAME` is the Package Name of your application. So given a Package Name of `com.contoso.myapp`, and an Auth0 Domain of `contoso.auth0.com`, your callback URL will be the following:

```text
com.contoso.myapp://contoso.auth0.com/android/com.contoso.myapp/callback
```