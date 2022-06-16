using System;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Transport;
using Shared;


namespace SimplePublisher
{
    internal class Program
    {
        static string _environment;
        static string _group;

        static void Main(string[] args)
        {
            _environment = args[0] ?? "noEnv";
            _group = args[1] ?? "noGroup";

            var _secrets = new SecretsReader();

            //logging
            var logging = LogManager.Use<DefaultFactory>();
            logging.Level(LogLevel.Error);

            //config 
            //basic configurations
            var config = new EndpointConfiguration(Queues.SimplePublisherQueue(_environment, _group));


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
            Console.WriteLine("UP to publish Simple Event");
            Console.WriteLine("--------------------------------");

            Console.WriteLine("Press any key to exit");

            var j = 100;

            while (true)
            {
                var key = Console.ReadKey();
                Console.WriteLine();

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        PublishSimpleEvent(bus, j++);
                        continue;

                    default:
                        return;
                }

            }
        }

        private static void PublishSimpleEvent(IEndpointInstance bus, int j)
        {
            var simpleEvent = new SimpleEvent
            {
                Id = j,
                Message = $"Simple Event {j} ({_environment}.{_group} - {Environment.MachineName})",
            };

            var publishOptions = new PublishOptions();
            publishOptions.SetHeader(CustomHeaders.Environment, _environment);
            publishOptions.SetHeader(CustomHeaders.Group, _group);

            bus.Publish(simpleEvent, publishOptions).ConfigureAwait(false);
            Console.WriteLine("Published: " + simpleEvent.Message);
        }
    }
}
