using System;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;

namespace Auth0.OidcClient
{
    public class PlatformWebView : IBrowser
    {
        private readonly bool _enableWindowsAuthentication;

        public PlatformWebView(bool enableWindowsAuthentication = false)
        {
            _enableWindowsAuthentication = enableWindowsAuthentication;
        }

        private async Task<BrowserResult> InvokeAsyncCore(BrowserOptions options, bool silentMode)
        {
            bool isLogout = false;
            var wabOptions = WebAuthenticationOptions.None;

            if (options.ResponseMode == OidcClientOptions.AuthorizeResponseMode.FormPost)
            {
                wabOptions |= WebAuthenticationOptions.UseHttpPost;
            }
            if (_enableWindowsAuthentication)
            {
                wabOptions |= WebAuthenticationOptions.UseCorporateNetwork;
            }
            if (silentMode)
            {
                wabOptions |= WebAuthenticationOptions.SilentMode;
            }

            WebAuthenticationResult wabResult = null;

            try
            {
                // Check for logout
                Uri startUri = new Uri(options.StartUrl);
                if (startUri.AbsolutePath.StartsWith("/v2/logout", StringComparison.OrdinalIgnoreCase))
                {
                    isLogout = true;

                    // See http://www.cloudidentity.com/blog/2014/11/21/getting-rid-of-residual-cookies-in-windows-store-apps/
                    try
                    {
                        await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.SilentMode, new Uri(options.StartUrl));
                    }
                    catch (Exception)
                    {
                        // timeout. That's expected
                    }
                }
                else
                {
                    if (string.Equals(options.EndUrl, WebAuthenticationBroker.GetCurrentApplicationCallbackUri().AbsoluteUri, StringComparison.Ordinal))
                    {
                        wabResult = await WebAuthenticationBroker.AuthenticateAsync(
                            wabOptions, new Uri(options.StartUrl));
                    }
                    else
                    {
                        wabResult = await WebAuthenticationBroker.AuthenticateAsync(
                            wabOptions, new Uri(options.StartUrl), new Uri(options.EndUrl));
                    }
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

            if (wabResult == null ) 
            {
                if (isLogout)
                {
                    return new BrowserResult
                    {
                        ResultType = BrowserResultType.Success,
                        Response = String.Empty
                    };
                }

                return new BrowserResult
                {
                    ResultType = BrowserResultType.UnknownError,
                    Error = "Invalid response from WebAuthenticationBroker"
                };
            }

            if (wabResult.ResponseStatus == WebAuthenticationStatus.Success)
            {
                return new BrowserResult
                {
                    ResultType = BrowserResultType.Success,
                    
                    // Windows IoT Core adds a \0 char at the end of the ResponseData
                    // when doing response_mode=form_post, so we remove it here
                    // to avoid breaking the response processor.
                    Response = wabResult.ResponseData?.Replace("\0", string.Empty)
                };
            }

            if (wabResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            {
                return new BrowserResult
                {
                    ResultType = BrowserResultType.HttpError,
                    Error = wabResult.ResponseErrorDetail.ToString()
                };
            }

            if (wabResult.ResponseStatus == WebAuthenticationStatus.UserCancel)
            {
                return new BrowserResult
                {
                    ResultType = BrowserResultType.UserCancel
                };
            }

            return new BrowserResult
            {
                ResultType = BrowserResultType.UnknownError,
                Error = "Invalid response from WebAuthenticationBroker"
            };
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
