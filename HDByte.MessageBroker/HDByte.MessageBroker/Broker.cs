using HDByte.MessageBroker.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;

namespace HDByte.MessageBroker
{
    public sealed class Broker
    {
        private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
        private List<Subscription> _subscriptions = new List<Subscription>();
        private readonly object _padLock = new object();
        public readonly BlockingCollection<IMessageQueueItem> _messageQueue = new BlockingCollection<IMessageQueueItem>();
        private readonly Thread _thread;

        public Broker()
        {
            _thread = new Thread(() => EventQueueThreadConsumer());
            _thread.IsBackground = true;
            _thread.Start();
        }

        /// <summary>
        /// Subscribes to a message type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">Method to call when a subscribed message type is broadcasted.</param>
        /// <param name="actionThread">The thread in which the action is performed on. Defaults to ActionThread.Task</param>
        /// <returns></returns>
        public Guid Subscribe<T>(Action<T> action, ActionThread actionThread = ActionThread.Task)
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
            var messageQueue = new MessageQueueItem<T>() { Message = message, Type = typeof(T) };
            _messageQueue.Add(messageQueue);
        }

        /// <summary>
        /// The method that the background thread is using on a loop.
        /// </summary>
        private void EventQueueThreadConsumer()
        {
            while (!_messageQueue.IsCompleted)
            {
                if (_messageQueue.TryTake(out IMessageQueueItem message))
                {
                    Type messageType = message.Type;

                    lock (_padLock)
                    {
                        foreach (Subscription subscription in _subscriptions)
                        {
                            if (subscription.Type.IsAssignableFrom(messageType))
                            {
                                switch (subscription.ActionThread)
                                {
                                    case ActionThread.UI:
                                        _synchronizationContext.Post(delegate { message.Execute(subscription); }, null);
                                        break;
                                    case ActionThread.Background:
                                        message.Execute(subscription);
                                        break;
                                    case ActionThread.Task:
                                    default:
                                        Task.Run(() => { message.Execute(subscription); });
                                        break;
                                }
                            }
                        }
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

        /// <summary>
        /// Returns the Thread ID of the Broker consumer thread.
        /// 
        /// Mostly useful for testing purposes but may be needed in rare circumstances.
        /// </summary>
        /// <returns></returns>
        public int GetBackgroundThreadID()
        {
            return _thread.ManagedThreadId;
        }
    }
}
