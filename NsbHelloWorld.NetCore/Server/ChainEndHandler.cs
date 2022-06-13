﻿using System;
using System.Threading.Tasks;
using NServiceBus;
using Shared;

namespace Server
{
    public class ChainEndHandler : IHandleMessages<ChainEndMessage>
    {
        public Task Handle(ChainEndMessage message, IMessageHandlerContext context)
        {
            Console.WriteLine($"ChainEnd received {message.Id}");

            var somethingHappened = new SomethingHappenedInTheServerEvent()
            {
                Id = message.Id,
                Message = "Chain End was finished"
            };

            context.Publish(somethingHappened);

            Console.WriteLine($"Chain End was finished on the server {somethingHappened.Id}");

            return Task.CompletedTask;
        }
    }
}
