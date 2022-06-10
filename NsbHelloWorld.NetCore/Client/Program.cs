using System;
using NServiceBus;
using NServiceBus.Logging;
using Shared;

namespace Client
{
    class Program
    {
        static void Main()
        {
            //logging
            var logging = LogManager.Use<DefaultFactory>();
            logging.Level(LogLevel.Error);

            //config 
            //basic configurations
            var config = new EndpointConfiguration(Queues.ClientQueue);

            config.UseSerialization<NewtonsoftSerializer>();
            config.UsePersistence<InMemoryPersistence>();
            config.SendFailedMessagesTo(Queues.Error);

            //this is required only the first time you run the endpoint
            //in order to create the queues in Rabbit or any other trasnport
            config.EnableInstallers();

            //if the licence is not valid,
            //NSB will open the browser to get a Free License: https://particular.net/license/nservicebus?v=7.0.1&t=0&p=windows
            //just download and replace the file Shared\License\License.xml
            //(it works also without, but with few red errors and some limitations)
            var licensePath = License.Path();
            config.LicensePath(licensePath);

            //routing
            //routing is needed to tell which message goes where
            var transport = config.UseTransport<AzureServiceBusTransport>();
            transport.ConnectionString(() => Secrets.AzureServiceBus_ConnectionString);
            //transport.UseDirectRoutingTopology();

            var routing = transport.Routing();

            routing.RouteToEndpoint(typeof(PlaceOrderMessage), Queues.ServerQueue);
            routing.RouteToEndpoint(typeof(StartHelloWorldSagaMessage), Queues.ServerQueue);
            routing.RouteToEndpoint(typeof(SendSomethingToSagaMessage), Queues.ServerQueue);
            routing.RouteToEndpoint(typeof(PrintSagaDataMessage), Queues.ServerQueue);
            routing.RouteToEndpoint(typeof(CompleteHelloWorldSagaMessage), Queues.ServerQueue);

            //conventions
            //conventions are used to define, precisely, conventions
            //instead of saying "hey, this class is a message", we can say "whatever class that ends with 'Message' is a message"
            //so on for events, commands, etc
            //for now, we don't use them

            //bus
            var bus = Endpoint.Start(config).Result;
            Run(bus);
        }


        static void Run(IEndpointInstance bus)
        {
            //Console.WriteLine("X to delete all queues with [my.core.] prefix (recommended)");
            Console.WriteLine("--------------------------------");
            Console.WriteLine("UP to send a message");
            Console.WriteLine("DOWN to start Saga");
            Console.WriteLine("RIGHT to send message to Saga");
            Console.WriteLine("SPACE to print Saga");
            Console.WriteLine("LEFT to complete to Saga");
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Press any key to exit");

            var i = 0;
            var j = 100;

            var myId = Guid.NewGuid();

            while (true)
            {
                var key = Console.ReadKey();
                Console.WriteLine();

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        SendMessage(bus, i++, j++);
                        continue;

                    case ConsoleKey.DownArrow:
                        StartSaga(bus, myId);
                        continue;

                    case ConsoleKey.RightArrow:
                        SendToSaga(bus, myId);
                        continue;

                    case ConsoleKey.Spacebar:
                        PrintSagaData(bus, myId);
                        continue;

                    case ConsoleKey.LeftArrow:
                        CompleteSaga(bus, myId);
                        continue;

                    default:
                        return;
                }

            }
        }

        private static void SendMessage(IEndpointInstance bus, int i, int j)
        {
            var placeOrder = new PlaceOrderMessage
            {
                Id = j,
                Product = "New shoes",
            };
            bus.Send(placeOrder).ConfigureAwait(false);
            Console.WriteLine($"Sent PlaceOrder {j}\n\n");
        }

        private static void StartSaga(IEndpointInstance bus, Guid myGuid)
        {
            c.w("Start Saga " + myGuid);

            bus.Send(new StartHelloWorldSagaMessage()
            {
                Id = myGuid,
                Message = "Client send to Start Saga at " + DateTime.Now.ToString("hh : mm : ss")
            }).ConfigureAwait(false);
        }

        private static void SendToSaga(IEndpointInstance bus, Guid myGuid)
        {
            c.w("Send to Saga");

            bus.Send(new SendSomethingToSagaMessage()
            {
                Id = myGuid,
                Message = "Client send to Saga at " + DateTime.Now.ToString("hh : mm : ss")
            }).ConfigureAwait(false);
        }

        private static void PrintSagaData(IEndpointInstance bus, Guid myGuid)
        {
            c.w("Print Saga Data");

            bus.Send(new PrintSagaDataMessage()
            {
                Id = myGuid,
            }).ConfigureAwait(false);
        }

        private static void CompleteSaga(IEndpointInstance bus, Guid myGuid)
        {
            c.w("Complete Saga");

            bus.Send(new CompleteHelloWorldSagaMessage()
            {
                Id = myGuid,
                Message = "Client send to Stop Saga at " + DateTime.Now.ToString("hh : mm : ss")
            }).ConfigureAwait(false);
        }
    }
}
