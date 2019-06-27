using IdentityModel.OidcClient.Browser;
using Microsoft.Toolkit.Wpf.UI.Controls;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Implements the <see cref="IBrowser"/> interface using the <see cref="WebViewCompatible"/> control.
    /// </summary>
    public class WebViewBrowser : IBrowser
    {
        private readonly Func<Window> _windowFactory;
        private readonly bool _shouldCloseWindow;

        /// <summary>
        /// Create a new instance of <see cref="WebViewBrowser"/> with a custom windowFactory and optional window close flag.
        /// </summary>
        /// <param name="windowFactory">A function that returns a <see cref="Window"/> to be used for hosting the browser.</param>
        /// <param name="shouldCloseWindow"> Whether the Window should be closed or not after completion.</param>
        public WebViewBrowser(Func<Window> windowFactory, bool shouldCloseWindow = true)
        {
            _windowFactory = windowFactory;
            _shouldCloseWindow = shouldCloseWindow;
        }

        /// <summary>
        /// Create a new instance of <see cref="WebViewBrowser"/> allowing parts of the <see cref="Window"/> container to be set.
        /// </summary>
        /// <param name="title">Optional title for the form - defaults to 'Authenticating...'.</param>
        /// <param name="width">Optional width for the form in pixels. Defaults to 1024.</param>
        /// <param name="height">Optional height for the form in pixels. Defaults to 768.</param>
        public WebViewBrowser(string title = "Authenticating...", int width = 1024, int height = 768)
            : this(() => new Window
            {
                Name = "WebAuthentication",
                Title = title,
                Width = width,
                Height = height
            })
        {
        }

        /// <inheritdoc />
        public Task<BrowserResult> InvokeAsync(BrowserOptions options)
        {
            var tcs = new TaskCompletionSource<BrowserResult>();

            var window = _windowFactory();
            var webView = new WebViewCompatible();
            window.Content = webView;

            webView.NavigationStarting += (sender, e) =>
            {
                if (e.Uri.AbsoluteUri.StartsWith(options.EndUrl))
                {
                    tcs.SetResult(new BrowserResult { ResultType = BrowserResultType.Success, Response = e.Uri.ToString() });
                    if (_shouldCloseWindow)
                        window.Close();
                    else
                        window.Content = null;
                }
            };

            window.Closing += (sender, e) =>
            {
                if (!tcs.Task.IsCompleted)
                    tcs.SetResult(new BrowserResult { ResultType = BrowserResultType.UserCancel });
            };

            window.Show();
            webView.Navigate(options.StartUrl);

            return tcs.Task;
        }
    }
}
