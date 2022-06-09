using NServiceBus;

namespace Shared
{
    public class OrderPlacedEvent : IEvent
    {
        public int Id { get; set; }
        public string Message { get; set; }
    }
}