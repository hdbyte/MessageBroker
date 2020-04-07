using System;
using System.Collections.Generic;
using System.Text;

namespace HDByte.MessageBroker.Events
{
    public class GenericEvent : BaseEvent
    {
        public string Message { get; set; }
    }
}
