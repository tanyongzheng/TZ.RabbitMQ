using System;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace TZ.RabbitMQ.Client
{
    public class RabbitMQClientBase : IDisposable
    {

        protected ConnectionFactory ConnectionFactory;

        protected IConnection Connection;

        protected readonly RabbitMQClientOptions RabbitMqOptions;

        //private string ConnectionString;
        /// <summary>
        /// 构造函数从配置环境中取RabbitMQClientOptions
        /// </summary>
        public RabbitMQClientBase(IOptions<RabbitMQClientOptions> options)
        {
            if (options == null || options.Value == null)
            {
                throw new Exception("please set RabbitMQClientOptions!");
            }
            else if (options.Value != null)
            {
                RabbitMqOptions = options.Value;
            }

            #region ConnectionString
            /*
            if (optionValue != null)
            {
                ConnectionString =
                    $"host={optionValue.Host};port={optionValue.Port};virtualHost={optionValue.VirtualHost};username={optionValue.Username};password={optionValue.Password}";
            }
            else
            {
                var connStrArray = ConnectionString.Split(';');
                optionValue = new RabbitMQClientOptions();
                foreach (var item in connStrArray)
                {
                    if (item.ToLower().StartsWith("host="))
                    {
                        optionValue.Host = item.ToLower().Replace("host=", "");
                    }
                    if (item.ToLower().StartsWith("port="))
                    {
                        optionValue.Port = Convert.ToInt32(item.ToLower().Replace("port=", ""));
                    }
                    if (item.ToLower().StartsWith("virtualhost="))
                    {
                        optionValue.VirtualHost = item.ToLower().Replace("virtualhost=", "");
                    }
                    if (item.ToLower().StartsWith("username="))
                    {
                        optionValue.Username = item.ToLower().Replace("username=", "");
                    }
                    if (item.ToLower().StartsWith("password="))
                    {
                        optionValue.Password = item.ToLower().Replace("password=", "");
                    }
                }
            } 
            */
            #endregion

        }

        /// <summary>
        /// 打开连接
        /// </summary>
        public void Open()
        {
            if (ConnectionFactory == null)
                ConnectionFactory = new ConnectionFactory()
                {
                    HostName = RabbitMqOptions.Host,
                    Port = RabbitMqOptions.Port,
                    VirtualHost = RabbitMqOptions.VirtualHost,
                    UserName = RabbitMqOptions.Username,
                    Password = RabbitMqOptions.Password
                };
            ConnectionFactory.AutomaticRecoveryEnabled = true; //自动重连
            if (Connection == null)
                Connection = ConnectionFactory.CreateConnection();
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            Connection?.Close();
        }

        public void Dispose()
        {
            Close();
        }

    }
}