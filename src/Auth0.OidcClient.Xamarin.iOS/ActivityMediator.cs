namespace Auth0.OidcClient
{
    public class ActivityMediator
    {
        public delegate void MessageReceivedEventHandler(string message);

        private static ActivityMediator _instance;

        public static ActivityMediator Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ActivityMediator();

                return _instance;
            }
        }

        private ActivityMediator()
        {
        }

        public event MessageReceivedEventHandler ActivityMessageReceived;

        public void Cancel()
        {
            ActivityMessageReceived?.Invoke("UserCancel");
        }

        public void Send(string response)
        {
            ActivityMessageReceived?.Invoke(response);
        }
    }
}