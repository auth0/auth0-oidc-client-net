using Android.App;
using Android.Content;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Base class for automatically wiring up the necessary callback hooks required
    /// to facilitate communication with the Browser implementations.
    /// </summary>
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