using IdentityModel.OidcClient.Browser;
using System;
using System.Threading.Tasks;

namespace Auth0.OidcClient
{
    public abstract class AndroidWebViewBase : IBrowser
    {
        public Task<BrowserResult> InvokeAsync(BrowserOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.StartUrl))
                throw new ArgumentException("Missing StartUrl", nameof(options));

            if (string.IsNullOrWhiteSpace(options.EndUrl))
                throw new ArgumentException("Missing EndUrl", nameof(options));

            var tcs = new TaskCompletionSource<BrowserResult>();

            void Callback(string response)
            {
                ActivityMediator.Instance.ActivityMessageReceived -= Callback;

                var cancelled = response == "UserCancel";
                tcs.SetResult(new BrowserResult
                {
                    ResultType = cancelled ? BrowserResultType.UserCancel : BrowserResultType.Success,
                    Response = response
                });
            }

            ActivityMediator.Instance.ActivityMessageReceived += Callback;

            LaunchBrowser(Android.Net.Uri.Parse(options.StartUrl));

            return tcs.Task;
        }

        protected abstract void LaunchBrowser(Android.Net.Uri uri);
    }
}