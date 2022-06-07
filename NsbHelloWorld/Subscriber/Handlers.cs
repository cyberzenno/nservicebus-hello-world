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

    public class SomethingHappenedInTheClientHandler : IHandleMessages<SomethingHappenedInTheClientEvent>
    {
        public Task Handle(SomethingHappenedInTheClientEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine($"SomethingHappenedInTheClient {message.Id}");//: {message.Id}, {message.Message}");
            return Task.CompletedTask;
        }
    }

    public class SomethingHappenedInTheServerHandler : IHandleMessages<SomethingHappenedInTheServerEvent>
    {
        public Task Handle(SomethingHappenedInTheServerEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine($"SomethingHappenedInTheServer {message.Id}");//: {message.Id}, {message.Message}\n\n");
            return Task.CompletedTask;
        }
    }
}
