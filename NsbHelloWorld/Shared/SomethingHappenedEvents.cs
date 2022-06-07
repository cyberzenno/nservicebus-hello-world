using NServiceBus;

namespace Shared
{
    public class SomethingHappenedInTheClientEvent : IEvent
    {
        public int Id { get; set; }
        public string Message { get; set; }
    }


    public class SomethingHappenedInTheServerEvent : IEvent
    {
        public int Id { get; set; }
        public string Message { get; set; }
    }
}
