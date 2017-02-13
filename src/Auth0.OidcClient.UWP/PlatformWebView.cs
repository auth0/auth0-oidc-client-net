using System;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using IdentityModel.OidcClient.WebView;

namespace Auth0.OidcClient
{
    public class PlatformWebView : IWebView
    {
        private readonly bool _enableWindowsAuthentication;

        public event EventHandler<HiddenModeFailedEventArgs> HiddenModeFailed;

        public PlatformWebView(bool enableWindowsAuthentication = false)
        {
            _enableWindowsAuthentication = enableWindowsAuthentication;
        }

        private async Task<InvokeResult> InvokeAsyncCore(InvokeOptions options, bool silentMode)
        {
            var wabOptions = WebAuthenticationOptions.None;

            if (options.ResponseMode == ResponseMode.FormPost)
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

            WebAuthenticationResult wabResult;

            try
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
            catch (Exception ex)
            {
                return new InvokeResult
                {
                    ResultType = InvokeResultType.UnknownError,
                    Error = ex.ToString()
                };
            }

            if (wabResult.ResponseStatus == WebAuthenticationStatus.Success)
            {
                return new InvokeResult
                {
                    ResultType = InvokeResultType.Success,
                    Response = wabResult.ResponseData
                };
            }
            else if (wabResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            {
                return new InvokeResult
                {
                    ResultType = InvokeResultType.HttpError,
                    Error = string.Concat(wabResult.ResponseErrorDetail.ToString())
                };
            }
            else if (wabResult.ResponseStatus == WebAuthenticationStatus.UserCancel)
            {
                return new InvokeResult
                {
                    ResultType = InvokeResultType.UserCancel
                };
            }
            else
            {
                return new InvokeResult
                {
                    ResultType = InvokeResultType.UnknownError,
                    Error = "Invalid response from WebAuthenticationBroker"
                };
            }
        }

        public async Task<InvokeResult> InvokeAsync(InvokeOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.StartUrl)) throw new ArgumentException("Missing StartUrl", nameof(options));
            if (string.IsNullOrWhiteSpace(options.EndUrl)) throw new ArgumentException("Missing EndUrl", nameof(options));

            switch (options.InitialDisplayMode)
            {
                case DisplayMode.Visible:
                    return await InvokeAsyncCore(options, false);

                case DisplayMode.Hidden:
                    var result = await InvokeAsyncCore(options, true);
                    if (result.ResultType != InvokeResultType.Success)
                    {
                        var args = new HiddenModeFailedEventArgs(result);
                        HiddenModeFailed?.Invoke(this, args);
                        if (!args.Cancel)
                        {
                            result = await InvokeAsyncCore(options, false);
                        }
                    }
                    return result;
            }

            throw new ArgumentException("Invalid DisplayMode", nameof(options));
        }

        public Task StartInvokeAsync(InvokeOptions options)
        {
            throw new NotImplementedException();
        }
    }
}