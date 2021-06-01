using Android.App;
using Android.Content;
using IdentityModel.OidcClient.Browser;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Provides common IBrowser logic for Android.
    /// </summary>
    public abstract class AndroidBrowserBase : IBrowser
    {
        /// <summary>
        /// The <see cref="Context"/> associated with this browser.
        /// </summary>
        protected Context context;

        /// <summary>
        /// Whether this browser should launch a new Android Task.
        /// </summary>
        protected readonly bool IsNewTask;

        /// <summary>
        /// Whether this browser should close itself when returning to the Android task. Default is <b>false</b>, keeping the browser open in the background when returning to the main application.
        /// </summary>
        public bool AutoCloseBrowser { get; set; }

        /// <summary>
        /// Default constructor for <see cref="AndroidBrowserBase"/> that provides assignment
        /// of context and IsNewTask when called by subclasses.
        /// </summary>
        /// <param name="context">Optional <see cref="Context"/> to provide on subsequent callbacks.</param>
        /// <param name="autoCloseBrowser">Optional parameter to provide the <see cref="Activity"/> to close the browser after use.</param>
        protected AndroidBrowserBase(Context context = null, bool autoCloseBrowser = false)
        {
            this.context = context;
            IsNewTask = context == null;

            this.AutoCloseBrowser = autoCloseBrowser;
        }

        /// <summary>
        /// Provide the optional constructor to 1) provide no context and 2) set the <param name="autoCloseBrowser"> property.</param>
        /// </summary>
        /// <param name="autoCloseBrowser">Whether to keep the browser window open or to close it after end use.</param>
        protected AndroidBrowserBase(bool autoCloseBrowser) : this(null, autoCloseBrowser) { }

        /// <inheritdoc/>
        public Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
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

            OpenBrowser(Android.Net.Uri.Parse(options.StartUrl), context ?? Application.Context);

            return tcs.Task;
        }

        /// <summary>
        /// Open a web browser with the given uri.
        /// </summary>
        /// <param name="uri"><see cref="Uri"/> address to open in the browser.</param>
        /// <param name="context">Optional <see cref="Context"/> associated with the browser.</param>
        protected abstract void OpenBrowser(Android.Net.Uri uri, Context context = null);
    }
}