using System;
using System.Collections.Generic;
using System.Text;

namespace HDByte.MessageBroker.Core
{
    public interface IMessageQueueItem
    {
        void Execute(Subscription subscription);
        Type Type { get; set; }
    }
}
