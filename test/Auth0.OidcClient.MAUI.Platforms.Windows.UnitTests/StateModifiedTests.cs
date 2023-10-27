using System.Collections.Specialized;
using Auth0.OidcClient.Platforms.Windows;
using System.Text.Json.Nodes;

namespace Auth0.OidcClient.MAUI.Platforms.Windows.UnitTests;

public class StateModifiedTests
{
    [Fact]
    public void Should_move_state_to_returnTo()
    {
        var originalUri = new Uri("http://www.my-idp.com?state=abc&returnTo=myapp://callback");

        var newUri = StateModifier.MoveStateToReturnTo(originalUri);

        var newQuery = System.Web.HttpUtility.ParseQueryString(newUri.Query);
        var newReturnToUri = new Uri(newQuery["returnTo"] ?? string.Empty);

        var newReturnToQuery = System.Web.HttpUtility.ParseQueryString(newReturnToUri.Query);

        Assert.Equal(newReturnToQuery["state"], "abc");
        Assert.Null(newQuery["state"]);
    }

    [Fact]
    public void Should_reset_raw_state()
    {
        var jsonObject = new JsonObject
        {
            { "appInstanceKey", "123" },
            { "taskId", "456" }
        };
        
        jsonObject["state"] = "abc";

        var originalUriBuilder = new UriBuilder("http://www.my-idp.com");

        var query = System.Web.HttpUtility.ParseQueryString(new Uri("http://www.my-idp.com").Query);

        query["state"] = Helpers.Encode(jsonObject.ToJsonString());
        

        originalUriBuilder.Query = query.ToString();



        var newUri = StateModifier.UnwrapRedirectionContextFromState(originalUriBuilder.Uri);

        var newQuery = System.Web.HttpUtility.ParseQueryString(newUri.Query);
        var newState = newQuery["state"];

        Assert.Equal(newState, "abc");
    }

    [Fact]
    public void Should_reset_raw_state_when_escaped()
    {
        var jsonObject = new JsonObject
        {
            { "appInstanceKey", "123" },
            { "taskId", "456" }
        };

        jsonObject["state"] = "abc";

        var originalUriBuilder = new UriBuilder("http://www.my-idp.com");

        var query = System.Web.HttpUtility.ParseQueryString(new Uri("http://www.my-idp.com").Query);

        query["state"] = Helpers.Encode(jsonObject.ToJsonString());


        originalUriBuilder.Query = query.ToString();



        var newUri = StateModifier.UnwrapRedirectionContextFromState(originalUriBuilder.Uri);

        var newQuery = System.Web.HttpUtility.ParseQueryString(newUri.Query);
        var newState = newQuery["state"];

        Assert.Equal(newState, "abc");
    }

    [Fact]
    public void Should_remove_state_when_no_original_state()
    {
        var jsonObject = new JsonObject
        {
            { "appInstanceKey", "123" },
            { "taskId", "456" }
        };


        var originalUriBuilder = new UriBuilder("http://www.my-idp.com");

        var query = System.Web.HttpUtility.ParseQueryString(new Uri("http://www.my-idp.com").Query);

        query["state"] = Helpers.Encode(jsonObject.ToJsonString());


        originalUriBuilder.Query = query.ToString();



        var newUri = StateModifier.UnwrapRedirectionContextFromState(originalUriBuilder.Uri);

        var newQuery = System.Web.HttpUtility.ParseQueryString(newUri.Query);
        var newState = newQuery["state"];

        Assert.Null(newState);
    }

}