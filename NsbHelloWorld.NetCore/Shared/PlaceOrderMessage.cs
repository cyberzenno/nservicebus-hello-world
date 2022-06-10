using NServiceBus;

namespace Shared
{
    public class PlaceOrderMessage : IMessage
    {
        public static string SendToQueue => "";

        public int Id { get; set; }
        public string Product { get; set; }
    }
}