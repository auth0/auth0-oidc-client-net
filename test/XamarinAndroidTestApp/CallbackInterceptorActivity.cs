using Android.App;
using Android.Content;
using Android.OS;
using Auth0.OidcClient;

namespace XamarinAndroidTestApp
{
    [Activity(Label = "Auth0 OIDC Callback Activity")]
    [IntentFilter(
        new[] {Intent.ActionView},
        Categories = new[] {Intent.CategoryDefault, Intent.CategoryBrowsable},
        DataScheme = "https",
        DataHost = "@string/auth0_domain",
        DataPathPrefix = "/android/XamarinAndroidTestApp.XamarinAndroidTestApp/callback")]
    public class CallbackInterceptorActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Finish();

            ActivityMediator.Instance.Send(Intent.DataString);

            // We don't need to redirect to MainActivity explicitly...?
            //StartActivity(typeof(MainActivity));
        }
    }
}