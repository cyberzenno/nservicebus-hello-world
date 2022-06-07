using System;
using System.Threading.Tasks;
using NServiceBus;
using Shared;

namespace Server
{
    public class PlaceOrderHandler : IHandleMessages<PlaceOrderMessage>
    {
        public Task Handle(PlaceOrderMessage message, IMessageHandlerContext context)
        {
            Console.WriteLine($"PlaceOrder received {message.Id}");

            var somethingHappened = new SomethingHappenedInTheServerEvent()
            {
                Id = message.Id,
                Message = "Something happened published by SERVER"
            };

            context.Publish(somethingHappened);

            Console.WriteLine($"Published that something happened on the server{somethingHappened.Id}");

            var orderPlaced = new OrderPlacedEvent
            {
                Id = message.Id,
                Message = "Sent from SERVER"
            };

            context.Publish(orderPlaced);

            Console.WriteLine($"Published Order Placed {orderPlaced.Id}\n\n");


            return Task.CompletedTask;
        }
    }
}
