Breaking Change for Version 1.1.0
---------------------------------

Please note that there was a bug which slipped into the 1.0.0 release of this package. Version 1.1.0 fixed this bug but it can break your application.

The issue was that 1.0.0 required a Callback URL of https://YOUR_AUTH_DOMAIN/mobile. This was wrong. 

The Callback URL should be the in the format ms-app://SID, where SID is the Package SID for your application. 

This has been fixed for version 1.1.0, so please ensure that your callback URL which you register for your Client in the Auth0 Dashboard is in the format ms-app://SID

Full details on how to do this can be found in the UWP Quickstart document under the section "Set Up the Auth0 Callback URL":
https://auth0.com/docs/quickstart/native/windows-uwp-csharp

Apologies for this. This was fixed during the beta versions, but somehome did not make it into the 1.0.0 release of the NuGet package
