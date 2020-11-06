using System;
using System.IO;
using Abp;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Options;
using TZ.RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TZ.RabbitMQ.Abp
{

    [DependsOn(typeof(AbpKernelModule))]
    public class RabbitMqModule : AbpModule
    {
        private readonly IConfiguration _configuration;

        public RabbitMqModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(RabbitMqModule).GetAssembly());

            //生产者单例注入，也可以在.net core启动类的ConfigureServices(IServiceCollection services)方法中配置
            IocManager.IocContainer.Register(Component.For(typeof(RabbitMQProducer))
                .ImplementedBy(typeof(RabbitMQProducer))
                .LifeStyle.Singleton);

            //消费者单例注入，也可以在.net core启动类的ConfigureServices(IServiceCollection services)方法中配置
            IocManager.IocContainer.Register(Component.For(typeof(RabbitMQConsumer))
                .ImplementedBy(typeof(RabbitMQConsumer))
                .LifeStyle.Singleton);
        }

        public override void PostInitialize()
        {
            //Abp中配置Rabbit客户端，也可以在.net core启动类的ConfigureServices(IServiceCollection services)方法中配置
            Configuration.Modules.AbpConfiguration.Modules.ConfigRabbitMqClient();
            /*
            IOptions<RabbitMQClientOptions> rabbitMqClientOptions = Configuration.Modules.AbpConfiguration.Get<IOptions<RabbitMQClientOptions>>();
            //var configuration= AppConfigurations.Get(Directory.GetCurrentDirectory());
            rabbitMqClientOptions.Value.Host = _configuration.GetValue<string>("RabbitMQClient:Host");
            rabbitMqClientOptions.Value.Port = _configuration.GetValue<int>("RabbitMQClient:Port");
            rabbitMqClientOptions.Value.VirtualHost = _configuration.GetValue<string>("RabbitMQClient:VirtualHost");
            rabbitMqClientOptions.Value.Username = _configuration.GetValue<string>("RabbitMQClient:Username");
            rabbitMqClientOptions.Value.Password = _configuration.GetValue<string>("RabbitMQClient:Password");
            */
        }

    }


}

