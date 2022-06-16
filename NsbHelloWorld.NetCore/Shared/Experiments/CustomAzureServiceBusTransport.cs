using NServiceBus;
using NServiceBus.Settings;
using NServiceBus.Transport;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Experiments
{
    public class CustomAzureServiceBusTransport : AzureServiceBusTransport
    {
        public override TransportInfrastructure Initialize(SettingsHolder settings, string connectionString)
        {
            var originalInfrastructure = base.Initialize(settings, connectionString);
            return new CustomAzureServiceBusInfrastructure(originalInfrastructure, settings, connectionString);
        }
    }
}
