namespace HDByte.MessageBroker
{
    public sealed class BrokerManager
    {
        private static readonly object _padLock = new object();
        private static MessageBroker _instance = null;

        /// <summary>
        /// Creates and/or returns an instance of MessageBroker.
        /// </summary>
        /// <returns></returns>
        public static MessageBroker GetMessageBroker()
        {
            if (_instance == null)
            {
                lock (_padLock)
                {
                    if (_instance == null)
                    {
                        _instance = new MessageBroker();
                    }
                }
            }

            return _instance;
        }
    }
}
