using System.Diagnostics;
using System.Windows;
using Auth0.OidcClient;

namespace WpfTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Auth0Client _auth0Client;

        public MainWindow()
        {
            InitializeComponent();

            _auth0Client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = "auth0-dotnet-integration-tests.auth0.com",
                ClientId = "qmss9A66stPWTOXjR6X1OeA0DLadoNP2",
                Scope = "openid profile email"
            });
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            var loginResult = await _auth0Client.LoginAsync();

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

        private async void LogoutButton_OnClick(object sender, RoutedEventArgs e)
        {
            await _auth0Client.LogoutAsync();
        }
    }
}
