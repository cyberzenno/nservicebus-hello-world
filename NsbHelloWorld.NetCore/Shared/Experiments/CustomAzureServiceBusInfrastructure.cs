using NServiceBus;
using NServiceBus.Routing;
using NServiceBus.Transport;
using System;
using System.Collections.Generic;
using System.Text;
using NServiceBus.Transport.AzureServiceBus;
using NServiceBus.Settings;
using Azure.Messaging.ServiceBus.Administration;
using Azure.Messaging.ServiceBus;
using Azure.Core;
using System.Text.RegularExpressions;

namespace Shared.Experiments
{
    public class CustomAzureServiceBusInfrastructure : TransportInfrastructure
    {
        const string defaultTopicName = "bundle-1";
        static readonly Func<string, string> defaultNameShortener = name => name;
        static readonly Func<string, string> defaultSubscriptionNamingConvention = name => name;
        static readonly Func<Type, string> defaultSubscriptionRuleNamingConvention = type => type.FullName;

        static readonly Func<string, (string, string)> defaultEnvironmentAndGroupConvention = name =>
        {
            var regEx = new Regex("[a-zA-Z0-9_]+");
            var matches = regEx.Matches(name);

            var env = matches[0].Value;
            var group = matches[1].Value;

            return (env, group);
        };

        readonly SettingsHolder settings;
        readonly ServiceBusAdministrationClient administrationClient;
        readonly string topicName;
        readonly CustomNamespacePermissions namespacePermissions;
        readonly TokenCredential tokenCredential;

        private readonly TransportInfrastructure OriginalInfrastructure;

        public CustomAzureServiceBusInfrastructure(TransportInfrastructure originalInfrastructure, SettingsHolder settings, string connectionString)
        {
            OriginalInfrastructure = originalInfrastructure;

            this.settings = settings;
 
            if (!settings.TryGet(CustomSettingsKeys.TopicName, out topicName))
            {
                topicName = defaultTopicName;
            }
            _ = settings.TryGet(CustomSettingsKeys.CustomTokenCredential, out TokenCredential tc);
            tokenCredential = tc;

            administrationClient = tokenCredential != null
                ? new ServiceBusAdministrationClient(connectionString, tokenCredential)
                : new ServiceBusAdministrationClient(connectionString);

            namespacePermissions = new CustomNamespacePermissions(administrationClient);
        }

        public override IEnumerable<Type> DeliveryConstraints => OriginalInfrastructure.DeliveryConstraints;

        public override TransportTransactionMode TransactionMode => OriginalInfrastructure.TransactionMode;

        public override OutboundRoutingPolicy OutboundRoutingPolicy => OriginalInfrastructure.OutboundRoutingPolicy;

        public override EndpointInstance BindToLocalEndpoint(EndpointInstance instance)
        {
            return OriginalInfrastructure.BindToLocalEndpoint(instance);
        }

        public override TransportReceiveInfrastructure ConfigureReceiveInfrastructure()
        {
            return OriginalInfrastructure.ConfigureReceiveInfrastructure();
        }

        public override TransportSendInfrastructure ConfigureSendInfrastructure()
        {
            return OriginalInfrastructure.ConfigureSendInfrastructure();
        }

        public override TransportSubscriptionInfrastructure ConfigureSubscriptionInfrastructure()
        {
            return new TransportSubscriptionInfrastructure(() => CreateSubscriptionManager());
        }

        CustomSubscriptionManager CreateSubscriptionManager()
        {
            if (!settings.TryGet(CustomSettingsKeys.SubscriptionNameShortener, out Func<string, string> subscriptionNameShortener))
            {
                subscriptionNameShortener = defaultNameShortener;
            }

            if (!settings.TryGet(CustomSettingsKeys.RuleNameShortener, out Func<string, string> ruleNameShortener))
            {
                ruleNameShortener = defaultNameShortener;
            }

            if (!settings.TryGet(CustomSettingsKeys.SubscriptionNamingConvention, out Func<string, string> subscriptionNamingConvention))
            {
                subscriptionNamingConvention = defaultSubscriptionNamingConvention;
            }

            if (!settings.TryGet(CustomSettingsKeys.SubscriptionRuleNamingConvention, out Func<Type, string> ruleNamingConvention))
            {
                ruleNamingConvention = defaultSubscriptionRuleNamingConvention;
            }

            if (!settings.TryGet(CustomSettingsKeys.EnvironmentAndGroupConvention, out Func<string, (string, string)> environmentAndGroupConvention))
            {
                environmentAndGroupConvention = defaultEnvironmentAndGroupConvention;
            }

            return new CustomSubscriptionManager(
                settings.LocalAddress(), 
                topicName, 
                administrationClient,
                namespacePermissions,
                subscriptionNameShortener,
                ruleNameShortener, 
                subscriptionNamingConvention,
                ruleNamingConvention,
                environmentAndGroupConvention);
        }

        public override string ToTransportAddress(LogicalAddress logicalAddress)
        {
            return OriginalInfrastructure.ToTransportAddress(logicalAddress);
        }
    }
}
