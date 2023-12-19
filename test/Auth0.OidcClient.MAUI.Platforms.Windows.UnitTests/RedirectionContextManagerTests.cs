using System.Text.Json.Nodes;
using Windows.ApplicationModel.Activation;
using Microsoft.Windows.AppLifecycle;
using Moq;
using WinRT;
using Auth0.OidcClient.Platforms.Windows;

namespace Auth0.OidcClient.MAUI.Platforms.Windows.UnitTests
{
    public class RedirectionContextManagerTests
    {
        [Fact]
        public void Should_Get_Redirection_Context()
        {
            var eventArgsMock = new Mock<IProtocolActivatedEventArgs>();

            var jsonObject = new JsonObject
            {
                { "appInstanceKey", "abc" },
                { "taskId", "def" }
            };

            var query = System.Web.HttpUtility.ParseQueryString("");

            query["state"] = Helpers.Encode(jsonObject.ToJsonString());

            UriBuilder uriBuilder = new UriBuilder("http://localhost");
            uriBuilder.Query = query.ToString();

            eventArgsMock.SetupGet(x => x.Uri).Returns(uriBuilder.Uri);

            var ctx = OidcClient.Platforms.Windows.RedirectionContextManager.GetRedirectionContext(eventArgsMock.Object);

            Assert.Equal("abc", ctx.AppInstanceKey);
            Assert.Equal("def", ctx.TaskId);
        }

        [Fact]
        public void Should_Get_Redirection_Context_When_Escaped()
        {
            var eventArgsMock = new Mock<IProtocolActivatedEventArgs>();

            var jsonObject = new JsonObject
            {
                { "appInstanceKey", "abc" },
                { "taskId", "def" }
            };


            var query = System.Web.HttpUtility.ParseQueryString("");

            query["state"] = Helpers.Encode(jsonObject.ToJsonString());


            UriBuilder uriBuilder = new UriBuilder("http://localhost")
            {
                Query = query.ToString()
            };

            eventArgsMock.SetupGet(x => x.Uri).Returns(uriBuilder.Uri);

            var ctx = OidcClient.Platforms.Windows.RedirectionContextManager.GetRedirectionContext(eventArgsMock.Object);

            Assert.Equal("abc", ctx.AppInstanceKey);
            Assert.Equal("def", ctx.TaskId);
        }

        [Fact]
        public void Should_Get_Redirection_Context_Without_Values()
        {
            var eventArgsMock = new Mock<IProtocolActivatedEventArgs>();

            var jsonObject = new JsonObject { };

            var query = System.Web.HttpUtility.ParseQueryString("");

            query["state"] = Helpers.Encode(jsonObject.ToJsonString());


            UriBuilder uriBuilder = new UriBuilder("http://localhost")
            {
                Query = query.ToString()
            };

            eventArgsMock.SetupGet(x => x.Uri).Returns(uriBuilder.Uri);

            var ctx = OidcClient.Platforms.Windows.RedirectionContextManager.GetRedirectionContext(eventArgsMock.Object);

            Assert.Null(ctx.AppInstanceKey);
            Assert.Null(ctx.TaskId);
        }

        [Fact]
        public void Should_Get_null()
        {
            var eventArgsMock = new Mock<IProtocolActivatedEventArgs>();

            var query = System.Web.HttpUtility.ParseQueryString("");

            UriBuilder uriBuilder = new UriBuilder("http://localhost")
            {
                Query = query.ToString()
            };

            eventArgsMock.SetupGet(x => x.Uri).Returns(uriBuilder.Uri);

            var ctx = OidcClient.Platforms.Windows.RedirectionContextManager.GetRedirectionContext(eventArgsMock.Object);

            Assert.Null(ctx);
        }
    }
}