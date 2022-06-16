namespace Shared.Experiments
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus.Administration;
    using NServiceBus.Transport;

    class CustomNamespacePermissions
    {
        readonly ServiceBusAdministrationClient administrativeClient;

        public CustomNamespacePermissions(ServiceBusAdministrationClient administrativeClient)
        {
            this.administrativeClient = administrativeClient;
        }

        public async Task<StartupCheckResult> CanManage()
        {
            try
            {
                await administrativeClient.QueueExistsAsync("$nservicebus-verification-queue").ConfigureAwait(false);
            }
            catch (UnauthorizedAccessException)
            {
                return StartupCheckResult.Failed("Management rights are required to run this endpoint. Verify that the SAS policy has the Manage claim.");
            }
            catch (Exception exception)
            {
                return StartupCheckResult.Failed(exception.Message);
            }

            return StartupCheckResult.Success;
        }
    }
}