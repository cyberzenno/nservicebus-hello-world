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
            var _secrets = new SecretsReader();

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
            //just download and add the file Shared\Secrets\ActualSecrets\License.xml
            config.License(_secrets.NServiceBus_License);

            //routing
            //routing is needed to tell which message goes where
            var transport = config.UseTransport<AzureServiceBusTransport>();
            transport.ConnectionString(() => _secrets.AzureServiceBus_ConnectionString);

            //RabbitMq specific
            //transport.UseDirectRoutingTopology();

            var routing = transport.Routing();

            routing.RouteToEndpoint(typeof(PlaceOrderMessage), Queues.ServerQueue);
            routing.RouteToEndpoint(typeof(StartHelloWorldSagaMessage), Queues.ServerQueue);
            routing.RouteToEndpoint(typeof(SendSomethingToSagaMessage), Queues.ServerQueue);
            routing.RouteToEndpoint(typeof(PrintSagaDataMessage), Queues.ServerQueue);
            routing.RouteToEndpoint(typeof(CompleteHelloWorldSagaMessage), Queues.ServerQueue);
            routing.RouteToEndpoint(typeof(ChainStartMessage), Queues.ServerQueue);
            routing.RouteToEndpoint(typeof(DelayMessage), Queues.ServerQueue);

            //conventions
            //conventions are used to define, precisely, conventions
            //instead of saying "hey, this class is a message", we can say "whatever class that ends with 'Message' is a message"
            //so on for events, commands, etc
            //for now, we don't use them

            var recoverabilityConfiguration = config.Recoverability();
            recoverabilityConfiguration.Immediate(set => set.NumberOfRetries(0));
            recoverabilityConfiguration.Delayed(set => set.NumberOfRetries(3).TimeIncrease(TimeSpan.FromSeconds(30)));

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
            Console.WriteLine("M, L, T, K to send a message to Mario, Luigi, Toad or Koopa Context");
            Console.WriteLine("--------------------------------");
            Console.WriteLine("S to delay sending message by few seconds");
            Console.WriteLine("D to delay sending message by many days");
            Console.WriteLine("--------------------------------");
            Console.WriteLine("A to send a failing chained message");
            Console.WriteLine("B to send a succeeding chained message");
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
                        SendMessage(bus, j++);
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


                    //------
                    case ConsoleKey.M:
                        SendMessageWithHeader(bus, "Mario", j++);
                        continue;
                    case ConsoleKey.L:
                        SendMessageWithHeader(bus, "Luigi", j++);
                        continue;
                    case ConsoleKey.T:
                        SendMessageWithHeader(bus, "Toad", j++);
                        continue;
                    case ConsoleKey.K:
                        SendMessageWithHeader(bus, "Koopa", j++);
                        continue;

                    //------
                    case ConsoleKey.S:
                        DelaySendingMessageSeconds(bus, j++);
                        continue;
                    case ConsoleKey.Y:
                        DelaySendingMessageDays(bus, j++);
                        continue;

                    //-------
                    case ConsoleKey.A:
                        SendChainMessage(bus, i++, j++, false);
                        continue;

                    case ConsoleKey.B:
                        SendChainMessage(bus, i++, j++, true);
                        continue;


                    default:
                        return;
                }

            }
        }

        private static void DelaySendingMessageSeconds(IEndpointInstance bus, int j, int secondsToDelay = 60)
        {
            var placeOrder = new PlaceOrderMessage
            {
                Id = j,
                Product = $"New shoes - Delayed by {secondsToDelay} seconds",
            };

            var options = new SendOptions();
            options.DelayDeliveryWith(TimeSpan.FromSeconds(secondsToDelay));

            bus.Send(placeOrder, options).ConfigureAwait(false);

            Console.WriteLine($"Sent PlaceOrder with {secondsToDelay} seconds delay {j}\n\n");
        }

        private static void DelaySendingMessageDays(IEndpointInstance bus, int j, int daysToDelay = 365)
        {
            var placeOrder = new PlaceOrderMessage
            {
                Id = j,
                Product = $"New shoes - Delayed by {daysToDelay} days",
            };

            var options = new SendOptions();
            options.DelayDeliveryWith(TimeSpan.FromDays(daysToDelay));

            bus.Send(placeOrder, options).ConfigureAwait(false);

            Console.WriteLine($"Sent PlaceOrder with {daysToDelay} days delay {j}\n\n");
        }



        private static void SendMessageWithHeader(IEndpointInstance bus, string contextMessageHeaderValue, int j)
        {
            var placeOrder = new PlaceOrderMessage
            {
                Id = j,
                Product = "New shoes for " + contextMessageHeaderValue,
            };

            var options = new SendOptions();
            options.SetHeader(CustomHeaders.DealerContext, contextMessageHeaderValue);

            bus.Send(placeOrder, options).ConfigureAwait(false);

            Console.WriteLine($"Sent PlaceOrder for {contextMessageHeaderValue} {j}\n\n");
        }

        private static void SendMessage(IEndpointInstance bus, int j)
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

        private static void SendChainMessage(
            IEndpointInstance bus,
            int i,
            int j,
            bool shouldSucceed
        )
        {
            var chain = new ChainStartMessage
            {
                Id = j,
                MessageShouldSucceed = shouldSucceed
            };
            bus.Send(chain).ConfigureAwait(false);
            Console.WriteLine($"Sent Chain {j} with success status {shouldSucceed}\n\n");
        }

    }
}
