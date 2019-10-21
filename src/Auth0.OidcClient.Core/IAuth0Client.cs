using System;
using System.Threading.Tasks;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using IdentityModel.OidcClient.Results;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Interface for performing authentication and authorization operations with Auth0 using the
    /// underlying <see cref="IdentityModel.OidcClient.OidcClient"/>.
    /// </summary>
    public interface IAuth0Client
    {
        /// <summary>
        /// Launches a browser to log the user in.
        /// </summary>
        /// <param name="extraParameters">Optional extra parameters that need to be passed to the endpoint.</param>
        /// <returns>A <see cref="LoginResult"/> containing the tokens and claims.</returns>
        Task<LoginResult> LoginAsync(object extraParameters = null);

        /// <summary>
        /// Launches a browser to log the user out and clear the Auth0 SSO Cookie.
        /// </summary>
        /// <param name="federated">Whether to log the user out of their federated identity provider. Defaults to false.</param>
        /// <param name="extraParameters">Optional extra parameters that need to be passed to the endpoint.</param>
        /// <returns>A <see cref="BrowserResultType"/> indicating whether the logout was successful.</returns>
        Task<BrowserResultType> LogoutAsync(bool federated = false, object extraParameters = null);

        /// <summary>
        /// Generates an <see cref="AuthorizeState"/> containing the URL, state, nonce and code challenge which can
        /// be used to redirect the user to the authorization URL, and subsequently process any response by calling
        /// the <see cref="ProcessResponseAsync"/> method.
        /// </summary>
        /// <param name="extraParameters">Optional extra parameters that need to be passed to the endpoint.</param>
        /// <returns>A <see cref="AuthorizeState"/> with necessary URLs, nonce, state and code verifiers.</returns>
        Task<AuthorizeState> PrepareLoginAsync(object extraParameters = null);

        /// <summary>
        /// Process the response from the Auth0 redirect URI.
        /// </summary>
        /// <param name="data">The data containing the full redirect URI.</param>
        /// <param name="state">The <see cref="AuthorizeState"/> which was generated when the <see cref="PrepareLoginAsync"/>
        /// method was called.</param>
        /// <param name="extraParameters">Optional extra parameters that need to be passed to the endpoint.</param>
        /// <returns>A <see cref="LoginResult"/> containing the tokens and claims.</returns>
        Task<LoginResult> ProcessResponseAsync(string data, AuthorizeState state, object extraParameters = null);

        /// <summary>
        /// Generates a new set of tokens based on a refresh token. 
        /// </summary>
        /// <param name="refreshToken">Refresh token which was issued during the authorization flow, or subsequent
        /// calls to <see cref="IdentityModel.OidcClient.OidcClient.RefreshTokenAsync"/>.</param>
        /// <param name="extraParameters">Optional extra parameters that need to be passed to the endpoint.</param>
        /// <returns>A <see cref="RefreshTokenResult"/> with the refreshed tokens.</returns>
        Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken, object extraParameters = null);

        /// <summary>
        /// Gets the user claims from the userinfo endpoint.
        /// </summary>
        /// <param name="accessToken">Access token to use in obtaining claims.</param>
        /// <returns>
        /// <returns>A <see cref="UserInfoResult"/> with the user information and claims.</returns>
        /// </returns>
        /// <exception cref="ArgumentNullException">When <paramref name="accessToken"/> is null.</exception>
        /// <exception cref="InvalidOperationException">When no userinfo endpoint specified.</exception>
        Task<UserInfoResult> GetUserInfoAsync(string accessToken);
    }
}