using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TZ.RabbitMQ.Client;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace TZ.RabbitMQ.AbpVnext
{
    public class RabbitMqModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
             var configuration=context.Services.GetConfiguration();
             
            var enabledRabbitMq= configuration.GetValue<bool>("EnabledRabbitMq");
            if (enabledRabbitMq)
            {
                //配置RabbitMq
                context.Services.AddRabbitMQClient(opt =>
                {
                   opt.Host = configuration.GetValue<string>("RabbitMQClient:Host");
                   opt.Port = configuration.GetValue<int>("RabbitMQClient:Port");
                   opt.VirtualHost = configuration.GetValue<string>("RabbitMQClient:VirtualHost");
                   opt.Username = configuration.GetValue<string>("RabbitMQClient:Username");
                    opt.Password = configuration.GetValue<string>("RabbitMQClient:Password");
                });
                /*
                Configure<RabbitMQClientOptions>(opt =>
                {
                   opt.Host = configuration.GetValue<string>("RabbitMQClient:Host");
                   opt.Port = configuration.GetValue<int>("RabbitMQClient:Port");
                   opt.VirtualHost = configuration.GetValue<string>("RabbitMQClient:VirtualHost");
                   opt.Username = configuration.GetValue<string>("RabbitMQClient:Username");
                    opt.Password = configuration.GetValue<string>("RabbitMQClient:Password");
                });
                */
                //单例注入
                context.Services.AddSingleton<RabbitMQProducer>();
            }
        }

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var configuration = context.ServiceProvider.GetService<IConfiguration>();
            /*
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            */
            var enabledRabbitMq= configuration.GetValue<bool>("EnabledRabbitMq");
            if (enabledRabbitMq)
            {
                var mqProducer = context.ServiceProvider.GetService<RabbitMQProducer>();
                mqProducer.SendWorkQueues("hello word", "queue-1");
            }
        }
    }
}