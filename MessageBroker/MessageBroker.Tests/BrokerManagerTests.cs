using NUnit.Framework;
using System;

namespace MessageBroker.Tests
{
    [TestFixture]
    public class BrokerManagerTests
    {
        [Test]
        public void AlwaysReturnsSameInstance()
        {
            var firstInstance = HDByte.MessageBroker.BrokerManager.GetMessageBroker();
            var secondInstance = new HDByte.MessageBroker.MessageBroker();
            var thirdInstance = HDByte.MessageBroker.BrokerManager.GetMessageBroker();

            Assert.That(firstInstance, Is.SameAs(thirdInstance));
            Assert.That(firstInstance, Is.Not.SameAs(secondInstance));
        }
    }
}
