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

        public PlatformWebView(Func<Window> windowFactory, bool shouldCloseWindow = true)
        {
            _windowFactory = windowFactory;
            _shouldCloseWindow = shouldCloseWindow;
        }

        public PlatformWebView(string title = "Authenticating ...", int width = 1024, int height = 768, bool shouldCloseWindow = true)
            : this(() => new Window
            {
                Name = "WebAuthentication",
                Title = title,
                Width = width,
                Height = height
            })
        {
            _shouldCloseWindow = shouldCloseWindow;
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
