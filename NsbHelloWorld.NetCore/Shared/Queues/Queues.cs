using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class Queues
    {
        public static string Prefix => "xxx";

        public static string ClientQueue => $"{Prefix}.client.queue";

        public static string ServerQueue => $"{Prefix}.server.queue";

        public static string DealerQueue => $"{Prefix}.dealer.queue";

        public static string SubscriberQueue => $"{Prefix}.subscriber.queue";

        public static string Error => $"{Prefix}.error";
        public static string Error_Machine => $"{Prefix}.error";


        //--> Understanding Topology in Multitenant Scenarios
        public static string SimplePublisherQueue(string environment, string group)
        {
            return $"{environment}.{group}.simple-publisher.queue";
        }
        public static string SimplePublisherQueue_Machine(string environment, string group)
        {
            return $"{SimplePublisherQueue(environment, group)}";
        }

        public static string SimpleSubscriberQueue(string environment, string group)
        {
            return $"{environment}.{group}.simple-subscriber.queue";
        }
        public static string SimpleSubscriberQueue_Machine(string environment, string group)
        {
            return $"{SimpleSubscriberQueue(environment, group)}";
        }
    }
}
