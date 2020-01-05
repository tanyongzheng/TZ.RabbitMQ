using Autofac;
using TZ.RabbitMQ.Client;

namespace TZ.RabbitMQ.Demo
{

    /// <summary>
    /// 模块化注入，默认注入类型
    /// </summary>
    public class DefaultRegisterModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RabbitMQProducer>().SingleInstance();
            builder.RegisterType<RabbitMQConsumer>().SingleInstance();
        }
    }
}