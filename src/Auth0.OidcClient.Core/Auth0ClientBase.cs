﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Auth0.OidcClient.Tokens;

using Duende.IdentityModel.Client;
using Duende.IdentityModel.OidcClient;
using Duende.IdentityModel.OidcClient.Browser;
using Duende.IdentityModel.OidcClient.Results;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Base class for performing authentication and authorization operations with Auth0 using the
    /// underlying <see cref="Duende.IdentityModel.OidcClient.OidcClient"/>.
    /// </summary>
    public abstract class Auth0ClientBase : IAuth0Client
    {
        private readonly IdTokenRequirements _idTokenRequirements;
        private readonly IdTokenValidator _idTokenValidator;
        private readonly Auth0ClientOptions _options;
        private readonly string _userAgent;
        private Duende.IdentityModel.OidcClient.OidcClient _oidcClient;
        private Duende.IdentityModel.OidcClient.OidcClient OidcClient
        {
            get
            {
                return _oidcClient ?? (_oidcClient = new Duende.IdentityModel.OidcClient.OidcClient(CreateOidcClientOptions(_options)));
            }
        }

        /// <summary>
        /// Create a new instance of <see cref="Auth0ClientBase"/>.
        /// </summary>
        /// <param name="options"><see cref="Auth0ClientOptions"/> specifying the configuration options for this client.</param>
        /// <param name="platformName">Platform name that forms part of the user-agent when communicating with Auth0 servers.</param>
        protected Auth0ClientBase(Auth0ClientOptions options, string platformName)
        {
            _options = options;
            _idTokenRequirements = new IdTokenRequirements($"https://{_options.Domain}/", _options.ClientId, options.Leeway, options.MaxAge);
            _userAgent = CreateAgentString(platformName);
            _idTokenValidator = new IdTokenValidator(options.BackchannelHandler);
        }

        /// <inheritdoc />
        public async Task<LoginResult> LoginAsync(object extraParameters = null, CancellationToken cancellationToken = default)
        {
            var finalExtraParameters = AppendTelemetry(extraParameters);
            if (_options.MaxAge.HasValue)
                finalExtraParameters["max_age"] = _options.MaxAge.Value.TotalSeconds.ToString("0");

            var loginRequest = new LoginRequest { FrontChannelExtraParameters = new Parameters(finalExtraParameters) };
            
            Debug.WriteLine($"Using Callback URL '{OidcClient.Options.RedirectUri}'. Ensure this is an Allowed Callback URL for application/client ID {_options.ClientId}.");

            var result = await OidcClient.LoginAsync(loginRequest, cancellationToken);

            if (!result.IsError)
            {
                if (finalExtraParameters.ContainsKey("organization"))
                {
                    _idTokenRequirements.Organization = finalExtraParameters["organization"];
                }

                await _idTokenValidator.AssertTokenMeetsRequirements(_idTokenRequirements, result.IdentityToken); // Nonce is created & tested by OidcClient
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<BrowserResultType> LogoutAsync(bool federated = false, object extraParameters = null, CancellationToken cancellationToken = default)
        {
            Debug.WriteLine($"Using Callback URL '{OidcClient.Options.PostLogoutRedirectUri}'. Ensure this is an Allowed Logout URL for application/client ID {_options.ClientId}.");

            var logoutParameters = AppendTelemetry(extraParameters);
            logoutParameters["client_id"] = OidcClient.Options.ClientId;
            logoutParameters["returnTo"] = OidcClient.Options.PostLogoutRedirectUri;

            var endSessionUrl = new RequestUrl($"https://{_options.Domain}/v2/logout").Create(new Parameters(logoutParameters));
            if (federated)
                endSessionUrl += "&federated";

            var logoutRequest = new LogoutRequest();
            var browserOptions = new BrowserOptions(endSessionUrl, OidcClient.Options.PostLogoutRedirectUri ?? string.Empty)
            {
                Timeout = TimeSpan.FromSeconds(logoutRequest.BrowserTimeout),
                DisplayMode = logoutRequest.BrowserDisplayMode
            };

            var browserResult = await OidcClient.Options.Browser.InvokeAsync(browserOptions, cancellationToken);

            return browserResult.ResultType;
        }

        /// <inheritdoc/>
        public Task<AuthorizeState> PrepareLoginAsync(object extraParameters = null, CancellationToken cancellationToken = default)
        {
            return OidcClient.PrepareLoginAsync(new Parameters(AppendTelemetry(extraParameters)), cancellationToken);
        }

        /// <inheritdoc/>
        public Task<LoginResult> ProcessResponseAsync(string data, AuthorizeState state, object extraParameters = null, CancellationToken cancellationToken = default)
        {
            return OidcClient.ProcessResponseAsync(data, state, new Parameters(AppendTelemetry(extraParameters)), cancellationToken);
        }

        /// <inheritdoc/>
        public Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            return this.RefreshTokenAsync(refreshToken, null, null, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken, object extraParameters, CancellationToken cancellationToken = default)
        {
            return this.RefreshTokenAsync(refreshToken, null, extraParameters, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken, string scope, object extraParameters = null, CancellationToken cancellationToken = default)
        {
            var finalExtraParameters = AppendTelemetry(extraParameters);
            var result = await OidcClient.RefreshTokenAsync(refreshToken, new Parameters(finalExtraParameters), scope, cancellationToken: cancellationToken);

            if (!result.IsError)
            {
                if (finalExtraParameters.ContainsKey("Organization"))
                {
                    _idTokenRequirements.Organization = finalExtraParameters["Organization"];
                }

                await _idTokenValidator.AssertTokenMeetsRequirements(_idTokenRequirements, result.IdentityToken); // Nonce is created & tested by OidcClient
            }

            return result;
        }

        /// <inheritdoc/>
        public Task<UserInfoResult> GetUserInfoAsync(string accessToken)
        {
            return OidcClient.GetUserInfoAsync(accessToken);
        }

        private OidcClientOptions CreateOidcClientOptions(Auth0ClientOptions options)
        {
            var scopes = options.Scope.Split(' ').ToList();
            if (!scopes.Contains("openid"))
                scopes.Insert(0, "openid");

            var oidcClientOptions = new OidcClientOptions
            {
                Authority = $"https://{options.Domain}",
                ClientId = options.ClientId,
                Scope = String.Join(" ", scopes),
                LoadProfile = options.LoadProfile,
                Browser = options.Browser,
                RedirectUri = options.RedirectUri ?? $"https://{_options.Domain}/mobile",
                PostLogoutRedirectUri = options.PostLogoutRedirectUri ?? $"https://{_options.Domain}/mobile",
                ClockSkew = options.Leeway,
                LoggerFactory = options.LoggerFactory,

                Policy = {
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

            if (_options.EnableTelemetry && !dictionary.ContainsKey("auth0Client"))
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
            if (values is Dictionary<string, string> dictionary)
                return dictionary.ToDictionary(entry => entry.Key, entry => entry.Value);

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