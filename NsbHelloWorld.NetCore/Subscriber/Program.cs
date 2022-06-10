using System;
using NServiceBus;
using NServiceBus.Logging;
using Shared;

namespace Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            var _secrets = new SecretsReader();

            //logging
            var logging = LogManager.Use<DefaultFactory>();
            logging.Level(LogLevel.Info);

            //config
            //basic configurations
            var config = new EndpointConfiguration(Queues.SubscriberQueue);
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
            var transport = config.UseTransport<RabbitMQTransport>();
            transport.ConnectionString(() => _secrets.RabbitMQ_ConnectionString);

            //RabbitMq specific
            transport.UseDirectRoutingTopology();

            //bus
            var endpointInstance = Endpoint.Start(config).Result;

            Console.ReadLine();
        }
    }
}
