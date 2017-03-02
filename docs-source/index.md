# Auth0 OIDC Client

The Auth0 OIDC Client allows you to add authentication for your .NET Client applications. The Auth0 OIDC Client supports the following platforms:

* Universal Windows Platform
* Windows Presentation Foundation (.NET 4.52 and higher)
* Windows Forms (.NET 4.52 and higher)

## Quick Start

### 1. Install the NuGet Package for your platform

**Universal Windows Platform (UWP)**

```text
Install-Package Auth0.OidcClient.UWP -Pre
```

**Windows Presentation Foundation (WPF)**

```text
Install-Package Auth0.OidcClient.WPF -Pre
```

**Windows Forms**

```text
Install-Package Auth0.OidcClient.WinForms -Pre
```

### 2. Initialize 

```csharp
using Auth0.OidcClient;

var client = new Auth0Client("YOUR_AUTH0_DOMAIN", "YOUR_AUTH0_CLIENT_ID");
```

### 3. Authenticate

```csharp
var loginResult = await client.LoginAsync();
```

## Further Reading

For more information please refer to the [Documentation](documentation/intro.md) section, or to the [API Documentation](api/index.md)