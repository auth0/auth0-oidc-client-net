using Android.App;
using Android.Content;
using System;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Primary class for performing authentication and authorization operations with Auth0 using the
    /// underlying <see cref="IdentityModel.OidcClient.OidcClient"/>.
    /// </summary>
    public class Auth0Client : Auth0ClientBase
    {
        /// <summary>
        /// Creates a new instance of the Auth0 OIDC Client.
        /// </summary>
        /// <param name="options">The <see cref="Auth0ClientOptions"/> specifying the configuration for the Auth0 OIDC Client.</param>
        /// <param name="context">The <see cref="Context"/> you start the requests from. Usually the <see cref="Activity"/> with the <see cref="IntentFilterAttribute"/>.</param>
        /// <remarks>options.RedirectUri must match your IntentFilter attribute's DataScheme, DataPathPrefix and DataHost values.
        /// If not supplied it will first try to detect the registered IntentFilter automatically if your supplied <paramref name="context"/> inherits from 
        /// <see cref="Auth0ClientActivity"/>.
        /// If it does not then it will make a best-guess using the format
        /// <code>$"{Context.PackageName}://{options.Domain}/android/{Context.PackageName}/callback".ToLower();</code>.
        /// Your IntentFilter attribute used to register for callbacks should have DataScheme, DataPathPrefix and DataHost with need values
        /// that match.
        /// Alternatively set the RedirectUri manually to match your IntentFilter. Please note that DataScheme should be lower-case or Android
        /// will not listen to callbacks.
        /// </remarks>
        public Auth0Client(Auth0ClientOptions options, Context context = null)
            : base(options, "xamarin-android")
        {
            context = context ?? Application.Context;
            options.Browser = options.Browser ?? new AutoSelectBrowser(context);

            var defaultCallbackUrl = GetCallbackUri(context) ?? $"{context.PackageName}://{options.Domain}/android/{context.PackageName}/callback".ToLower();
            options.RedirectUri = options.RedirectUri ?? defaultCallbackUrl;
            options.PostLogoutRedirectUri = options.PostLogoutRedirectUri ?? defaultCallbackUrl;
        }

        private string GetCallbackUri(Context context)
        {
            var intent = (IntentFilterAttribute)Attribute.GetCustomAttribute(context.GetType(), typeof(IntentFilterAttribute));
            if (intent == default)
                return null;

            if (intent.DataScheme == null || intent.DataHost == null || intent.DataPathPrefix == null)
                return null;

            var dataSchemeId = context.Resources.GetIdentifier(intent.DataScheme, null, context.PackageName);
            var dataScheme = dataSchemeId > 0 ? context.Resources.GetString(dataSchemeId) : intent.DataScheme;

            var dataHostId = context.Resources.GetIdentifier(intent.DataHost, null, context.PackageName);
            var dataHost = dataHostId > 0 ? context.Resources.GetString(dataHostId) : intent.DataHost;

            var dataPathPrefixId = context.Resources.GetIdentifier(intent.DataPathPrefix, null, context.PackageName);
            var dataPathPrefix = dataPathPrefixId > 0 ? context.Resources.GetString(dataPathPrefixId) : intent.DataPathPrefix;

            return $"{dataScheme}://{dataHost}{dataPathPrefix}";
        }
    }
}
