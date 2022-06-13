using NServiceBus;

namespace Shared
{
    public class ChainStartMessage : IMessage
    {
        public static string SendToQueue => "";

        public int Id { get; set; }
        public bool MessageShouldSucceed { get; set; }
    }
}