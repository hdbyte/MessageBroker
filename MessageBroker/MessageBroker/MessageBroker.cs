using HDByte.MessageBroker.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace HDByte.MessageBroker
{
    public sealed class MessageBroker
    {
        private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
        private List<Subscription> _subscriptions = new List<Subscription>();
        private readonly object _padLock = new object();

        /// <summary>
        /// Subscribes to a message type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">Method to call when a subscribed message type is broadcasted.</param>
        /// <param name="actionThread">The thread in which the action is performed on. Defaults to ActionThread.Publisher which performs the action on thread that published the event.</param>
        /// <returns></returns>
        public Guid Subscribe<T>(Action<T> action, ActionThread actionThread = ActionThread.Publisher)
        {
            Guid token = Guid.NewGuid();
            Type type = typeof(T);
            Subscription subscription = new Subscription() {Token = token, Type = type, Action = action, ActionThread = actionThread};

            lock (_padLock)
            {
                _subscriptions.Add(subscription);
            }

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

        /// <summary>
        /// Returns true if token is used in any subscription.
        /// </summary>
        /// <param name="token">Token to be checked.</param>
        /// <returns></returns>
        public bool IsSubscribed(Guid token)
        {
            bool isTokenSubscribed = _subscriptions.Any(a => a.Token == token);

            return isTokenSubscribed;
        }

        /// <summary>
        /// Unsubscribes a given token to prevent further actions.
        /// </summary>
        /// <param name="token">Token to unsubscribe.</param>
        /// <returns></returns>
        public bool Unsubscribe(Guid token)
        {
            if (IsSubscribed(token))
            {
                lock (_padLock)
                {
                    var newSubscriptionList = _subscriptions.Where(a => a.Token != token).ToList();
                    _subscriptions = newSubscriptionList;
                }
                return true;
            } else
            {
                return false;
            }
        }
    }
}
