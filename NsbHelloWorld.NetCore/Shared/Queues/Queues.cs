using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class Queues
    {
        public static string Prefix => "my.core"; 

        public static string ClientQueue => $"{Prefix}.client.queue_{System.Environment.MachineName}";
        public static string ClientQueueError => $"{Prefix}.{ClientQueue}.error";

        public static string ServerQueue =>$"{Prefix}.server.queue_{System.Environment.MachineName}";
        public static string ServerQueueError => $"{Prefix}.{ServerQueue}.error";

        public static string DealerQueue =>$"{Prefix}.dealer.queue_{System.Environment.MachineName}";
        public static string DealerQueueError => $"{Prefix}.{DealerQueue}.error";

        public static string SubscriberQueue =>$"{Prefix}.subscriber.queue_{System.Environment.MachineName}";
        public static string SubscriberQueueError => $"{Prefix}.{SubscriberQueue}.error";

        public static string Error => $"{Prefix}.error_{System.Environment.MachineName}";
    }
}
