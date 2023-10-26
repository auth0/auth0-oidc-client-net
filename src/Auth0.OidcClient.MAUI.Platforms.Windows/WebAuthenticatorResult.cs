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
    /// <param name="uri">The callback uri that was used to end the authentication sequence.</param>
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
    /// The uri that was used to call back.
    /// </summary>
    public Uri CallbackUri { get; }

    /// <summary>
    /// The dictionary of key/value pairs parsed form the callback URI's query string.
    /// </summary>
    public Dictionary<string, string> Properties { get; } = new(StringComparer.Ordinal);
}