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
            var firstInstance = HDByte.MessageBroker.BrokerManager.GetBroker();
            var secondInstance = new HDByte.MessageBroker.Broker();
            var thirdInstance = HDByte.MessageBroker.BrokerManager.GetBroker();

            Assert.That(firstInstance, Is.SameAs(thirdInstance));
            Assert.That(firstInstance, Is.Not.SameAs(secondInstance));
        }

    }
}
