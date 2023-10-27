using System.Text.Json.Nodes;

namespace Auth0.OidcClient.Platforms.Windows;

internal class StateModifier
{
    /// <summary>
    /// Takes the state query param, and moves it to be a query param on the URL passed to returnTo
    /// </summary>
    /// <param name="uri">The Uri instance that contains both a state and returnTo query parameter.</param>
    /// <returns></returns>
    internal static Uri MoveStateToReturnTo(Uri uri)
    {
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        // The state QueryString
        var state = query["state"];
        // The original returnTo as configured externally
        var returnTo = query["returnTo"];

        UriBuilder returnToBuilder = new UriBuilder(returnTo!);

        // Get the original returnTo querystring params, so we can append state to it
        var returnToQuery = System.Web.HttpUtility.ParseQueryString(new Uri(returnTo).Query);
        // Append state as a querystring parameter to returnTo
        // We need to escape it for it to be accepted
        returnToQuery["state"] = state;
        // Set the query again on the returnTo url
        returnToBuilder.Query = returnToQuery.ToString() ?? string.Empty;

        // Update returnTo in the original query so that it now includes state
        query["returnTo"] = returnToBuilder.Uri.ToString();
        // Remove original state
        query.Remove("state");

        UriBuilder uriBuilder = new UriBuilder(uri);
        // Set the query again on the logout url
        uriBuilder.Query = query.ToString() ?? string.Empty;

        // Return the Uri
        return uriBuilder.Uri;
    }

    /// <summary>
    /// Wraps the state query parameter with the redirection context.
    /// </summary>
    /// <param name="uri">The uri containing a state parameter.</param>
    /// <param name="redirectContext">The <see cref="RedirectionContext"/> used to wrap the state query parameter.</param>
    /// <returns></returns>
    internal static Uri WrapStateWithRedirectionContext(Uri uri, RedirectionContext redirectContext)
    {
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        var redirectContextJson = redirectContext.ToJsonObject(query);

        query["state"] = Helpers.Encode(redirectContextJson.ToJsonString());

        UriBuilder authorizeUriBuilder = new UriBuilder(uri)
        {
            Query = query.ToString() ?? string.Empty
        };

        return authorizeUriBuilder.Uri;
    }

    /// <summary>
    /// Unwraps the state and removes the redirection context from it.
    /// </summary>
    /// <param name="uri">The uri containing a wrapped state query parameter.</param>
    /// <returns>The uri contained an unwrapped state query parameter.</returns>
    internal static Uri UnwrapRedirectionContextFromState(Uri uri)
    {
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

        var state = Helpers.Decode(query["state"]);

        JsonObject jsonObject = JsonNode.Parse(state ?? "{}") as JsonObject;

        var originalState = jsonObject["state"];

        if (originalState is not null)
        {
            query["state"] = originalState.ToString();
        }
        else
        {
            query.Remove("state");
        }


        UriBuilder uriBuilder = new UriBuilder(uri);
        uriBuilder.Query = query.ToString();
        return uriBuilder.Uri;

    }
}