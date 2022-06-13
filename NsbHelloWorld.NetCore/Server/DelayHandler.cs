using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using Shared;

namespace Server
{
    public class DelayHandler : IHandleMessages<DelayMessage>
    {
        public Task Handle(DelayMessage message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Delay received {message.Id}");

            var somethingHappened = new SomethingHappenedInTheServerEvent()
            {
                Id = message.Id,
                Message = "Delay was finished"
            };

            context.Publish(somethingHappened);

            Console.WriteLine($"Delay was finished on the server {somethingHappened.Id}");

            return Task.CompletedTask;
        }
    }
}
