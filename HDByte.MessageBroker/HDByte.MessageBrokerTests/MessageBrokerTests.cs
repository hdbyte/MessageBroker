using HDByte.MessageBroker.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MessageBroker.Tests
{
    [TestFixture]
    public class MessageBrokerTests
    {

        [Test]
        public void IsSubscribedWorks()
        {
            var messageBroker = new HDByte.MessageBroker.Broker();
            var token = messageBroker.Subscribe<EventArgs>(null);

            Assert.That(messageBroker.IsSubscribed(token), Is.EqualTo(true));
            Assert.That(messageBroker.IsSubscribed(new Guid()), Is.EqualTo(false));
        }

        [Test]
        public void UnsubscribeOnlyUnsubcribesGivenToken()
        {
            var messageBroker = new HDByte.MessageBroker.Broker();
            var tokenOne = messageBroker.Subscribe<EventArgs>(null);
            var tokenTwo = messageBroker.Subscribe<EventArgs>(null);

            Assert.That(messageBroker.IsSubscribed(tokenOne), Is.EqualTo(true));
            Assert.That(messageBroker.IsSubscribed(tokenTwo), Is.EqualTo(true));

            messageBroker.Unsubscribe(tokenOne);

            Assert.That(messageBroker.IsSubscribed(tokenOne), Is.EqualTo(false));
            Assert.That(messageBroker.IsSubscribed(tokenTwo), Is.EqualTo(true));
        }

        [Test]
        public void PublishBackgroundThreadWorks()
        {
            var messageBroker = new HDByte.MessageBroker.Broker();

            bool publishExecuted = false;
            int executedThreadID = 0;
            Action<EventArgs> doSomethingOnBackgroundThread = (e) =>
            {
                publishExecuted = true;
                executedThreadID = Thread.CurrentThread.ManagedThreadId;
            };

            var tokenOne = messageBroker.Subscribe<EventArgs>(doSomethingOnBackgroundThread, ActionThread.Background);

            Assert.That(publishExecuted, Is.EqualTo(false));

            messageBroker.Publish(new EventArgs());

            Thread.Sleep(200); // Give MessageBroker plenty of time to process action.
            Assert.That(publishExecuted, Is.EqualTo(true));
            Assert.That(executedThreadID, Is.EqualTo(messageBroker.GetBackgroundThreadID()));
        }

        [Test]
        public void PublishTaskWorks()
        {
            var messageBroker = new HDByte.MessageBroker.Broker();

            bool publishExecuted = false;
            int executedThreadID = 0;
            Action<EventArgs> doSomethingOnBackgroundThread = (e) =>
            {
                publishExecuted = true;
                executedThreadID = Thread.CurrentThread.ManagedThreadId;
            };

            var tokenOne = messageBroker.Subscribe<EventArgs>(doSomethingOnBackgroundThread);

            Assert.That(publishExecuted, Is.EqualTo(false));

            messageBroker.Publish(new EventArgs());

            Thread.Sleep(200); // Give MessageBroker plenty of time to process action.
            Assert.That(publishExecuted, Is.EqualTo(true));
            Assert.That(executedThreadID, Is.Not.EqualTo(messageBroker.GetBackgroundThreadID()));
        }

        [Test]
        public void PublishUIThreadWorks()
        {
            // This is needed because when testing, SynchronizationContext isn't set because we're not using a GUI. Test will fail without this because Context is null until a GUI sets it.
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            var messageBroker = new HDByte.MessageBroker.Broker();

            bool publishExecuted = false;
            int checkBackgroundThreadID = 0;
            Action<EventArgs> doSomethingOnBackgroundThread = (e) =>
            {
                publishExecuted = true;
                checkBackgroundThreadID = Thread.CurrentThread.ManagedThreadId;
            };

            var tokenOne = messageBroker.Subscribe<EventArgs>(doSomethingOnBackgroundThread, ActionThread.UI);

            Assert.That(publishExecuted, Is.EqualTo(false));

            messageBroker.Publish(new EventArgs());

            Thread.Sleep(200); // Give MessageBroker plenty of time to process action.
            Assert.That(publishExecuted, Is.EqualTo(true));
            Assert.That(checkBackgroundThreadID, Is.Not.EqualTo(messageBroker.GetBackgroundThreadID()));
        }
    }
}
