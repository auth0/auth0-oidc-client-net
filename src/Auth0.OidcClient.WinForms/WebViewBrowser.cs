using IdentityModel.OidcClient.Browser;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Implements the <see cref="IBrowser"/> interface using the <see cref="WebViewCompatible"/> control.
    /// </summary>
    public class WebViewBrowser : IBrowser
    {
        private readonly Func<Form> _formFactory;

        /// <summary>
        /// Creates a new instance of <see cref="WebViewBrowser"/> with a specified function to create the <see cref="Form"/>
        /// used to host the <see cref="WebViewCompatible"/> control.
        /// </summary>
        /// <param name="formFactory">The function used to create the <see cref="Form"/> that will host the <see cref="WebViewCompatible"/> control.</param>
        public WebViewBrowser(Func<Form> formFactory)
        {
            _formFactory = formFactory;
        }

        /// <summary>
        /// Creates a new instance of <see cref="WebViewBrowser"/> allowing parts of the <see cref="Form"/> container to be set.
        /// </summary>
        /// <param name="title">Optional title for the form - defaults to 'Authenticating...'.</param>
        /// <param name="width">Optional width for the form in pixels. Defaults to 1024.</param>
        /// <param name="height">Optional height for the form in pixels. Defaults to 768.</param>
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

        /// <inheritdoc />
        public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<BrowserResult>();

            var window = _formFactory();
            #pragma warning disable 618
            var webView = new WebView2 { Dock = DockStyle.Fill };

            webView.NavigationStarting += (sender, e) =>
            {
                if (e.Uri.StartsWith(options.EndUrl))
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

            await webView.EnsureCoreWebView2Async();
            webView.CoreWebView2.Navigate(options.StartUrl);

            return await tcs.Task;
        }
    }
}
