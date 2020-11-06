using System.Collections.Concurrent;
using System.Reflection;
using Abp.Extensions;
using Abp.Reflection.Extensions;
using Microsoft.Extensions.Configuration;

namespace TZ.RabbitMQ.Abp
{
    /// <summary>
    /// 应用程序配置加载类.
    /// 此类为静态类,统一处理不同容器的配置初始化过程.
    /// WebHost/Ef tools/Unit Test/都有不同的加载逻辑.
    /// </summary>
    public static class AppConfigurations
    {
        private static readonly ConcurrentDictionary<string, IConfigurationRoot> ConfigurationCache;

        static AppConfigurations()
        {
            ConfigurationCache = new ConcurrentDictionary<string, IConfigurationRoot>();
        }

        /// <summary>
        /// 从指定的文件夹加载配置
        /// </summary>
        /// <param name="path">指定的文件夹</param>
        /// <param name="environmentName">环境名</param>
        /// <param name="addUserSecrets">是否加载用户机密</param>
        /// <param name="secretAssembly">用户机密所在程序集</param>
        /// <returns></returns>
        public static IConfigurationRoot Get(string path, string environmentName = null, bool addUserSecrets = false, Assembly secretAssembly = null)
        {
            // 做个缓存 防止每次都去加载
            var cacheKey = path + "#" + environmentName + "#" + addUserSecrets;
            return ConfigurationCache.GetOrAdd(cacheKey,
                _ => BuildConfiguration(path, environmentName, addUserSecrets, secretAssembly)
            );
        }

        /// <summary>
        /// 常规配置加载方式.包括以下来源:
        /// <para>从指定的文件夹中的 appsettings.json</para>
        /// <para>appsettings.{environmentName}.json</para>
        /// <para>EnvironmentVariables</para>
        /// <para>UserSecrets</para>
        /// </summary>
        /// <param name="path">appsettings.json所在的文件夹</param>
        /// <param name="environmentName">当前环境名</param>
        /// <param name="addUserSecrets">是否加载用户机密</param>
        /// <param name="secretAssembly">用户机密所在的程序集</param>
        /// <returns></returns>
        private static IConfigurationRoot BuildConfiguration(string path, string environmentName = null, bool addUserSecrets = false, Assembly secretAssembly = null)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            if (!environmentName.IsNullOrWhiteSpace())
            {
                builder = builder.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
            }
            
            builder = builder.AddEnvironmentVariables();

            if (addUserSecrets && secretAssembly != null)
            {
                builder.AddUserSecrets(secretAssembly);
            }
            
            return builder.Build();
        }
    }
}