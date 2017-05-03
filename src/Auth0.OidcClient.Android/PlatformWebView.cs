using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.CustomTabs;
using IdentityModel.OidcClient.Browser;

namespace Auth0.OidcClient
{

    class PlatformWebView : IBrowser
    {
        private readonly Activity _context;
        private CustomTabsActivityManager _customTabs;

        public PlatformWebView(Activity context)
        {
            _context = context;
        }

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

            // must be able to wait for the intent to be finished to continue
            // with setting the task result
            var tcs = new TaskCompletionSource<BrowserResult>();

            // create & open chrome custom tab
            _customTabs = new CustomTabsActivityManager(_context);

            // build custom tab
            var builder = new CustomTabsIntent.Builder(_customTabs.Session)
               .SetToolbarColor(Color.Argb(255, 52, 152, 219))
               .SetShowTitle(true)
               .EnableUrlBarHiding();

            var customTabsIntent = builder.Build();

            // ensures the intent is not kept in the history stack, which makes
            // sure navigating away from it will close it
            customTabsIntent.Intent.AddFlags(ActivityFlags.NoHistory);

            ActivityMediator.MessageReceivedEventHandler callback = null;
            callback = (response) =>
            {
                // remove handler
                ActivityMediator.Instance.ActivityMessageReceived -= callback;

                // set result
                if (response == "UserCancel")
                {
                    tcs.SetResult(new BrowserResult
                    {
                        ResultType = BrowserResultType.UserCancel
                    });
                }
                else
                {
                    tcs.SetResult(new BrowserResult
                    {
                        Response = response,
                        ResultType = BrowserResultType.Success
                    });
                }
            };

            // attach handler
            ActivityMediator.Instance.ActivityMessageReceived += callback;

            // launch
            customTabsIntent.LaunchUrl(_context, Android.Net.Uri.Parse(options.StartUrl));

            // need an intent to be triggered when browsing to the "io.identitymodel.native://callback"
            // scheme/URI => CallbackInterceptorActivity
            return tcs.Task;
        }
    }
}