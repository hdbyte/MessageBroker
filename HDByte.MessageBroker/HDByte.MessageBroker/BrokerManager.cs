namespace HDByte.MessageBroker
{
    public sealed class BrokerManager
    {
        private static readonly object _padLock = new object();
        private static Broker _instance = null;

        /// <summary>
        /// Creates and/or returns an instance of Broker.
        /// </summary>
        /// <returns></returns>
        public static Broker GetBroker()
        {
            if (_instance == null)
            {
                lock (_padLock)
                {
                    if (_instance == null)
                    {
                        _instance = new Broker();
                    }
                }
            }

            return _instance;
        }
    }
}
