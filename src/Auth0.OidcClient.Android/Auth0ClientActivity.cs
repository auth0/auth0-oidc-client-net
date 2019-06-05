using Android.App;
using Android.Content;

namespace Auth0.OidcClient
{
    public class Auth0ClientActivity : Activity
    {
        protected override void OnResume()
        {
            base.OnResume();
            ActivityMediator.Instance.Cancel();
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            ActivityMediator.Instance.Send(intent.DataString);
        }
    }
}