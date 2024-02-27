using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using Windows.ApplicationModel.Activation;

[assembly: InternalsVisibleTo("Auth0.OidcClient.MAUI.Platforms.Windows.UnitTests")]
namespace Auth0.OidcClient.Platforms.Windows
{
    internal class RedirectionContextManager
    {
        /// <summary>
        /// Gets the <see cref="RedirectionContext"/> from the provided  <see cref="IProtocolActivatedEventArgs"/>
        /// </summary>
        /// <param name="protocolArgs">The event arguments associated with the corresponding protocol activation.</param>
        /// <returns>The newly created <see cref="RedirectionContext"/>.</returns>
        internal static RedirectionContext? GetRedirectionContext(IProtocolActivatedEventArgs protocolArgs)
        {
            var query = System.Web.HttpUtility.ParseQueryString(protocolArgs.Uri.Query);
            var state = query["state"];
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
            
            return null;
        }


        /// <summary>
        /// Helper method to try and get a value from a <see cref="JsonObject"/>.
        /// </summary>
        /// <param name="jsonObject">The corresponding <see cref="JsonObject"/>.</param>
        /// <param name="key">The key for the value to be retrieve from the <see cref="JsonObject"/></param>
        /// <returns>The value from the provided key, or null if not found.</returns>
        private static string TryGetJsonValue(JsonObject jsonObject, string key)
        {
            if (jsonObject.ContainsKey(key) && jsonObject[key] is JsonValue jValue && jValue.TryGetValue(out string value))
            {
                return value;
            }

            return null;
        }
    }
}