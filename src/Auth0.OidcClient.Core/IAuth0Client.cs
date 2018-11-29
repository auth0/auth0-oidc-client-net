using System.Threading.Tasks;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using IdentityModel.OidcClient.Results;

namespace Auth0.OidcClient
{
    public interface IAuth0Client
    {
        /// <summary>
        /// Launches a browser to log the user in.
        /// </summary>
        /// <param name="extraParameters">Any extra parameters that need to be passed to the authorization endpoint.</param>
        /// <returns></returns>
        Task<LoginResult> LoginAsync(object extraParameters = null);

        /// <summary>
        /// Launches a browser to log the user out and clear the Auth0 SSO Cookie
        /// </summary>
        /// <returns></returns>
        Task<BrowserResultType> LogoutAsync();

        /// <summary>
        /// Launches a browser to log the user out and clear the Auth0 SSO Cookie
        /// </summary>
        /// <param name="federated">Indicates whether the user should also be logged out of their identity provider.</param>
        /// <returns></returns>
        Task<BrowserResultType> LogoutAsync(bool federated);

        /// <summary>
        /// Generates an <see cref="IdentityModel.OidcClient.AuthorizeState"/> containing the URL, state, nonce and code challenge which can
        /// be used to redirect the user to the authorization URL, and subsequently process any response by calling
        /// the <see cref="ProcessResponseAsync"/> method.
        /// </summary>
        /// <param name="extraParameters"></param>
        /// <returns></returns>
        Task<AuthorizeState> PrepareLoginAsync(object extraParameters = null);

        /// <summary>
        /// Process the response from the Auth0 redirect URI
        /// </summary>
        /// <param name="data">The data containing the full redirect URI.</param>
        /// <param name="state">The <see cref="IdentityModel.OidcClient.AuthorizeState"/> which was generated when the <see cref="PrepareLoginAsync"/>
        /// method was called.</param>
        /// <returns></returns>
        Task<LoginResult> ProcessResponseAsync(string data, AuthorizeState state);

        /// <summary>
        /// Generates a new set of tokens based on a refresh token. 
        /// </summary>
        /// <param name="refreshToken">The refresh token which was issued during the authorization flow, or subsequent
        /// calls to <see cref="RefreshTokenAsync"/>.</param>
        /// <returns></returns>
        Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken);
    }
}