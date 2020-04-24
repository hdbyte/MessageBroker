using System;
using System.Collections.Generic;
using System.Text;

namespace HDByte.MessageBroker.Core
{
    public class MessageQueueItem<TMessage> : IMessageQueueItem
    {
        public TMessage Message { get; set; }
        public Type Type { get; set; }
        public void Execute(Subscription subscription)
        {
            ((Action<TMessage>)subscription.Action)(Message);
        }
    }
}
