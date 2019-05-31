using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Text;
using Auth0.OidcClient;

namespace UWPTestApp
{
    public sealed partial class MainPage : Page
    {
        private readonly Auth0Client _auth0Client;
        private string accessToken;

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
            resultTextBox.Text = "Logging in...";

            var loginResult = await _auth0Client.LoginAsync();

            // Display error
            if (loginResult.IsError)
            {
                resultTextBox.Text += "\n" + loginResult.Error;
                return;
            }

            accessToken = loginResult.AccessToken;

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

            resultTextBox.Text += "\n" + sb.ToString();
        }

        private async void LogoutButton_OnClick(object sender, RoutedEventArgs e)
        {
            resultTextBox.Text = "Logging out...";
            var result = await _auth0Client.LogoutAsync();
            accessToken = null;
            resultTextBox.Text += "\n" + result.ToString();
        }

        private async void UserInfoButton_Click(object sender, RoutedEventArgs e)
        {
            resultTextBox.Text = "Getting user info...";
            if (string.IsNullOrEmpty(accessToken))
            {
                resultTextBox.Text += "\n" + "You need to be logged in to get user info";
            }
            else
            {
                var userInfoResult = await _auth0Client.GetUserInfoAsync(accessToken);

                if (userInfoResult.IsError)
                {
                    resultTextBox.Text += "\n" + $"An error occurred getting user info: {userInfoResult.Error}";
                }
                else
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("Claims");
                    sb.AppendLine("------");

                    foreach (var claim in userInfoResult.Claims)
                    {
                        sb.AppendLine($"{claim.Type} = {claim.Value}");
                    }

                    resultTextBox.Text += "\n" + sb.ToString();
                }
            }
        }
    }
}
