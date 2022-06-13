using System;
using System.Threading.Tasks;
using NServiceBus;
using Shared;

namespace Server
{
    public class PlaceOrderHandler : IHandleMessages<PlaceOrderMessage>
    {
        public async Task Handle(PlaceOrderMessage message, IMessageHandlerContext context)
        {
            var dealerContext = (string)null;

            var hasDealerContext = context.MessageHeaders.ContainsKey(CustomHeaders.DealerContext);
            if (hasDealerContext)
            {
                dealerContext = context.MessageHeaders[CustomHeaders.DealerContext];
                c.w($"---THIS MESSAGE IS MEANT FOR: {dealerContext}---\n\n");
            }

            c.w($"PlaceOrder received {message.Id}");

            var somethingHappened = new SomethingHappenedInTheServerEvent()
            {
                Id = message.Id,
                Message = "Something happened published by SERVER"
            };

            await context.Publish(somethingHappened);

            Console.WriteLine($"Published that something happened on the server{somethingHappened.Id}");

            var orderPlaced = new OrderPlacedEvent
            {
                Id = message.Id,
                Message = "Sent from SERVER"
            };

            await context.Publish(orderPlaced);

            Console.WriteLine($"Published Order Placed {orderPlaced.Id}\n\n");

            if (dealerContext != null)
            {
                c.w($"---END --- THIS MESSAGE IS MEANT FOR: {dealerContext}---\n\n");
            }
        }
    }
}
