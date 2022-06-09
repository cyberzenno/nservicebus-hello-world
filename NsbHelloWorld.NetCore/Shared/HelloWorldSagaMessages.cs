using System;
using NServiceBus;

namespace Shared 
{
    public class StartHelloWorldSagaMessage : IMessage
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
    }
   
    public class SendSomethingToSagaMessage : IMessage
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
    }
   
    public class PrintSagaDataMessage : IMessage
    {
        public Guid Id { get; set; }
    }
   
    public class CompleteHelloWorldSagaMessage : IMessage
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
    }
}
