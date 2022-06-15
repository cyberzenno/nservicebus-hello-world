using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class Queues
    {
        public static string Prefix => "aaa";

        public static string ClientQueue => $"{Prefix}.client.queue_{Environment.MachineName}";

        public static string ServerQueue => $"{Prefix}.server.queue_{Environment.MachineName}";

        public static string DealerQueue => $"{Prefix}.dealer.queue_{Environment.MachineName}";

        public static string SubscriberQueue => $"{Prefix}.subscriber.queue_{Environment.MachineName}";

        public static string Error => $"{Prefix}.error";
        public static string Error_Machine => $"{Prefix}.error_{Environment.MachineName}";


        //--> Understanding Topology in Multitenant Scenarios
        public static string SimplePublisherQueue(string environment, string group)
        {
            return $"{environment}.{group}.simple-publisher.queue";
        }
        public static string SimplePublisherQueue_Machine(string environment, string group)
        {
            return $"{SimplePublisherQueue(environment, group)}_{Environment.MachineName}";
        }

        public static string SimpleSubscriberQueue(string environment, string group)
        {
            return $"{environment}.{group}.simple-subscriber.queue";
        }
        public static string SimpleSubscriberQueue_Machine(string environment, string group)
        {
            return $"{SimpleSubscriberQueue(environment, group)}_{Environment.MachineName}";
        }
    }
}
