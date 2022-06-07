using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using Shared;

namespace Server
{
    public class HelloWorldSaga :
        Saga<HelloWorldSagaData>,
        IAmStartedByMessages<StartHelloWorldSagaMessage>,
        IHandleMessages<SendSomethingToSagaMessage>,
        IHandleMessages<PrintSagaDataMessage>,
        IHandleMessages<CompleteHelloWorldSagaMessage>
    {
        //nsb 6
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<HelloWorldSagaData> mapper)
        {
            mapper.ConfigureMapping<StartHelloWorldSagaMessage>(x => x.Id).ToSaga(x => x.MyId);
            mapper.ConfigureMapping<SendSomethingToSagaMessage>(x => x.Id).ToSaga(x => x.MyId);
            mapper.ConfigureMapping<PrintSagaDataMessage>(x => x.Id).ToSaga(x => x.MyId);
            mapper.ConfigureMapping<CompleteHelloWorldSagaMessage>(x => x.Id).ToSaga(x => x.MyId);
        }

        public Task Handle(StartHelloWorldSagaMessage message, IMessageHandlerContext context)
        {
            c.w($"StartHelloWorldSaga " + message.Id);

            //nsb 6: in nsb 6 the Id field of saga data is used internally for 'stuff'
            Data.MyId = message.Id;

            if (Data.Something == null) Data.Something = new List<string>();

            Data.Something.Add(message.Message);

            return Task.CompletedTask;
        }

        public Task Handle(SendSomethingToSagaMessage message, IMessageHandlerContext context)
        {
            c.w($"SendSomethingToSaga");

            Data.Something.Add(message.Message);

            return Task.CompletedTask;
        }

        public Task Handle(PrintSagaDataMessage message, IMessageHandlerContext context)
        {
            c.w("\n*************** PrintSagaData ***************");

            foreach (var data in Data.Something)
            {
                c.w(" - " + data);
            }

            c.w("*************** End ***************\n");

            return Task.CompletedTask;
        }

        public Task Handle(CompleteHelloWorldSagaMessage message, IMessageHandlerContext context)
        {
            c.w($"CompleteHelloWorldSaga");

            // Data.Id = message.Id;

            Data.Something.Add(message.Message);

            // code to handle order completion
            MarkAsComplete();

            return Task.CompletedTask;
        }
    }

    public class HelloWorldSagaData : ContainSagaData
    {
        public Guid MyId { get; set; }
        public IList<string> Something { get; set; }
    }
}
