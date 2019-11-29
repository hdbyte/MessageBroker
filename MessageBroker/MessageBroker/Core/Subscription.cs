using System;
using System.Collections.Generic;
using System.Text;

namespace HDByte.MessageBroker.Core
{
    public sealed class Subscription
    {
        public Guid Key { get; set; }
        public Type Type { get; set; }
        public object Action { get; set; }
        public ActionThread ActionThread { get; set; }

    }
}
