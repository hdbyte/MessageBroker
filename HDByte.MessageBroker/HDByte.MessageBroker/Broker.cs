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
        private readonly ConcurrentDictionary<Guid, Subscription> _subscriptions = new ConcurrentDictionary<Guid, Subscription>();
        private readonly BlockingCollection<IMessageQueueItem> _messageQueue = new BlockingCollection<IMessageQueueItem>();
        private readonly Thread _thread;

        public int SubscriptionCount
        {
            get => _subscriptions.Count;
        }

        public int BackgroundThreadID
        {
            get => _thread.ManagedThreadId;
        }

        public Broker()
        {
            _thread = new Thread(EventQueueThreadConsumer)
            {
                IsBackground = true
            };
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
            Subscription subscription = new Subscription() {Type = type, Action = action, ActionThread = actionThread};

            _subscriptions[token] = subscription;

            return token;
        }

        /// <summary>
        /// Publishes a message to all subscribers of that type of message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message that is broadcast.</param>
        public void Publish<T>(T message)
        {
            var messageQueue = new MessageQueueItem<T> { Message = message, Type = typeof(T) };
            _messageQueue.Add(messageQueue);
        }

        /// <summary>
        /// The method that the background thread is using on a loop.
        /// </summary>
        private void EventQueueThreadConsumer()
        {
            foreach (var message in _messageQueue.GetConsumingEnumerable())
            {
                foreach (var keyPair in _subscriptions)
                {
                    var subscription = keyPair.Value;
                    if (subscription.Type.IsAssignableFrom(message.Type))
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

        /// <summary>
        /// Returns true if token is used in any subscription.
        /// </summary>
        /// <param name="token">Token to be checked.</param>
        /// <returns></returns>
        public bool IsSubscribed(Guid token)
        {
            return _subscriptions.ContainsKey(token);
        }

        /// <summary>
        /// Returns true if a type is used in any subscription.
        /// </summary>
        /// <returns></returns>
        public bool IsSubscribed<T>()
        {
            return _subscriptions.Any(x => x.Value.Type == typeof(T));
        }

        /// <summary>
        /// Returns true if type is used in any subscription.
        /// </summary>
        /// <param name="type">Type of class to be checked [use typeof()].</param>
        /// <returns></returns>
        public bool IsSubscribed(Type type)
        {
            return _subscriptions.Any(x => x.Value.Type == type);
        }

        /// <summary>
        /// Unsubscribes a given token to prevent further actions.
        /// </summary>
        /// <param name="token">Token to unsubscribe.</param>
        /// <returns></returns>
        public bool Unsubscribe(Guid token)
        {
            return _subscriptions.TryRemove(token, out _);
        }

        /// <summary>
        /// [DEPRECIATED] Returns the Thread ID of the Broker consumer thread.
        ///
        /// Will be replaced by only int BackgroundThreadID in a future release.
        /// Mostly useful for testing purposes but may be needed in rare circumstances.
        /// </summary>
        /// <returns></returns>
        public int GetBackgroundThreadID()
        {
            return _thread.ManagedThreadId;
        }
    }
}
