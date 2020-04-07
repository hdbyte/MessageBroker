using HDByte.MessageBroker.Events;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace MessageBroker.Tests
{
    [TestFixture]
    public class EventTests
    {

        [Test]
        public void BaseEventTimestampWorks()
        {
            var applicationStartTime = Process.GetCurrentProcess().StartTime.ToUniversalTime();
            var offsetTimestamp = ((DateTime.UtcNow - applicationStartTime).TotalMilliseconds) / 1000;

            var baseEvent = new BaseEvent();
            var beforeTimestamp = Double.Parse(baseEvent.Timestamp);

            Thread.Sleep(2000);

            var baseEvent2 = new BaseEvent();
            var afterTimestamp = Double.Parse(baseEvent2.Timestamp);
            var difference = afterTimestamp - beforeTimestamp;

            Assert.That(difference > 1.5 && difference < 2.5, Is.EqualTo(true));
            Assert.That(beforeTimestamp - offsetTimestamp < .01, Is.EqualTo(true));
        }

    }
}
