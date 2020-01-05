using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AspectCore.Extensions.Autofac;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TZ.RabbitMQ.Client;

namespace TZ.RabbitMQ.Demo
{
    class Program
    {

        private static IServiceProvider ServiceProvider { get; set; }
        private static IContainer ApplicationContainer { get; set; }
        private static IServiceCollection ServiceCollections { get; set; }

        private static IConfiguration Configuration { get; set; }

        static void Main(string[] args)
        {
            SetConfiguration();
            ServiceCollections = new ServiceCollection();
            ConfigureServices(ServiceCollections);
            //var queueName = "queue_test_workqueues";
            var queueName = "test_delayqueues";
            if (Configuration.GetValue<bool>("RunProducer"))
            {
                var producer = ServiceProvider.GetService<RabbitMQProducer>();
                producer.Open();
                for (int i = 0; i < 500; i++)
                {
                    //producer.SendWorkQueues("hello"+i,queueName);
                    producer.SendDelayQueues("这是延迟消息"+i,queueName,1000*10+i*1000);
                }
                producer.Close();
            }
            if (Configuration.GetValue<bool>("RunConsumer"))
            {
                var doSeconds = 2;
                ushort prefetchCount = 1;
                var consumer = ServiceProvider.GetService<RabbitMQConsumer>();
                consumer.Open();
                /*consumer.SetWorkQueuesReceivedAction((msg) =>
                {
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss")+" 接收到消息："+msg);
                    Thread.Sleep(doSeconds * 100 * GetRandomNum(1, 50));
                },queueName,prefetchCount,consumerCount:20);*/

                consumer.SetDelayQueuesReceivedAction((msg) =>
                {
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss")+" 1接收到消息："+msg);
                    Thread.Sleep(doSeconds*100*GetRandomNum(1,50));
                    /*var str =  GetData().Result;
                    Console.WriteLine(str);*/
                },queueName,prefetchCount,consumerCount:20);
                Console.WriteLine("启动完毕！");
            }
            Console.ReadKey();
        }

        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        static int GetRandomNum(int minValue, int maxValue)
        {
            long tick = DateTime.Now.Ticks;
            Random ran = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
            int result = ran.Next(minValue, maxValue);
            System.Threading.Thread.Sleep(1);
            return result;
        }

        static void SetConfiguration()
        {

            #region 配置文件

            //在当前目录或者根目录中寻找appsettings.json文件
            var fileName = "appsettings.json";

            var directory = AppContext.BaseDirectory;
            directory = directory.Replace("\\", "/");

            var filePath = $"{directory}/{fileName}";
            if (!File.Exists(filePath))
            {
                var length = directory.IndexOf("/bin");
                filePath = $"{directory.Substring(0, length)}/{fileName}";
            }

            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile(filePath, false, true);
            Configuration = configurationBuilder.Build();
            #endregion
        }
        static void ConfigureServices(IServiceCollection services)
        {
            #region MQ
            services.AddRabbitMQClient(Configuration);
            #endregion

            #region 注入容器
            var containerBuilder = new ContainerBuilder();//实例化 AutoFac  容器
            containerBuilder.RegisterDynamicProxy();//注册AOP动态代理，目前使用AspectCore//模块化注入，默认注入模块
            containerBuilder.Populate(services);//管道寄居
            containerBuilder.RegisterModule<DefaultRegisterModule>();
            ApplicationContainer = containerBuilder.Build();//IUserService UserService 构造 
            #endregion

            ServiceProvider = new AutofacServiceProvider(ApplicationContainer);//将autofac反馈到管道中
        }

        static async Task<string> GetData()
        {
            var url = "https://www.baidu.com";
            var client = new HttpClient { BaseAddress = new Uri(url) };
            /*var content = new StringContent("");
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");*/
            HttpResponseMessage response =await client.GetAsync("");
            string responseStr =await response.Content.ReadAsStringAsync();
            return responseStr;
        }
    }
}
