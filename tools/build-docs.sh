#!/bin/bash
# Build Android Project
# msbuild src/Auth0.OidcClient.Android/Auth0.OidcClient.Android.csproj /property:Configuration=Release
# Build iOS Project
# dotnet build src/Auth0.OidcClient.iOS/Auth0.OidcClient.iOS.csproj --configuration Release
# Clear docs folder
rm -rf new-docs
docfx src/Auth0.OidcClient.Android/docs-source/docfx.json 
docfx src/Auth0.OidcClient.iOS/docs-source/docfx.json 
docfx src/Auth0.OidcClient.WPF/docs-source/docfx.json
docfx src/Auth0.OidcClient.WinForms/docs-source/docfx.json 
docfx src/Auth0.OidcClient.UWP/docs-source/docfx.json
# Create root docs
docfx docs-source/docfx.json

mv src/Auth0.OidcClient.Android/docs-source/_site new-docs/android
mv src/Auth0.OidcClient.iOS/docs-source/_site new-docs/ios
mv src/Auth0.OidcClient.WPF/docs-source/_site new-docs/wpf
mv src/Auth0.OidcClient.WinForms/docs-source/_site new-docs/winforms
mv src/Auth0.OidcClient.UWP/docs-source/_site new-docs/uwp