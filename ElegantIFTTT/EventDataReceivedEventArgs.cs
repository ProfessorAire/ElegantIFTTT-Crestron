using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace ElegantIFTTT
{
    public class EventDataReceivedEventArgs : EventArgs
    {
        private EventData data;
        public EventData Data { get { return data; } }

        public EventDataReceivedEventArgs(EventData eventData)
        {
            data = eventData;
        }
    }
}