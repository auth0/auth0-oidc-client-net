using Android.App;
using Android.Content;
using System;
using System.Linq;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Primary class for performing authentication and authorization operations with Auth0 using the
    /// underlying <see cref="IdentityModel.OidcClient.OidcClient"/>.
    /// </summary>
    public class Auth0Client : Auth0ClientBase
    {
        /// <summary>
        /// Create a new instance of <see cref="Auth0Client"/> with a given <see cref="Auth0ClientOptions"/>.
        /// </summary>
        /// <param name="options">The <see cref="Auth0ClientOptions"/> specifying the configuration to use.</param>
        /// <remarks>options.RedirectUri must match your <see cref="Activity"/> <see cref="IntentFilterPriority"/>
        /// DataScheme, DataPathPrefix and DataHost values.
        /// If not supplied it will presume the convention
        /// <code>$"{Context.PackageName}://{options.Domain}/android/{Context.PackageName}/callback".ToLower();</code>.
        /// Your <see cref="IntentFilterAttribute"/> should have DataScheme, DataPathPrefix and DataHost with values that match.
        /// Alternatively set <see cref="Auth0ClientOptions"/> RedirectUri and PostLogoutRedirectUri to match your <see cref="IntentFilterAttribute"/>. 
        /// DataScheme must be lower-case or Android will not receive the callbacks.
        /// </remarks>
        public Auth0Client(Auth0ClientOptions options)
            : base(options, "xamarin-android")
        {
            options.Browser = options.Browser ?? new AutoSelectBrowser();

            var defaultRedirectUri = options.RedirectUri == null || options.PostLogoutRedirectUri == null
                ? GetConventionCallbackUri(options.Domain) : null;

            options.RedirectUri = options.RedirectUri ?? defaultRedirectUri;
            options.PostLogoutRedirectUri = options.PostLogoutRedirectUri ?? defaultRedirectUri;
        }

        /// <summary>
        /// Create a new instance of <see cref="Auth0Client"/> with a given <see cref="Auth0ClientOptions"/> and <see cref="Context"/>.
        /// </summary>
        /// <param name="options">The <see cref="Auth0ClientOptions"/> specifying the configuration to use.</param>
        /// <param name="activity">The <see cref="Activity"/> with the <see cref="IntentFilterAttribute"/> you perform calls to <see cref="Auth0Client"/> from.</param>
        /// <remarks>options.RedirectUri must match your IntentFilter attribute's DataScheme, DataPathPrefix and DataHost values.
        /// If not supplied it will first try to detect the registered IntentFilter automatically if your supplied <paramref name="activity"/>.
        /// If it does it will presume the convention
        /// <code>$"{Context.PackageName}://{options.Domain}/android/{Context.PackageName}/callback".ToLower();</code>.
        /// Your <see cref="IntentFilter"/> attribute used to register for callbacks should have DataScheme, DataPathPrefix and DataHost with need values
        /// that match.
        /// Alternatively set the RedirectUri manually to match your IntentFilter. Please note that DataScheme should be lower-case or Android
        /// will not listen to callbacks.
        /// </remarks>
        public Auth0Client(Auth0ClientOptions options, Activity activity)
            : base(options, "xamarin-android")
        {
            options.Browser = options.Browser ?? new AutoSelectBrowser(activity);

            var defaultRedirectUri = options.RedirectUri == null || options.PostLogoutRedirectUri == null ?
                GetActivityIntentCallbackUri(activity) ?? GetConventionCallbackUri(options.Domain) : null;

            options.RedirectUri = options.RedirectUri ?? defaultRedirectUri;
            options.PostLogoutRedirectUri = options.PostLogoutRedirectUri ?? defaultRedirectUri;
        }

        private string GetConventionCallbackUri(string domain)
        {
            return $"{Application.Context.PackageName}://{domain}/android/{Application.Context.PackageName}/callback".ToLower();
        }

        /// <summary>
        /// Attempt to find the right <see cref="IntentFilterAttribute"/> for the given
        /// <see cref="Activity"/>.
        /// </summary>
        /// <param name="activity"><see cref="Activity"/> to determine callback Uri from using the associated <see cref="IntentFilterAttribute"/></param>
        /// <returns>A url that can be used as a callback to get to the given <paramref name="activity"/>.</returns>
        private string GetActivityIntentCallbackUri(Activity activity)
        {
            var intents = Attribute
                .GetCustomAttributes(activity.GetType(), typeof(IntentFilterAttribute))
                .OfType<IntentFilterAttribute>()
                .Where(i => IsActionDefaultBrowsable(i) && HasSchemeHostAndPrefix(i))
                .ToList();

            if (intents.Count != 1)
                return null;

            var dataScheme = GetResourcableValue(activity, intents[0].DataScheme);
            var dataHost = GetResourcableValue(activity, intents[0].DataHost);
            var dataPathPrefix = GetResourcableValue(activity, intents[0].DataPathPrefix);

            return $"{dataScheme}://{dataHost}{dataPathPrefix}";
        }

        private bool IsActionDefaultBrowsable(IntentFilterAttribute ifa)
        {
            return ifa.Actions.Contains(Intent.ActionView) &&
                   ifa.Categories.Contains(Intent.CategoryDefault) && 
                   ifa.Categories.Contains(Intent.CategoryBrowsable);
        }

        private bool HasSchemeHostAndPrefix(IntentFilterAttribute ifa)
        {
            return ifa.DataScheme != null && ifa.DataHost != null && ifa.DataPathPrefix != null;
        }

        private string GetResourcableValue(Context context, string value)
        {
            if (!value.StartsWith("@")) return value;
            var resourceId = context.Resources.GetIdentifier(value, null, context.PackageName);
            return resourceId > 0 ? context.Resources.GetString(resourceId) : value;
        }
    }
}
