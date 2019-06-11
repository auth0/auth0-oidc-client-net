using IdentityModel.OidcClient.Browser;
using Microsoft.Toolkit.Forms.UI.Controls;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Auth0.OidcClient
{
    public class WebViewBrowser : IBrowser
    {
        private readonly Func<Form> _formFactory;

        public WebViewBrowser(Func<Form> formFactory)
        {
            _formFactory = formFactory;
        }

        public WebViewBrowser(string title = "Authenticating...", int width = 1024, int height = 768)
            : this(() => new Form
            {
                Name = "WebAuthentication",
                Text = title,
                Width = width,
                Height = height
            })
        {
        }

        public Task<BrowserResult> InvokeAsync(BrowserOptions options)
        {
            var tcs = new TaskCompletionSource<BrowserResult>();

            var window = _formFactory();
            var webView = new WebViewCompatible { Dock = DockStyle.Fill };

            webView.NavigationStarting += (sender, e) =>
            {
                if (e.Uri.AbsoluteUri.StartsWith(options.EndUrl))
                {
                    tcs.SetResult(new BrowserResult { ResultType = BrowserResultType.Success, Response = e.Uri.ToString() });
                    window.Close();
                }
            };

            window.Closing += (sender, e) =>
            {
                if (!tcs.Task.IsCompleted)
                    tcs.SetResult(new BrowserResult { ResultType = BrowserResultType.UserCancel });
            };


            window.Controls.Add(webView);
            window.Show();
            webView.Navigate(options.StartUrl);

            return tcs.Task;
        }
    }
}
