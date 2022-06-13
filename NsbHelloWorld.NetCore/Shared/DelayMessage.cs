using NServiceBus;

namespace Shared
{
    public class DelayMessage : IMessage
    {
        public static string SendToQueue => "";

        public int Id { get; set; }
        public int DelaySeconds { get; set; }
    }
}