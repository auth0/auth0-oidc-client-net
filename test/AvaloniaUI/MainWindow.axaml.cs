using System;
using Auth0.OidcClient;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AvaloniaUI
{
    public partial class MainWindow : Window
    {
        private readonly Auth0Client _auth0Client;
        private readonly Action<string> writeLine;
        private Action clearText;
        private string accessToken;

        public MainWindow()
        {
            InitializeComponent();
            writeLine = (text) => outputText.Text += text + "\n";
            clearText = () => outputText.Text = "";

            _auth0Client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = "auth0-dotnet-integration-tests.auth0.com",
                ClientId = "qmss9A66stPWTOXjR6X1OeA0DLadoNP2"
            });
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            clearText();
            writeLine("Starting login...");

            var loginResult = await _auth0Client.LoginAsync( new { organization = "" });

            if (loginResult.IsError)
            {
                writeLine($"An error occurred during login: {loginResult.Error}");
                return;
            }

            accessToken = loginResult.AccessToken;

            writeLine($"id_token: {loginResult.IdentityToken}");
            writeLine($"access_token: {loginResult.AccessToken}");
            writeLine($"refresh_token: {loginResult.RefreshToken}");

            writeLine($"name: {loginResult.User.FindFirst(c => c.Type == "name")?.Value}");
            writeLine($"email: {loginResult.User.FindFirst(c => c.Type == "email")?.Value}");

            foreach (var claim in loginResult.User.Claims)
            {
                writeLine($"{claim.Type} = {claim.Value}");
            }
        }

        private async void LogoutButton_OnClick(object sender, RoutedEventArgs e)
        {
            clearText();
            writeLine("Starting logout...");

            var result = await _auth0Client.LogoutAsync();
            accessToken = null;
            writeLine(result.ToString());
        }

        private async void UserInfoButton_Click(object sender, RoutedEventArgs e)
        {
            clearText();

            if (string.IsNullOrEmpty(accessToken))
            {
                writeLine("You need to be logged in to get user info");
                return;
            }

            writeLine("Getting user info...");
            var userInfoResult = await _auth0Client.GetUserInfoAsync(accessToken);

            if (userInfoResult.IsError)
            {
                writeLine($"An error occurred getting user info: {userInfoResult.Error}");
                return;
            }

            foreach (var claim in userInfoResult.Claims)
            {
                writeLine($"{claim.Type} = {claim.Value}");
            }
        }
    }
}