using IdentityModel.OidcClient.Browser;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Implements the <see cref="IBrowser"/> interface using the <see cref="WebView"/> control.
    /// </summary>
    public class WebViewBrowser : IBrowser
    {
        /// <inheritdoc />
        public Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<BrowserResult>();
            var currentAppView = ApplicationView.GetForCurrentView();

            RunOnNewView(async () => {
                var newAppView = CreateApplicationView();
                var webView = CreateWebView(Window.Current, options, tcs);
                webView.Navigate(new Uri(options.StartUrl));
                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newAppView.Id, ViewSizePreference.UseMinimum, currentAppView.Id, ViewSizePreference.UseMinimum);
            });

            return tcs.Task;
        }

        private async void RunOnNewView(DispatchedHandler function)
        {
            await CoreApplication.CreateNewView().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, function);
        }

        private static ApplicationView CreateApplicationView()
        {
            var appView = ApplicationView.GetForCurrentView();
            appView.Title = "Authentication...";
            return appView;
        }

        private static WebView CreateWebView(Window window, BrowserOptions options, TaskCompletionSource<BrowserResult> tcs)
        {
            var webView = new WebView();

            webView.NavigationStarting += (sender, e) =>
            {
                if (e.Uri.AbsoluteUri.StartsWith(options.EndUrl))
                {
                    tcs.SetResult(new BrowserResult { ResultType = BrowserResultType.Success, Response = e.Uri.ToString() });
                    window.Close();
                }
            };

            // There is no closed event so the best we can do is detect visibility. This means we close when they minimize too.
            window.VisibilityChanged += (sender, e) =>
            {
                if (!window.Visible && !tcs.Task.IsCompleted)
                {
                    tcs.SetResult(new BrowserResult { ResultType = BrowserResultType.UserCancel });
                    window.Close();
                }
            };

            window.Content = webView;
            window.Activate();

            return webView;
        }
    }
}
