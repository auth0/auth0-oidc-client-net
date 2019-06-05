namespace Auth0.OidcClient
{
    public class ActivityMediator
    {
        private static ActivityMediator _instance;

        private ActivityMediator() { }

        public static ActivityMediator Instance
        {
            get { return _instance ?? (_instance = new ActivityMediator()); }
        }

        public delegate void MessageReceivedEventHandler(string message);
        public event MessageReceivedEventHandler ActivityMessageReceived;

        public void Send(string response)
        {
            ActivityMessageReceived?.Invoke(response);
        }

        public void Cancel()
        {
            Send("UserCancel");
        }
    }
}