using NServiceBus;

namespace Shared
{
    public class PlaceOrderMessage : IMessage
    {
        public int Id { get; set; }
        public string Product { get; set; }
    }
}