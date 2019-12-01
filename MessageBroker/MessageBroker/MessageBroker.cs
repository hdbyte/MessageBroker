using HDByte.MessageBroker.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HDByte.MessageBroker
{
    public sealed class MessageBroker
    {
        private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
        private readonly List<Subscription> _subscriptions = new List<Subscription>();

        /// <summary>
        /// Subscribes to a message type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">Method to call when a subscribed message type is broadcasted.)</param>
        /// <param name="actionThread">The thread in which the action is performed on. Defaults to ActionThread.Publisher</param>
        /// <returns></returns>
        public Guid Subscribe<T>(Action<T> action, ActionThread actionThread = ActionThread.Publisher)
        {
            Guid token = Guid.NewGuid();
            Type type = typeof(T);
            Subscription subscription = new Subscription() {Token = token, Type = type, Action = action, ActionThread = actionThread};

            _subscriptions.Add(subscription);

            return token;
        }

        /// <summary>
        /// Publishes a message to all subscribers of that type of message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message that is broadcast.</param>
        public void Publish<T>(T message)
        {
            Type messageType = typeof(T);

            foreach(Subscription subscription in _subscriptions)
            {
                if (subscription.Type.IsAssignableFrom(messageType))
                {
                    switch (subscription.ActionThread)
                    {
                        case ActionThread.Ui:
                            _synchronizationContext.Post(delegate { ((Action<T>)subscription.Action)(message); }, null);
                            break;
                        case ActionThread.Background:
                            Task.Run(() => ((Action<T>)subscription.Action)(message));
                            break;
                        case ActionThread.Publisher:
                        default:
                            ((Action<T>)subscription.Action)(message);
                            break;
                    }
                }
            }
        }
    }
}
