using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace HDByte.MessageBroker.Events
{
    public class BaseEvent
    {
        private static DateTime? _applicationStartTime;
        public readonly string Timestamp;

        public BaseEvent()
        {
            if (_applicationStartTime == null)
            {
                _applicationStartTime = Process.GetCurrentProcess().StartTime.ToUniversalTime();
            }

            decimal timeDifference = Convert.ToDecimal((DateTime.UtcNow - (DateTime)_applicationStartTime).TotalMilliseconds) / 1000;
            Timestamp = timeDifference.ToString("00000.000");
        }
    }
}
