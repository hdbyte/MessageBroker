using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageBroker.Tests
{
    [TestFixture]
    public class MessageBrokerTests
    {

        [Test]
        public void IsSubscribedWorks()
        {
            var messageBroker = new HDByte.MessageBroker.MessageBroker();
            var token = messageBroker.Subscribe<EventArgs>(null);

            Assert.That(messageBroker.IsSubscribed(token), Is.EqualTo(true));
            Assert.That(messageBroker.IsSubscribed(new Guid()), Is.EqualTo(false));
        }

        [Test]
        public void UnsubscribeOnlyUnsubcribesGivenToken()
        {
            var messageBroker = new HDByte.MessageBroker.MessageBroker();
            var tokenOne = messageBroker.Subscribe<EventArgs>(null);
            var tokenTwo = messageBroker.Subscribe<EventArgs>(null);

            Assert.That(messageBroker.IsSubscribed(tokenOne), Is.EqualTo(true));
            Assert.That(messageBroker.IsSubscribed(tokenTwo), Is.EqualTo(true));

            messageBroker.Unsubscribe(tokenOne);

            Assert.That(messageBroker.IsSubscribed(tokenOne), Is.EqualTo(false));
            Assert.That(messageBroker.IsSubscribed(tokenTwo), Is.EqualTo(true));
        }

        [Test]
        public void PublishWorks()
        {
            var messageBroker = new HDByte.MessageBroker.MessageBroker();

            bool publishExecuted = false;
            Action<EventArgs> doSomething = (e) => publishExecuted = true;

            var tokenOne = messageBroker.Subscribe<EventArgs>(doSomething);

            Assert.That(publishExecuted, Is.EqualTo(false));

            messageBroker.Publish(new EventArgs());
            Assert.That(publishExecuted, Is.EqualTo(true));
        }
    }
}
