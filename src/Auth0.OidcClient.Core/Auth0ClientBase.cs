using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using IdentityModel.OidcClient.Results;
using static IdentityModel.OidcClient.OidcClientOptions;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Base class for performing authentication and authorization operations with Auth0 using the
    /// underlying <see cref="IdentityModel.OidcClient.OidcClient"/>.
    /// </summary>
    public abstract class Auth0ClientBase : IAuth0Client
    {
        private readonly Auth0ClientOptions _options;
        private readonly string _userAgent;
        private readonly bool _formPost;
        private IdentityModel.OidcClient.OidcClient _oidcClient;

        /// <summary>
        /// Creates a new instance of the Auth0 OIDC Client.
        /// </summary>
        /// <param name="options">The <see cref="Auth0ClientOptions"/> specifying the configuration for the Auth0 OIDC Client.</param>
        /// <param name="platformName">The platform name that forms part of the user-agent when communicating with Auth0 servers.</param>
        public Auth0ClientBase(Auth0ClientOptions options, string platformName)
        {
            _options = options;
            _userAgent = CreateAgentString(platformName);
        }

        /// <inheritdoc/>
        public Task<LoginResult> LoginAsync(object extraParameters = null)
        {
            var loginRequest = new LoginRequest
            {
                FrontChannelExtraParameters = AppendTelemetry(extraParameters)
            };
            return OidcClient.LoginAsync(loginRequest);
        }

        /// <inheritdoc/>
        public Task<BrowserResultType> LogoutAsync()
        {
            return LogoutAsync(false);
        }

        private IdentityModel.OidcClient.OidcClient OidcClient
        {
            get
            {
                return _oidcClient ?? (_oidcClient = new IdentityModel.OidcClient.OidcClient(CreateOidcClientOptions(_options)));
            }
        }

        /// <inheritdoc/>
        public async Task<BrowserResultType> LogoutAsync(bool federated)
        {
            var logoutParameters = new Dictionary<string, string>
            {
                { "client_id", OidcClient.Options.ClientId },
                { "returnTo", OidcClient.Options.PostLogoutRedirectUri }
            };

            var endSessionUrl = new RequestUrl($"https://{_options.Domain}/v2/logout").Create(logoutParameters);
            if (federated)
                endSessionUrl += "&federated";

            var logoutRequest = new LogoutRequest();
            var browserOptions = new BrowserOptions(endSessionUrl, OidcClient.Options.PostLogoutRedirectUri ?? string.Empty)
            {
                Timeout = TimeSpan.FromSeconds(logoutRequest.BrowserTimeout),
                DisplayMode = logoutRequest.BrowserDisplayMode
            };

            var browserResult = await OidcClient.Options.Browser.InvokeAsync(browserOptions);

            return browserResult.ResultType;
        }

        /// <inheritdoc/>
        public Task<AuthorizeState> PrepareLoginAsync(object extraParameters = null)
        {
            return OidcClient.PrepareLoginAsync(AppendTelemetry(extraParameters));
        }

        /// <inheritdoc/>
        public Task<LoginResult> ProcessResponseAsync(string data, AuthorizeState state)
        {
            return OidcClient.ProcessResponseAsync(data, state);
        }

        /// <inheritdoc/>
        public Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken)
        {
            return RefreshTokenAsync(refreshToken, null);
        }

        /// <inheritdoc/>
        public Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken, object extraParameters)
        {
            return OidcClient.RefreshTokenAsync(refreshToken, extraParameters);
        }

        private OidcClientOptions CreateOidcClientOptions(Auth0ClientOptions options)
        {
            var oidcClientOptions = new OidcClientOptions
            {
                Authority = $"https://{options.Domain}",
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret,
                Scope = options.Scope,
                LoadProfile = options.LoadProfile,
                Browser = options.Browser,
                Flow = AuthenticationFlow.AuthorizationCode,
                ResponseMode = _formPost ? AuthorizeResponseMode.FormPost : AuthorizeResponseMode.Redirect,
                RedirectUri = options.RedirectUri ?? $"https://{_options.Domain}/mobile",
                PostLogoutRedirectUri = options.PostLogoutRedirectUri,

                Policy = {
                    RequireAuthorizationCodeHash = false,
                    RequireAccessTokenHash = false
                }
            };

            if (options.RefreshTokenMessageHandler != null)
                oidcClientOptions.RefreshTokenInnerHttpHandler = options.RefreshTokenMessageHandler;

            if (options.BackchannelHandler != null)
                oidcClientOptions.BackchannelHandler = options.BackchannelHandler;

            return oidcClientOptions;
        }

        private Dictionary<string, string> AppendTelemetry(object values)
        {
            var dictionary = ObjectToDictionary(values);

            if (_options.EnableTelemetry)
                dictionary.Add("auth0Client", _userAgent);

            return dictionary;
        }

        private string CreateAgentString(string platformName)
        {
            var sdkVersion = typeof(Auth0ClientBase).GetTypeInfo().Assembly.GetName().Version;
            var agentJson = $"{{\"name\":\"oidc-net\",\"version\":\"{sdkVersion.Major}.{sdkVersion.Minor}.{sdkVersion.Revision}\",\"env\":{{\"platform\":\"{platformName}\"}}}}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(agentJson));
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
    }
}