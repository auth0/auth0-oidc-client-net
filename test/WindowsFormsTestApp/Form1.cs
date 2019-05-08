using Auth0.OidcClient;
using System;
using System.Windows.Forms;

namespace WindowsFormsTestApp
{
    public partial class Form1 : Form
    {
        private Auth0Client _auth0Client;
        private Action<string> writeLine;
        
        public Form1()
        {
            InitializeComponent();
            writeLine = (s) => outputTextBox.Text += s + "\r\n";

            _auth0Client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = "auth0-dotnet-integration-tests.auth0.com",
                ClientId = "qmss9A66stPWTOXjR6X1OeA0DLadoNP2"
            });
        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {
            writeLine("Starting login...");

            var loginResult = await _auth0Client.LoginAsync();

            if (loginResult.IsError)
            {
                writeLine($"An error occurred during login: {loginResult.Error}");
            }
            else
            {
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
        }

        private async void LogoutButton_Click(object sender, EventArgs e)
        {
            writeLine("Starting logout...");

            var result = await _auth0Client.LogoutAsync();

            writeLine(result.ToString());
        }
    }
}
