using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using IdentityModel.OidcClient.Browser;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Implements the <see cref="IBrowser"/> interface using <see cref="WebBrowser"/>.
    /// </summary>
    public class WebBrowserBrowser : IBrowser
    {
        private readonly Func<Window> _windowFactory;
        private readonly bool _shouldCloseWindow;

        /// <summary>
        /// Create a new instance of <see cref="WebBrowserBrowser"/> with a custom windowFactory and optional window close flag.
        /// </summary>
        /// <param name="windowFactory">A function that returns a <see cref="Window"/> to be used for hosting the browser.</param>
        /// <param name="shouldCloseWindow"> Whether the Window should be closed or not after completion.</param>
        /// <example> 
        /// This sample shows how to call the <see cref="WebBrowserBrowser(Func&lt;Window&gt;, bool)"/> constructor.
        /// <code>
        /// Window ReturnWindow()
        /// {
        ///     return window; // your WPF application window where you want the login to pop up
        /// }
        /// var browser = new WebBrowserBrowser(ReturnWindow, shouldCloseWindow: false); // specify false if you want the window to remain open
        /// </code>
        /// </example>
        public WebBrowserBrowser(Func<Window> windowFactory, bool shouldCloseWindow = true)
        {
            _windowFactory = windowFactory;
            _shouldCloseWindow = shouldCloseWindow;
        }

        /// <summary>
        /// Create a new instance of <see cref="WebBrowserBrowser"/> allowing parts of the <see cref="Window"/> container to be set.
        /// </summary>
        /// <param name="title">Optional title for the form - defaults to 'Authenticating...'.</param>
        /// <param name="width">Optional width for the form in pixels. Defaults to 1024.</param>
        /// <param name="height">Optional height for the form in pixels. Defaults to 768.</param>
        public WebBrowserBrowser(string title = "Authenticating...", int width = 1024, int height = 768)
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
        public async Task<BrowserResult> InvokeAsync(BrowserOptions options)
        {
            var window = _windowFactory.Invoke();
            using (var browser = new WebBrowser())
            {
                var signal = new SemaphoreSlim(0, 1);

                var result = new BrowserResult
                {
                    ResultType = BrowserResultType.UserCancel
                };

                window.Closed += (s, e) =>
                {
                    signal.Release();
                };

                browser.LoadCompleted += (s, e) =>
                {
                    if (e.Uri.ToString().StartsWith(options.EndUrl))
                    {
                        result.ResultType = BrowserResultType.Success;
                        result.Response = e.Uri.ToString();
                        signal.Release();
                    }
                };

                try
                {
                    window.Content = browser;
                    window.Show();

                    browser.Navigate(options.StartUrl);

                    await signal.WaitAsync();
                }
                finally
                {
                    if (_shouldCloseWindow)
                        window.Close();
                    else
                        window.Content = null;
                }

                return result;
            }
        }
    }
}
