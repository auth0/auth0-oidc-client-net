using IdentityModel.OidcClient.Browser;
using System;
using System.Threading.Tasks;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Provides common <see cref="IBrowser"/> logic for iOS platform.
    /// </summary>
    public abstract class IOSBrowserBase : IBrowser
    {
        public Task<BrowserResult> InvokeAsync(BrowserOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.StartUrl))
                throw new ArgumentException("Missing StartUrl", nameof(options));

            if (string.IsNullOrWhiteSpace(options.EndUrl))
                throw new ArgumentException("Missing EndUrl", nameof(options));

            return Launch(options);
        }

        protected abstract Task<BrowserResult> Launch(BrowserOptions options);

        protected static BrowserResult Canceled()
        {
            return new BrowserResult
            {
                ResultType = BrowserResultType.UserCancel
            };
        }

        protected static BrowserResult UnknownError(string error)
        {
            return new BrowserResult
            {
                ResultType = BrowserResultType.UnknownError,
                Error = error
            };
        }

        protected static BrowserResult Success(string response)
        {
            return new BrowserResult
            {
                Response = response,
                ResultType = BrowserResultType.Success
            };
        }
    }
}