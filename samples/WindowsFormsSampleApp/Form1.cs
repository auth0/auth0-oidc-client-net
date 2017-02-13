using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Auth0.OidcClient;

namespace WindowsFormsSampleApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var client = new Auth0Client("jerrie.auth0.com", "vV9twaySQzfGesS9Qs6gOgqDsYDdgoKE");

            await client.LoginAsync();

        }
    }
}
