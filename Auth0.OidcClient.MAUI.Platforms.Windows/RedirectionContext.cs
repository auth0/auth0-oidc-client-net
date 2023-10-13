using System.Collections.Specialized;
using System.Text.Json.Nodes;

namespace Auth0.OidcClient.Platforms.Windows
{
    internal class RedirectionContext
    {
        internal string TaskId { get; set; }
        internal string AppInstanceKey { get; set; }

        internal static RedirectionContext New(IAppInstanceProxy appInstanceProxy)
        {
            return new RedirectionContext
            {
                TaskId = Guid.NewGuid().ToString(),
                AppInstanceKey = appInstanceProxy.GetCurrentAppKey()
            };
        }

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