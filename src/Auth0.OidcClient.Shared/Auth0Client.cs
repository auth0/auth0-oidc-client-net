using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using IdentityModel.OidcClient.Results;

namespace Auth0.OidcClient
{
    public class Auth0Client : IAuth0Client
    {
        private readonly Auth0ClientOptions _options;
        private IdentityModel.OidcClient.OidcClient _oidcClient;

        /// <summary>
        /// Creates a new instance of the Auth0 OIDC Client.
        /// </summary>
        /// <param name="options">The <see cref="Auth0ClientOptions"/> specifying the configuration for the Auth0 OIDC Client.</param>
        public Auth0Client(Auth0ClientOptions options)
        {
            _options = options;

            ConfigureOidcClient();
        }

        private void ConfigureOidcClient()
        {
            var authority = $"https://{_options.Domain}";
#if __ANDROID__
            string packageName = Android.App.Application.Context.PackageName;
#endif

            // Determine redirect uri depending on platform
#if __IOS__
			string redirectUri = $"{Foundation.NSBundle.MainBundle.BundleIdentifier}://{_options.Domain}/ios/{Foundation.NSBundle.MainBundle.BundleIdentifier}/callback";
#elif __ANDROID__
			string redirectUri = $"{packageName}://{_options.Domain}/android/{packageName}/callback".ToLower();
#elif WINDOWS_UWP
            string redirectUri = Windows.Security.Authentication.Web.WebAuthenticationBroker.GetCurrentApplicationCallbackUri().AbsoluteUri;
#else
            string redirectUri = $"https://{_options.Domain}/mobile";
#endif

            var oidcClientOptions = new OidcClientOptions
            {
                Authority = authority,
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret,
                Scope = _options.Scope,
                LoadProfile = _options.LoadProfile,
                Browser = _options.Browser ?? new PlatformWebView(),
                Flow = OidcClientOptions.AuthenticationFlow.AuthorizationCode,

				RedirectUri = _options.RedirectUri ?? redirectUri,
                PostLogoutRedirectUri = _options.PostLogoutRedirectUri ?? redirectUri,

                // Set correct response mode depending on the platform
#if WINDOWS_UWP
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.FormPost,
#else
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect,
#endif
                Policy =
                {
                    RequireAuthorizationCodeHash = false,
                    RequireAccessTokenHash = false
                }
            };
            _oidcClient = new IdentityModel.OidcClient.OidcClient(oidcClientOptions);
        }

        private Dictionary<string, string> AppendTelemetry(object values)
        {
            var dictionary = ObjectToDictionary(values);

            if (_options.EnableTelemetry)
                dictionary.Add("auth0Client", CreateAgentString());

            return dictionary;
        }

        private string CreateAgentString()
        {
#if __ANDROID__
            string platform = "xamarin-android";
#elif __IOS__
            string platform = "xamarin-ios";
#elif WINFORMS
            string platform = "winforms";
#elif WPF
            string platform = "wpf";
#elif WINDOWS_UWP
            var platform = "uwp";
#endif
            var sdkVersion = typeof(Auth0Client).GetTypeInfo().Assembly.GetName().Version;

            var agentJson = $"{{\"name\":\"oidc-net\",\"version\":\"{sdkVersion.Major}.{sdkVersion.Minor}.{sdkVersion.Revision}\",\"env\":{{\"platform\":\"{platform}\"}}}}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(agentJson));
        }

        /// <summary>
        /// Launches a browser to log the user in.
        /// </summary>
        /// <param name="extraParameters">Any extra parameters that need to be passed to the authorization endpoint.</param>
        /// <returns></returns>
        public Task<LoginResult> LoginAsync(object extraParameters = null)
        {
            var loginRequest = new LoginRequest
            {
                FrontChannelExtraParameters = AppendTelemetry(extraParameters)
            };
            return _oidcClient.LoginAsync(loginRequest);
        }

        private Dictionary<string, string> ObjectToDictionary(object values)
        {
            var dictionary = values as Dictionary<string, string>;
            if (dictionary != null)
                return dictionary;

            dictionary = new Dictionary<string, string>();
            if (values != null)
                foreach (var prop in values.GetType().GetRuntimeProperties())
                {
                    var value = prop.GetValue(values) as string;
                    if (!string.IsNullOrEmpty(value))
                        dictionary.Add(prop.Name, value);
                }

            return dictionary;
        }

        /// <summary>
        /// Launches a browser to log the user out and clear the Auth0 SSO Cookie
        /// </summary>
        /// <returns></returns>
        public Task<BrowserResultType> LogoutAsync()
        {
            return LogoutAsync(false);
        }

        /// <inheritdoc />
        public async Task<BrowserResultType> LogoutAsync(bool federated)
        {
            var logoutUrl = $"https://{_options.Domain}/v2/logout";

            var dictionary = new Dictionary<string, string>();
            dictionary.Add("client_id", _oidcClient.Options.ClientId);
            dictionary.Add("returnTo", _oidcClient.Options.PostLogoutRedirectUri);

            string endSessionUrl = new RequestUrl(logoutUrl).Create(dictionary);
            if (federated)
                endSessionUrl += "&federated";
            var logoutRequest = new LogoutRequest();

            BrowserResult browserResult = await _oidcClient.Options.Browser.InvokeAsync(new BrowserOptions(endSessionUrl, _oidcClient.Options.PostLogoutRedirectUri ?? string.Empty)
            {
                Timeout = TimeSpan.FromSeconds(logoutRequest.BrowserTimeout),
                DisplayMode = logoutRequest.BrowserDisplayMode
            });

            return browserResult.ResultType;
        }

        /// <summary>
        /// Generates an <see cref="AuthorizeState"/> containing the URL, state, nonce and code challenge which can
        /// be used to redirect the user to the authorization URL, and subsequently process any response by calling
        /// the <see cref="ProcessResponseAsync"/> method.
        /// </summary>
        /// <param name="extraParameters"></param>
        /// <returns></returns>
        public Task<AuthorizeState> PrepareLoginAsync(object extraParameters = null)
        {
            return _oidcClient.PrepareLoginAsync(AppendTelemetry(extraParameters));
        }

        /// <summary>
        /// Process the response from the Auth0 redirect URI
        /// </summary>
        /// <param name="data">The data containing the full redirect URI.</param>
        /// <param name="state">The <see cref="AuthorizeState"/> which was generated when the <see cref="PrepareLoginAsync"/>
        /// method was called.</param>
        /// <returns></returns>
        public Task<LoginResult> ProcessResponseAsync(string data, AuthorizeState state)
        {
            return _oidcClient.ProcessResponseAsync(data, state);
        }

        /// <summary>
        /// Generates a new set of tokens based on a refresh token. 
        /// </summary>
        /// <param name="refreshToken">The refresh token which was issued during the authorization flow, or subsequent
        /// calls to <see cref="RefreshTokenAsync"/>.</param>
        /// <returns></returns>
        public Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken)
        {
            return _oidcClient.RefreshTokenAsync(refreshToken);
        }
    }
}