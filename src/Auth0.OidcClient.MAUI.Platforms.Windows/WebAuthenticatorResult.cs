namespace Auth0.OidcClient.Platforms.Windows;

/// <summary>
/// Represents a Web Authenticator Result object parsed from the callback Url.
/// </summary>
/// <remarks>
/// All of the query string or url fragment properties are parsed into a dictionary and can be accessed by their key.
/// </remarks>
public class WebAuthenticatorResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebAuthenticatorResult"/> class by parsing a URI's query string parameters.
    /// </summary>
    /// <remarks>
    /// If the responseDecoder is non-null, then it is used to decode the fragment or query string 
    /// returned by the authorization service.  Otherwise, a default response decoder is used.
    /// </remarks>
    /// <param name="uri">The callback uri that was used to end the authentication sequence.</param>
    /// <param name="responseDecoder">The decoder that can be used to decode the callback uri.</param>
    public WebAuthenticatorResult(Uri uri)
    {
        CallbackUri = uri;
        var properties = System.Web.HttpUtility.ParseQueryString(uri.Query);
        foreach (var key in properties.Keys)
        {
            Properties[(string) key] = properties[(string)key];
        }
    }

    /// <summary>
    /// The uri that was used to call back with the access token.
    /// </summary>
    /// <value>
    /// The value of the callback URI, including the fragment or query string bearing 
    /// the access token and associated information.
    /// </value>
    public Uri CallbackUri { get; }

    /// <summary>
    /// The dictionary of key/value pairs parsed form the callback URI's query string.
    /// </summary>
    public Dictionary<string, string> Properties { get; } = new(StringComparer.Ordinal);
}