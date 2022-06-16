﻿namespace Shared.Experiments
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus.Extensibility;
    using Azure.Messaging.ServiceBus.Administration;
    using Azure.Messaging.ServiceBus;
    using NServiceBus.Transport;
    using NServiceBus;

    class CustomSubscriptionManager : IManageSubscriptions
    {
        const int maxNameLength = 50;

        readonly string topicPath;
        readonly ServiceBusAdministrationClient administrationClient;
        readonly CustomNamespacePermissions namespacePermissions;
        readonly Func<string, string> ruleShortener;
        readonly Func<Type, string> subscriptionRuleNamingConvention;
        readonly string subscriptionName;

        readonly string environmentName;
        readonly string groupName;

        StartupCheckResult startupCheckResult;

        public CustomSubscriptionManager(string inputQueueName, string topicPath,
            ServiceBusAdministrationClient administrationClient,
            CustomNamespacePermissions namespacePermissions,
            Func<string, string> subscriptionShortener,
            Func<string, string> ruleShortener,
            Func<string, string> subscriptionNamingConvention,
            Func<Type, string> subscriptionRuleNamingConvention,
            Func<string, (string, string)> environmentAndGroupConvention
            )
        {
            this.topicPath = topicPath;
            this.administrationClient = administrationClient;
            this.namespacePermissions = namespacePermissions;
            this.ruleShortener = ruleShortener;
            this.subscriptionRuleNamingConvention = subscriptionRuleNamingConvention;

            subscriptionName = subscriptionNamingConvention(inputQueueName);
            subscriptionName = subscriptionName.Length > maxNameLength ? subscriptionShortener(subscriptionName) : subscriptionName;


            var envGroupTuple = environmentAndGroupConvention(inputQueueName);
            environmentName = envGroupTuple.Item1;
            groupName = envGroupTuple.Item2;
        }

        public async Task Subscribe(Type eventType, ContextBag context)
        {
            await CheckForManagePermissions().ConfigureAwait(false);

            var ruleName = subscriptionRuleNamingConvention(eventType);
            ruleName = ruleName.Length > maxNameLength ? ruleShortener(ruleName) : ruleName;
            var sqlExpression = $"[{Headers.EnclosedMessageTypes}] LIKE '%{eventType.FullName}%' AND Environment = '{environmentName}' AND Group = '{groupName}' AND 'Custom' = 'Custom'";
            var rule = new CreateRuleOptions(ruleName, new SqlRuleFilter(sqlExpression));

            try
            {
                var existingRule = await administrationClient.GetRuleAsync(topicPath, subscriptionName, rule.Name).ConfigureAwait(false);

                if (existingRule.Value.Filter.ToString() != rule.Filter.ToString())
                {
                    rule.Action = existingRule.Value.Action;

                    await administrationClient.UpdateRuleAsync(topicPath, subscriptionName, existingRule).ConfigureAwait(false);
                }
            }
            catch (ServiceBusException sbe) when (sbe.Reason == ServiceBusFailureReason.MessagingEntityNotFound)
            {
                try
                {
                    await administrationClient.CreateRuleAsync(topicPath, subscriptionName, rule).ConfigureAwait(false);
                }
                catch (ServiceBusException createSbe) when (createSbe.Reason == ServiceBusFailureReason.MessagingEntityAlreadyExists)
                {
                }
            }
        }

        public async Task Unsubscribe(Type eventType, ContextBag context)
        {
            await CheckForManagePermissions().ConfigureAwait(false);

            var ruleName = subscriptionRuleNamingConvention(eventType);
            ruleName = ruleName.Length > maxNameLength ? ruleShortener(ruleName) : ruleName;

            try
            {
                await administrationClient.DeleteRuleAsync(topicPath, subscriptionName, ruleName).ConfigureAwait(false);
            }
            catch (ServiceBusException sbe) when (sbe.Reason == ServiceBusFailureReason.MessagingEntityNotFound)
            {
            }
        }

        async Task CheckForManagePermissions()
        {
            if (startupCheckResult == null)
            {
                startupCheckResult = await namespacePermissions.CanManage().ConfigureAwait(false);
            }

            if (!startupCheckResult.Succeeded)
            {
                throw new Exception(startupCheckResult.ErrorMessage);
            }
        }
    }
}