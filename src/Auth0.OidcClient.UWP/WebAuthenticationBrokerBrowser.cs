using IdentityModel.OidcClient.Browser;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Implements the <see cref="IBrowser"/> interface using <see cref="WebAuthenticationBroker"/>.
    /// </summary>
    public class WebAuthenticationBrokerBrowser : IBrowser
    {
        private readonly bool _enableWindowsAuthentication;

        /// <summary>
        /// Create a new instance of <see cref="WebAuthenticationBrokerBrowser"/> class specifying if
        /// Windows authentication should be enabled.
        /// </summary>
        /// <param name="enableWindowsAuthentication">Whether Windows authentication is enabled (true) or not (false).</param>
        public WebAuthenticationBrokerBrowser(bool enableWindowsAuthentication = false)
        {
            _enableWindowsAuthentication = enableWindowsAuthentication;
        }

        /// <inheritdoc />
        public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(options.StartUrl))
                throw new ArgumentException("Missing StartUrl", nameof(options));

            var startUri = new Uri(options.StartUrl);
            if (startUri.AbsolutePath.StartsWith("/v2/logout", StringComparison.OrdinalIgnoreCase))
                return await InvokeLogoutAsync(startUri);

            try
            {
                var authOptions = ConfigureWebAuthOptions(options.DisplayMode);
                var authResult = await WebAuthenticationBroker.AuthenticateAsync(authOptions, startUri, new Uri(options.EndUrl));
                return CreateBrowserResult(authResult);
            }
            catch (Exception ex)
            {
                return new BrowserResult
                {
                    ResultType = BrowserResultType.UnknownError,
                    Error = ex.ToString()
                };
            }
        }

        private async Task<BrowserResult> InvokeLogoutAsync(Uri logoutUri)
        {
            try
            {
                await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.SilentMode, logoutUri);
            }
            catch
            {
            }

            return new BrowserResult
            {
                ResultType = BrowserResultType.Success,
                Response = String.Empty
            };
        }

        private static BrowserResult CreateBrowserResult(WebAuthenticationResult authResult)
        {
            switch (authResult.ResponseStatus)
            {
                case WebAuthenticationStatus.Success:
                    return new BrowserResult
                    {
                        ResultType = BrowserResultType.Success,
                        // Windows IoT Core adds a \0 char at the end of the ResponseData
                        // when doing response_mode=form_post, so we remove it here
                        // to avoid breaking the response processor.
                        Response = authResult.ResponseData?.Replace("\0", string.Empty)
                    };

                case WebAuthenticationStatus.ErrorHttp:
                    return new BrowserResult
                    {
                        ResultType = BrowserResultType.HttpError,
                        Error = authResult.ResponseErrorDetail.ToString()
                    };

                case WebAuthenticationStatus.UserCancel:
                    return new BrowserResult
                    {
                        ResultType = BrowserResultType.UserCancel
                    };

                default:
                    return new BrowserResult
                    {
                        ResultType = BrowserResultType.UnknownError,
                        Error = "Invalid response from WebAuthenticationBroker"
                    };
            }
        }

        private WebAuthenticationOptions ConfigureWebAuthOptions(DisplayMode mode)
        {
            var options = WebAuthenticationOptions.None;

            if (_enableWindowsAuthentication)
                options |= WebAuthenticationOptions.UseCorporateNetwork;

            switch (mode)
            {
                case DisplayMode.Visible:
                    return options;

                case DisplayMode.Hidden:
                    return options | WebAuthenticationOptions.SilentMode;

                default:
                    throw new ArgumentException("Invalid DisplayMode", nameof(options));
            }
        }
    }
}
