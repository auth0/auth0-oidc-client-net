using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.OidcClient.Browser;

namespace WpfSystemBrowserTestApp
{
    internal class SystemWebBrowser : IBrowser
    {
        public async Task<BrowserResult> InvokeAsync(BrowserOptions options)
        {
            // create an HttpListener to listen for requests on the redirect URI.
            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add(options.EndUrl);
                listener.Start();

                // Start the browser
                Process.Start(options.StartUrl);

                // wait for the authorization response.
                var context = await listener.GetContextAsync();

                // sends an HTTP response to the browser.
                var response = context.Response;
                string responseString =
                    "<html><head><meta http-equiv=\'refresh\'></head><body>Authentication is completed. You can now close the browser and return to the application.</body></html>";
                var buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                var responseOutput = response.OutputStream;
                await responseOutput.WriteAsync(buffer, 0, buffer.Length);
                responseOutput.Close();

                // Return the result
                return new BrowserResult
                {
                    ResultType = BrowserResultType.Success,
                    Response = context.Request.Url.ToString()
                };
            }
        }
    }
}