using System;
using System.Diagnostics;
using System.Windows.Forms;
using Auth0.OidcClient;

namespace WindowsFormsTestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var client = new Auth0Client("", "");

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
