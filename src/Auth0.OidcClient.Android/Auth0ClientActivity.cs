using Android.App;
using Android.Content;
using IdentityModel.OidcClient.Browser;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Base class for automatically wiring up the necessary callback hooks required
    /// to facilitate communication with the Browser implementations.
    /// </summary>
    public class Auth0ClientActivity : Activity
    {
        /// <summary>
        /// Method executed when the Activity resumes that will cancel any pending
        /// <see cref="IBrowser"/> implementation by way of the <see cref="ActivityMediator"/>.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            ActivityMediator.Instance.Cancel();
        }

        /// <summary>
        /// Method executed when the Activity receives a new intent that may continue a pending
        /// <see cref="IBrowser"/> implementation by way of the <see cref="ActivityMediator"/>.
        /// </summary>
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            ActivityMediator.Instance.Send(intent.DataString);
        }
    }
}