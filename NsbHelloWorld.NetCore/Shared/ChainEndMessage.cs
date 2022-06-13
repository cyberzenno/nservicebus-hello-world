using NServiceBus;

namespace Shared
{
    public class ChainEndMessage : IMessage
    {
        public static string SendToQueue => "";

        public int Id { get; set; }
    }
}