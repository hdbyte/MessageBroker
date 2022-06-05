using System;

namespace HDByte.MessageBroker.Core
{
    public sealed class Subscription
    {
        public Type Type { get; set; }
        public object Action { get; set; }
        public ActionThread ActionThread { get; set; }

    }
}
