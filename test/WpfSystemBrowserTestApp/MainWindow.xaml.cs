using System.Diagnostics;
using System.Windows;
using Auth0.OidcClient;

namespace WpfSystemBrowserTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            var client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = "jerrie.auth0.com",
                ClientId = "vV9twaySQzfGesS9Qs6gOgqDsYDdgoKE",
                Scope = "openid profile offline_access",
                Browser = new SystemWebBrowser(),
                RedirectUri = "http://127.0.0.1:7890/"
            });

            var loginResult = await client.LoginAsync();

            if (loginResult.IsError)
            {
                Debug.WriteLine($"An error occurred during login: {loginResult.Error}");
            }
            else
            {
                Debug.WriteLine($"id_token: {loginResult.IdentityToken}");
                Debug.WriteLine($"access_token: {loginResult.AccessToken}");
                Debug.WriteLine($"refresh_token: {loginResult.RefreshToken}");

                Debug.WriteLine($"name: {loginResult.User.FindFirst(c => c.Type == "name")?.Value}");
                Debug.WriteLine($"email: {loginResult.User.FindFirst(c => c.Type == "email")?.Value}");

                foreach (var claim in loginResult.User.Claims)
                {
                    Debug.WriteLine($"{claim.Type} = {claim.Value}");
                }
            }
        }

    }
}
