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
        /// Constructor which lets you pass in your WPF window and window closing property
        /// </summary>
        /// <param name="windowFactory"> </param>
        /// <param name="shouldCloseWindow"> Determines whether the window closes or not after sucessful login</param>
        
        /// Example Usage:
        /// Window ReturnWindow()
        /// {
        ///     return window; // your WPF applciation window where you want the login to pop up
        /// }
        /// Func<Window> windowFunc = ReturnWindow;
        /// PlatformWebView platformWebView = new PlatformWebView(windowFunc, shouldCloseWindow: false); // specify false if you want the window to remain open
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
            _shouldCloseWindow = true;
        }

        public async Task<BrowserResult> InvokeAsync(BrowserOptions options)
        {
            var window = _windowFactory.Invoke();

            try
            {
                var grid = new Grid();

                window.Content = grid;
                var browser = new WebBrowser();

                var signal = new SemaphoreSlim(0, 1);

                var result = new BrowserResult
                {
                    ResultType = BrowserResultType.UserCancel
                };

                window.Closed += (o, e) =>
                {
                    signal.Release();
                };
                browser.LoadCompleted += (sender, args) =>
                {
                    if (args.Uri.ToString().StartsWith(options.EndUrl))
                    {
                        result.ResultType = BrowserResultType.Success;

                        result.Response = args.Uri.ToString();

                        signal.Release();
                    }
                };

                grid.Children.Add(browser);
                window.Show();

                browser.Navigate(options.StartUrl);

                await signal.WaitAsync();

                if (!_shouldCloseWindow)
                {
                    grid.Children.Clear();
                }

                return result;
            }
            finally
            {
                if (_shouldCloseWindow)
                {
                    window.Close();
                }
            }
        }
    }
}
