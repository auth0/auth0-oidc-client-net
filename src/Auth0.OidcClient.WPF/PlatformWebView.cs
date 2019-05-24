using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using IdentityModel.OidcClient.Browser;

namespace Auth0.OidcClient
{
    public class PlatformWebView : IBrowser
    {
        private readonly Func<Window> _windowFactory;
        private readonly bool _shouldCloseWindow;

        /// <summary>
        /// Constructor which lets you pass in your WPF window and window closing choice.
        /// </summary>
        /// <param name="windowFactory"> </param>
        /// <param name="shouldCloseWindow"> Determines whether the window closes or not after sucessful login</param>
        /// <example> 
        /// This sample shows how to call the <see cref="PlatformWebView(Func&lt;Window&gt;, bool)"/> constructor.
        /// <code>
        /// Window ReturnWindow()
        /// {
        ///     return window; // your WPF application window where you want the login to pop up
        /// }
        /// Func&lt;Window&gt; windowFunc = ReturnWindow;
        /// PlatformWebView platformWebView = new PlatformWebView(windowFunc, shouldCloseWindow: false); // specify false if you want the window to remain open
        /// </code>
        /// </example>
        public PlatformWebView(Func<Window> windowFactory, bool shouldCloseWindow = true)
        {
            _windowFactory = windowFactory;
            _shouldCloseWindow = shouldCloseWindow;
        }

        public PlatformWebView(string title = "Authenticating ...", int width = 1024, int height = 768)
            : this(() => new Window
            {
                Name = "WebAuthentication",
                Title = title,
                Width = width,
                Height = height
            })
        {
        }

        public async Task<BrowserResult> InvokeAsync(BrowserOptions options)
        {
            var window = _windowFactory.Invoke();
            var result = new BrowserResult
            {
                ResultType = BrowserResultType.UserCancel
            };

            try
            {
                var browser = new WebBrowser();
                var signal = new SemaphoreSlim(0, 1);

                window.Closed += (o, e) =>
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
