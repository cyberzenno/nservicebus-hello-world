using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class SimpleEvent : IEvent
    {
        public int Id { get; set; }
        public string Message { get; set; }
    }
}
