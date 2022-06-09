using System;
using NServiceBus;
using NServiceBus.Logging;
using Shared;

namespace Dealer
{
    class Program
    {
        static void Main(string[] args)
        {
            //logging
            var logging = LogManager.Use<DefaultFactory>();
            logging.Level(LogLevel.Info);

            //config
            //basic configurations
            var config = new EndpointConfiguration("my.core.dealer.queue");
            config.UseSerialization<NewtonsoftSerializer>();
            config.UsePersistence<InMemoryPersistence>();
            config.SendFailedMessagesTo("my.core.dealer.queue.error");

            //this is required only the first time you run the endpoint
            //in order to create the queues in Rabbit or any other trasnport
            config.EnableInstallers();

            //if the licence is not valid,
            //NSB will open the browser to get a Free License: https://particular.net/license/nservicebus?v=7.0.1&t=0&p=windows
            //just download and replace the file Shared\License\License.xml
            var licensePath = License.Path();
            config.LicensePath(licensePath);

            //routing
            //routing is needed to tell which message goes where
            var transport = config.UseTransport<AzureServiceBusTransport>();
            transport.ConnectionString(() => Secrets.AzureServiceBus_ConnectionString);
            //transport.UseDirectRoutingTopology();

            //bus
            var endpointInstance = Endpoint.Start(config).Result;

            Console.ReadLine();
        }
    }
}
