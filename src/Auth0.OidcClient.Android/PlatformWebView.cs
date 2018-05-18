using Android.App;
using Android.Content;
using IdentityModel.OidcClient.Browser;
using System;
using System.Threading.Tasks;

namespace Auth0.OidcClient
{

    class PlatformWebView : IBrowser
    {
        public Task<BrowserResult> InvokeAsync(BrowserOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.StartUrl))
            {
                throw new ArgumentException("Missing StartUrl", nameof(options));
            }

            if (string.IsNullOrWhiteSpace(options.EndUrl))
            {
                throw new ArgumentException("Missing EndUrl", nameof(options));
            }

            // must be able to wait for the intent to be triggered to continue
            // with setting the task result
            var tcs = new TaskCompletionSource<BrowserResult>();

            void Callback(string response)
            {
                // remove handler
                ActivityMediator.Instance.ActivityMessageReceived -= Callback;

                // set result
                if (response == "UserCancel")
                {
                    tcs.SetResult(new BrowserResult { ResultType = BrowserResultType.UserCancel });
                }
                else
                {
                    tcs.SetResult(new BrowserResult
                    {
                        Response = response,
                        ResultType = BrowserResultType.Success
                    });
                }
            }

            // attach handler
            ActivityMediator.Instance.ActivityMessageReceived += Callback;

            // Launch browser
            var uri = Android.Net.Uri.Parse(options.StartUrl);
            var intent = new Intent(Intent.ActionView, uri);
            intent.AddFlags(ActivityFlags.NoHistory);
            Application.Context.StartActivity(intent);

            // Return task which will be completed when intent is triggered
            return tcs.Task;
        }
    }
}