namespace Auth0.OidcClient
{
    /// <summary>
    /// Facilitates communication between the app entry point and the callback function.
    /// </summary>
    public class ActivityMediator
    {
        private static ActivityMediator _instance;
        private ActivityMediator() { }

        /// <summary>
        /// Singleton instance of the <see cref="ActivityMediator"/> class.
        /// </summary>
        public static ActivityMediator Instance
        {
            get { return _instance ?? (_instance = new ActivityMediator()); }
        }

        /// <summary>
        /// Method signature required for methods subscribing to the ActivityMessageReceived event.
        /// </summary>
        /// <param name="message">Message that has been received.</param>
        public delegate void MessageReceivedEventHandler(string message);

        /// <summary>
        /// Event listener for subscribing to message received events.
        /// </summary>
        public event MessageReceivedEventHandler ActivityMessageReceived;

        /// <summary>
        /// Send a response message to all listeners.
        /// </summary>
        /// <param name="response">Response message to send to all listeners.</param>
        public void Send(string response)
        {
            ActivityMessageReceived?.Invoke(response);
        }

        /// <summary>
        /// Send a cancellation response message "UserCancel" to all listeners.
        /// </summary>
        public void Cancel()
        {
            Send("UserCancel");
        }
    }
}