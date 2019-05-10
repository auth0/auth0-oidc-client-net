using IdentityModel.OidcClient.Browser;
using System;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace Auth0.OidcClient
{
    public class PlatformWebView : IBrowser
    {
        private readonly bool _enableWindowsAuthentication;

        public PlatformWebView(bool enableWindowsAuthentication = false)
        {
            _enableWindowsAuthentication = enableWindowsAuthentication;
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

        private async Task<BrowserResult> InvokeAsyncCore(BrowserOptions options, bool silentMode)
        {
            var startUri = new Uri(options.StartUrl);
            if (startUri.AbsolutePath.StartsWith("/v2/logout", StringComparison.OrdinalIgnoreCase))
                return await InvokeLogoutAsync(startUri);

            WebAuthenticationOptions webAuthOptions = ConfigureWebAuthOptions(silentMode);

            WebAuthenticationResult wabResult = null;

            try
            {
                if (string.Equals(options.EndUrl, WebAuthenticationBroker.GetCurrentApplicationCallbackUri().AbsoluteUri, StringComparison.Ordinal))
                {
                    wabResult = await WebAuthenticationBroker.AuthenticateAsync(webAuthOptions, new Uri(options.StartUrl));
                }
                else
                {
                    wabResult = await WebAuthenticationBroker.AuthenticateAsync(webAuthOptions, new Uri(options.StartUrl), new Uri(options.EndUrl));
                }
            }
            catch (Exception ex)
            {
                return new BrowserResult
                {
                    ResultType = BrowserResultType.UnknownError,
                    Error = ex.ToString()
                };
            }

            switch (wabResult.ResponseStatus)
            {
                case WebAuthenticationStatus.Success:
                    return new BrowserResult
                    {
                        ResultType = BrowserResultType.Success,
                        // Windows IoT Core adds a \0 char at the end of the ResponseData
                        // when doing response_mode=form_post, so we remove it here
                        // to avoid breaking the response processor.
                        Response = wabResult.ResponseData?.Replace("\0", string.Empty)
                    };

                case WebAuthenticationStatus.ErrorHttp:
                    return new BrowserResult
                    {
                        ResultType = BrowserResultType.HttpError,
                        Error = wabResult.ResponseErrorDetail.ToString()
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

        private WebAuthenticationOptions ConfigureWebAuthOptions(bool silentMode)
        {
            var wabOptions = WebAuthenticationOptions.None;

            if (_enableWindowsAuthentication)
                wabOptions |= WebAuthenticationOptions.UseCorporateNetwork;
            if (silentMode)
                wabOptions |= WebAuthenticationOptions.SilentMode;

            return wabOptions;
        }

        public async Task<BrowserResult> InvokeAsync(BrowserOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.StartUrl)) throw new ArgumentException("Missing StartUrl", nameof(options));
            if (string.IsNullOrWhiteSpace(options.EndUrl)) throw new ArgumentException("Missing EndUrl", nameof(options));

            switch (options.DisplayMode)
            {
                case DisplayMode.Visible:
                    return await InvokeAsyncCore(options, false);

                case DisplayMode.Hidden:
                    var result = await InvokeAsyncCore(options, true);
                    if (result.ResultType == BrowserResultType.Success)
                    {
                        return result;
                    }
                    else
                    {
                        result.ResultType = BrowserResultType.Timeout;
                        return result;
                    }
            }

            throw new ArgumentException("Invalid DisplayMode", nameof(options));
        }
    }
}
