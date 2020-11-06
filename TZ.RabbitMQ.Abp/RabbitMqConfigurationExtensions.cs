using System.IO;
using Abp.Configuration.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TZ.RabbitMQ.Client;

namespace TZ.RabbitMQ.Abp
{
    public static class RabbitMqConfigurationExtensions
    {
        public static IOptions<RabbitMQClientOptions> ConfigRabbitMqClient(this IModuleConfigurations configurations)
        {
            var configuration = AppConfigurations.Get(Directory.GetCurrentDirectory());
            //var configuration = GlobalConfiguration.Configuration;
            var rabbitMqClientOptions = configurations.AbpConfiguration.Get<IOptions<RabbitMQClientOptions>>();

            rabbitMqClientOptions.Value.Host = configuration.GetValue<string>("RabbitMQClient:Host");
            rabbitMqClientOptions.Value.Port = configuration.GetValue<int>("RabbitMQClient:Port");
            rabbitMqClientOptions.Value.VirtualHost = configuration.GetValue<string>("RabbitMQClient:VirtualHost");
            rabbitMqClientOptions.Value.Username = configuration.GetValue<string>("RabbitMQClient:Username");
            rabbitMqClientOptions.Value.Password = configuration.GetValue<string>("RabbitMQClient:Password");
            return rabbitMqClientOptions;
        }
    }
}