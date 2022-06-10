using System;
using NServiceBus;
using NServiceBus.Logging;
using Shared;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var _secrets = new SecretsReader();

            //logging
            var logging = LogManager.Use<DefaultFactory>();
            logging.Level(LogLevel.Warn);

            //config
            //basic configurations
            var config = new EndpointConfiguration(Queues.ServerQueue);
            config.UseSerialization<NewtonsoftSerializer>();
            config.UsePersistence<InMemoryPersistence>();
            config.SendFailedMessagesTo(Queues.Error);

            //this is required only the first time you run the endpoint
            //in order to create the queues in Rabbit or any other trasnport
            config.EnableInstallers();

            //if the licence is not valid,
            //NSB will open the browser to get a Free License: https://particular.net/license/nservicebus?v=7.0.1&t=0&p=windows
            //just download and add the file Shared\Secrets\License.xml
            config.License(_secrets.NServiceBus_License);

            //routing
            //routing is needed to tell which message goes where
            var transport = config.UseTransport<AzureServiceBusTransport>();
            transport.ConnectionString(() => _secrets.AzureServiceBus_ConnectionString);

            //RabbitMq specific
            //transport.UseDirectRoutingTopology();

            var routing = transport.Routing();
           
            //important note: one even can be published to multiple queues
            //and we need to use Publish instead of Send
            routing.RouteToEndpoint(typeof(OrderPlacedEvent), Queues.DealerQueue);
            routing.RouteToEndpoint(typeof(OrderPlacedEvent), Queues.SubscriberQueue);

            //conventions
            //conventions are used to define, precisely, conventions
            //instead of saying "hey, this class is a message", we can say "whatever class that ends with 'Message' is a message"
            //so on for events, commands, etc
            //for now, we don't use them

            //bus
            var endpointInstance = Endpoint.Start(config).Result;

            Console.ReadLine();
        }
    }
}
