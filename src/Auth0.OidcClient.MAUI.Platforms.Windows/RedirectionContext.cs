using System.Collections.Specialized;
using System.Text.Json.Nodes;

namespace Auth0.OidcClient.Platforms.Windows
{
    internal class RedirectionContext
    {
        /// <summary>
        /// The id of the task associated with the current redirect.
        /// </summary>
        internal string TaskId { get; set; }

        /// <summary>
        /// The key of the application associated with the current redirect.
        /// </summary>
        internal string AppInstanceKey { get; set; }

        /// <summary>
        /// Generate a new <see cref="RedirectionContext"/> based on the application instance.
        /// </summary>
        /// <param name="appInstanceProxy">The current application instance.</param>
        /// <returns>The newly created <see cref="RedirectionContext"/></returns>
        internal static RedirectionContext New(IAppInstanceProxy appInstanceProxy)
        {
            return new RedirectionContext
            {
                TaskId = Guid.NewGuid().ToString(),
                AppInstanceKey = appInstanceProxy.GetCurrentAppKey()
            };
        }

        /// <summary>
        /// Converts the <see cref="RedirectionContext"/> to a <see cref="JsonObject"/> for serialization.
        /// </summary>
        /// <param name="query">A <see cref="NameValueCollection"/> holding an optional state property to incorporate in the <see cref="JsonObject"/>.</param>
        /// <returns></returns>
        internal JsonObject ToJsonObject(NameValueCollection query)
        {
            var jsonObject = new JsonObject
            {
                { "appInstanceKey", AppInstanceKey },
                { "taskId", TaskId }
            };

            if (query["state"] is string oldState && !string.IsNullOrEmpty(oldState))
            {
                jsonObject["state"] = oldState;
            }

            return jsonObject;
        }
    }
}