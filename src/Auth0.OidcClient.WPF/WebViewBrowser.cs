using IdentityModel.OidcClient.Browser;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Web.WebView2.Wpf;

namespace Auth0.OidcClient
{
    public class WebWindow : Window
    {
        private readonly WebView2 _webView = new();
        private string? _url;

        public WebWindow()
        {
            Content = _webView;
        }

        public Task<BrowserResult> Show(BrowserOptions options, bool shouldCloseWindow, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<BrowserResult>();
            _url = options.StartUrl;

            _webView.NavigationStarting += (sender, e) =>
            {
                if (!e.Uri.StartsWith(options.EndUrl)) return;

                tcs.SetResult(new BrowserResult { ResultType = BrowserResultType.Success, Response = e.Uri.ToString() });
                if (shouldCloseWindow)
                    Close();
                else
                    Content = null;
            };

            Closing += (sender, e) =>
            {
                if (!tcs.Task.IsCompleted)
                    tcs.SetResult(new BrowserResult { ResultType = BrowserResultType.UserCancel });
            };

            Show();
            return tcs.Task;
        }

        protected override async void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            try
            {
                await _webView.EnsureCoreWebView2Async();
                _webView.CoreWebView2.Navigate(_url);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception.Message);
            }
        }
    }

    /// <summary>
    /// Implements the <see cref="IBrowser"/> interface using the <see cref="WebViewCompatible"/> control.
    /// </summary>
    public class WebViewBrowser : IBrowser
    {
        private readonly Func<WebWindow> _windowFactory;
        private readonly bool _shouldCloseWindow;

        /// <summary>
        /// Create a new instance of <see cref="WebViewBrowser"/> with a custom windowFactory and optional window close flag.
        /// </summary>
        /// <param name="windowFactory">A function that returns a <see cref="WebWindow"/> to be used for hosting the browser.</param>
        /// <param name="shouldCloseWindow"> Whether the WebWindow should be closed or not after completion.</param>
        public WebViewBrowser(Func<WebWindow> windowFactory, bool shouldCloseWindow = true)
        {
            _windowFactory = windowFactory;
            _shouldCloseWindow = shouldCloseWindow;
        }

        /// <summary>
        /// Create a new instance of <see cref="WebViewBrowser"/> allowing parts of the <see cref="WebWindow"/> container to be set.
        /// </summary>
        /// <param name="title">Optional title for the form - defaults to 'Authenticating...'.</param>
        /// <param name="width">Optional width for the form in pixels. Defaults to 1024.</param>
        /// <param name="height">Optional height for the form in pixels. Defaults to 768.</param>
        public WebViewBrowser(string title = "Authenticating...", int width = 1024, int height = 768)
            : this(() => new WebWindow
            {
                Name = "WebAuthentication",
                Title = title,
                Width = width,
                Height = height
            })
        {
        }

        /// <inheritdoc />
        public Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            var window = _windowFactory();
            return window.Show(options, _shouldCloseWindow, cancellationToken);
        }
    }
}