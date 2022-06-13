using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


namespace Shared
{
    public class SecretsReader
    {
        readonly IConfiguration configuration;

        readonly string licenseXmlAsPlainText;

        public SecretsReader()
        {
            var secretsJson ="Secrets\\ActualSecrets\\secrets.json";

            var builder = new ConfigurationBuilder();
            configuration = builder
                 .AddJsonFile(secretsJson, false, true)
                 .Build();

            var licenseXml = "Secrets\\ActualSecrets\\License.xml";
            licenseXmlAsPlainText = File.ReadAllText(licenseXml);
        }

        public string AzureServiceBus_ConnectionString => configuration["AzureServiceBus_ConnectionString"];

        public string RabbitMQ_ConnectionString => configuration["RabbitMQ_ConnectionString"];

        public string NServiceBus_License => licenseXmlAsPlainText;
    }
}
