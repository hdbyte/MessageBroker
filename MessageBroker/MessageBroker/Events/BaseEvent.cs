using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace HDByte.MessageBroker.Events
{
    public class BaseEvent
    {
        public static DateTime _applicationStartTime = Process.GetCurrentProcess().StartTime.ToUniversalTime();
        public string Timestamp;

        public BaseEvent()
        {
            decimal timeDifference = Convert.ToDecimal((DateTime.UtcNow - _applicationStartTime).TotalMilliseconds) / 1000;
            Timestamp = timeDifference.ToString("00000.000");
        }
    }
}
