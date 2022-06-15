using System;
using System.Threading.Tasks;
using NServiceBus;
using Shared;

namespace SimpleSubscriber
{
    public class SimpleEventHandler : IHandleMessages<SimpleEvent>
    {
        public Task Handle(SimpleEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine($"SimpleEvent received via Auto-Subscription {message.Id}");
            Console.WriteLine(message.Message + "\n\n");

            return Task.CompletedTask;
        }
    }
}
