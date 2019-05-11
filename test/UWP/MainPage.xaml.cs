using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Text;
using Auth0.OidcClient;

namespace UWPTestApp
{
    public sealed partial class MainPage : Page
    {
        private readonly Auth0Client _auth0Client;

        public MainPage()
        {
            InitializeComponent();

            _auth0Client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = "auth0-dotnet-integration-tests.auth0.com",
                ClientId = "qmss9A66stPWTOXjR6X1OeA0DLadoNP2"
            });
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            resultTextBox.Text = "";

            var loginResult = await _auth0Client.LoginAsync();

            // Display error
            if (loginResult.IsError)
            {
                resultTextBox.Text = loginResult.Error;
                return;
            }

            // Display result
            var sb = new StringBuilder();

            sb.AppendLine("Tokens");
            sb.AppendLine("------");
            sb.AppendLine($"id_token: {loginResult.IdentityToken}");
            sb.AppendLine($"access_token: {loginResult.AccessToken}");
            sb.AppendLine($"refresh_token: {loginResult.RefreshToken}");
            sb.AppendLine();

            sb.AppendLine("Claims");
            sb.AppendLine("------");
            foreach (var claim in loginResult.User.Claims)
                sb.AppendLine($"{claim.Type}: {claim.Value}");

            resultTextBox.Text = sb.ToString();
        }

        private async void LogoutButton_OnClick(object sender, RoutedEventArgs e)
        {
            resultTextBox.Text = "Logging out...";
            var result = await _auth0Client.LogoutAsync();
            resultTextBox.Text += "\n" + result.ToString();
        }
    }
}
