using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auth0.OidcClient.Core;
using Xamarin.Forms;

namespace XamarinFormsTest
{
	public partial class MainPage : ContentPage
	{
	    private readonly IAuth0Client _auth0Client;

		public MainPage()
		{
			InitializeComponent();

		    _auth0Client = DependencyService.Get<IAuth0Client>();

		    Login.Clicked += Login_Clicked;
		}

	    private async void Login_Clicked(object sender, EventArgs e)
	    {
	        var result = await _auth0Client.LoginAsync();
	    }
	}
}
