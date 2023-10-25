using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using Windows.ApplicationModel.Activation;

[assembly: InternalsVisibleTo("Auth0.OidcClient.MAUI.Platforms.Windows.UnitTests")]
namespace Auth0.OidcClient.Platforms.Windows
{
    internal class RedirectionContextManager
    {
        internal static RedirectionContext? GetRedirectionContext(IProtocolActivatedEventArgs protocolArgs)
        {
            var vals = System.Web.HttpUtility.ParseQueryString(protocolArgs.Uri.Query);
            var state = vals["state"];
            JsonObject jsonObject = null;

            if (!string.IsNullOrEmpty(state))
            {
                jsonObject = JsonNode.Parse(Helpers.Decode(state)) as JsonObject;
            }

            if (jsonObject is not null)
            {
                return new RedirectionContext
                {
                    AppInstanceKey = TryGetJsonValue(jsonObject, "appInstanceKey"),
                    TaskId = TryGetJsonValue(jsonObject, "taskId")
                };
            }
            else
            {
                return null;
            }
        }

        private static string? TryGetJsonValue(JsonObject jsonObject, string key)
        {
            if (jsonObject.ContainsKey(key) && jsonObject[key] is JsonValue jValue && jValue.TryGetValue(out string value))
            {
                return value;
            }

            return null;
        }
    }
}