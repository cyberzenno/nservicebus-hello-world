using System;
using System.Threading.Tasks;
using NServiceBus;
using Shared;

namespace Subscriber
{
    public class OrderCreatedHandler : IHandleMessages<OrderPlacedEvent>
    {
        public Task Handle(OrderPlacedEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine($"OrderPlaced {message.Id}\n\n");//: {message.OrderId}, {message.Message}");

            return Task.CompletedTask;
        }
    }

}
