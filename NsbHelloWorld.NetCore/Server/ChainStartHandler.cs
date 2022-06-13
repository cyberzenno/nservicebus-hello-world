using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using Shared;

namespace Server
{
    public class ChainStartHandler : IHandleMessages<ChainStartMessage>
    {
        public async Task Handle(ChainStartMessage message, IMessageHandlerContext context)
        {
            Console.WriteLine($"ChainStart received {message.Id} with success status {message.MessageShouldSucceed} {DateTime.Now}");

            var chainEnd = new ChainEndMessage()
            {
                Id = message.Id
            };

           await context.Send(chainEnd);

            Thread.Sleep(5000);

            if (!message.MessageShouldSucceed)
                throw new Exception("Intentional exception invoked");

            Console.WriteLine($"Invoked chain end {chainEnd.Id}");

            var somethingHappened = new SomethingHappenedInTheServerEvent()
            {
                Id = message.Id,
                Message = "Chain End was invoked"
            };

            await context.Publish(somethingHappened);

            Console.WriteLine($"Chain End was invoked on the server {somethingHappened.Id}");
        }
    }
}
