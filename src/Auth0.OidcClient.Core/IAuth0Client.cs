using System.Threading.Tasks;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Results;

namespace Auth0.OidcClient.Core
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
        /// Generates an <see cref="IdentityModel.OidcClient.AuthorizeState"/> containing the URL, state, nonce and code challenge which can
        /// be used to redirect the user to the authorization URL, and subsequently process any response by calling
        /// the <see cref="Auth0.OidcClient.Core.IAuth0Client.ProcessResponseAsync(string,IdentityModel.OidcClient.AuthorizeState)"/> method.
        /// </summary>
        /// <param name="extraParameters"></param>
        /// <returns></returns>
        Task<AuthorizeState> PrepareLoginAsync(object extraParameters = null);

        /// <summary>
        /// Process the response from the Auth0 redirect URI
        /// </summary>
        /// <param name="data">The data containing the full redirect URI.</param>
        /// <param name="state">The <see cref="IdentityModel.OidcClient.AuthorizeState"/> which was generated when the <see cref="Auth0.OidcClient.Core.IAuth0Client.PrepareLoginAsync(object)"/>
        /// method was called.</param>
        /// <returns></returns>
        Task<LoginResult> ProcessResponseAsync(string data, AuthorizeState state);

        /// <summary>
        /// Generates a new set of tokens based on a refresh token. 
        /// </summary>
        /// <param name="refreshToken">The refresh token which was issued during the authorization flow, or subsequent
        /// calls to <see cref="Auth0.OidcClient.Core.IAuth0Client.RefreshTokenAsync(string)"/>.</param>
        /// <returns></returns>
        Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken);
    }
}