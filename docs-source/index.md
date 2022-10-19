![](./banner.png)

OIDC Client for .NET Desktop (WinForms, WPF and UWP) as well as Xamarin-based iOS and Android mobile applications.

[![NuGet version](https://img.shields.io/nuget/v/auth0.oidcclient.core.svg?style=flat)](https://www.nuget.org/packages/Auth0.OidcClient.Core/)
![Downloads](https://img.shields.io/nuget/dt/Auth0.OidcClient.Core)
[![License](https://img.shields.io/:license-Apache2.0-blue.svg?style=flat)](https://opensource.org/licenses/Apache-2.0)
[![Build Status](https://dev.azure.com/Auth0SDK/Auth0%20OIDC%20Client.NET/_apis/build/status/auth0.auth0-oidc-client-net?branchName=master)](https://dev.azure.com/Auth0SDK/Auth0%20OIDC%20Client.NET/_build/latest?definitionId=4&branchName=master)

:books: [Documentation](#documentation) - :rocket: [Getting Started](#getting-started) - :computer: [API Reference](#api-reference) - :speech_balloon: [Feedback](#feedback)

This library makes use of the [IdentityModel/IdentityModel.OidcClient](https://github.com/IdentityModel/IdentityModel.OidcClient) library and uses code from the [IdentityModel/IdentityModel.OidcClient.Samples](https://github.com/IdentityModel/IdentityModel.OidcClient.Samples) repository to achieve browser integration.

## Documentation

- [UWP Quickstart](https://auth0.com/docs/quickstart/native/windows-uwp-csharp) - our interactive guide for quickly adding login, logout and user information to a UWP application using Auth0.
- [WPF/WinForms Quickstart](https://auth0.com/docs/quickstart/native/wpf-winforms) - our interactive guide for quickly adding login, logout and user information to a WPF and WinForms application using Auth0.
- [Xamarin Quickstart](https://auth0.com/docs/quickstart/native/xamarin) - our interactive guide for quickly adding login, logout and user information to a Xamarin application using Auth0.
- [WPF Sample App](https://github.com/auth0-samples/auth0-WinFormsWPF-oidc-samples/tree/master/Quickstart/00-Starter-Seed/WPF) - a WPF application integrated with Auth0.
- [WinForms Sample App](https://github.com/auth0-samples/auth0-WinFormsWPF-oidc-samples/tree/master/Quickstart/00-Starter-Seed/WinForms) - a WinForms application integrated with Auth0.
- [Xamarin Sample App](https://github.com/auth0-samples/auth0-xamarin-oidc-samples/tree/master/Quickstart/01-Login) - a Xamarin application integrated with Auth0.
- [SDK docs](https://auth0.github.io/auth0-oidc-client-net/documentation/intro.html) - explore the documentation for this SDK. 
- [Auth0 docs](https://www.auth0.com/docs) - explore our docs site and learn more about 


## Getting started

### Requirements

For a list of supported platforms, please refer to the relevant documentation from Microsoft:

* [Xamarin](https://docs.microsoft.com/en-us/xamarin/get-started/supported-platforms)
* [UWP/WPF/WinForms](https://docs.microsoft.com/en-us/lifecycle/faq/windows)

### Installation
The SDK is available on [Nuget](https://www.nuget.org/packages?q=Auth0.OidcClient) for different platforms:

```
Install-Package Auth0.OidcClient.UWP
Install-Package Auth0.OidcClient.WPF
Install-Package Auth0.OidcClient.WinForms
Install-Package Auth0.OidcClient.iOS
Install-Package Auth0.OidcClient.Android
Install-Package Auth0.OidcClient.AndroidX
```

### Configure Auth0

Create a **Native Application** in the [Auth0 Dashboard](https://manage.auth0.com/#/applications).

> **If you're using an existing application**, verify that you have configured the following settings in your Native Application:
>
> - Click on the "Settings" tab of your application's page.
> - Ensure that "Token Endpoint Authentication Method" under "Application Properties" is set to "None"
> - Scroll down and click on the "Show Advanced Settings" link.
> - Under "Advanced Settings", click on the "OAuth" tab.
> - Ensure that "JsonWebToken Signature Algorithm" is set to `RS256` and that "OIDC Conformant" is enabled.

Next, configure the following URLs for your application under the "Application URIs" section of the "Settings" page:

- **Allowed Callback URLs**
- **Allowed Logout URLs**

> For the values for these URLs, please refer to the corresponding quickstart from our [documentation](#documentation).

Take note of the **Client ID** and **Domain** values under the "Basic Information" section. You'll need these values to configure the SDK.

### Configure the SDK
All platforms share the same interface, so you can use the following code to instantiate the `Auth0Client`:

```csharp
using Auth0.OidcClient;
// ...
var auth0Client = new Auth0Client(new Auth0ClientOptions
{
    Domain = "YOUR_AUTH0_DOMAIN",
    ClientId = "YOUR_AUTH0_CLIENT_ID"
});
```

## API reference
Read [the full API reference](https://auth0.github.io/auth0-oidc-client-net/api/Auth0.OidcClient.html) to find out about the public API's this SDK exposes.

## Feedback
### Contributing

We appreciate feedback and contribution to this repo! Before you get started, please see the following:

- [Auth0's general contribution guidelines](https://github.com/auth0/open-source-template/blob/master/GENERAL-CONTRIBUTING.md)
- [Auth0's code of conduct guidelines](https://github.com/auth0/open-source-template/blob/master/CODE-OF-CONDUCT.md)
- [This repo's contribution guide](https://github.com/auth0/auth0-oidc-client-net/blob/master/CONTRIBUTING.md)

### Raise an issue

To provide feedback or report a bug, please [raise an issue on our issue tracker](https://github.com/auth0/auth0-oidc-client-net/issues).

### Vulnerability Reporting

Please do not report security vulnerabilities on the public GitHub issue tracker. The [Responsible Disclosure Program](https://auth0.com/responsible-disclosure-policy) details the procedure for disclosing security issues.

---

<p align="center">
  <picture>
    <source media="(prefers-color-scheme: light)" srcset="https://cdn.auth0.com/website/sdks/logos/auth0_light_mode.png"   width="150">
    <source media="(prefers-color-scheme: dark)" srcset="https://cdn.auth0.com/website/sdks/logos/auth0_dark_mode.png" width="150">
    <img alt="Auth0 Logo" src="https://cdn.auth0.com/website/sdks/logos/auth0_light_mode.png" width="150">
  </picture>
</p>
<p align="center">Auth0 is an easy to implement, adaptable authentication and authorization platform. To learn more checkout <a href="https://auth0.com/why-auth0">Why Auth0?</a></p>
<p align="center">
This project is licensed under the Apache-2.0 license. See the <a href="https://github.com/auth0/auth0-oidc-client-net/blob/master/LICENSE"> LICENSE</a> file for more info.</p>
