using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System;
using System.IO;
using System.Reflection;
using WeTools.WorkerService.Options;
using System.Linq;
using WeTools.WorkerService.Attributes;

namespace WeTools.WorkerService
{
    public static class WeToolWorkerServiceExtensions
    {
        /// <summary>
        /// 注册json文件
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="jsonDir">json文件所在文件夹名称</param>
        public static IConfigurationBuilder AddJsonFiles(this IConfigurationBuilder builder, string jsonDir = "configs")
        {

            var jsonFiles = Directory.GetFiles(jsonDir, "*.json", SearchOption.AllDirectories);

            foreach (var item in jsonFiles)
            {
                builder.AddJsonFile(item, optional: true, reloadOnChange: true);
            }

            return builder;
        }

        /// <summary>
        /// 注册服务配置选项，需要配置WeTools:Workers节点
        /// </summary>
        /// <param name="services"></param>
        /// <param name="hostBuilder"></param>
        public static void AddServiceOptions(this IServiceCollection services, HostBuilderContext hostBuilder)
        {
            services.AddOptions<WeToolWorkersOption>().Bind(hostBuilder.Configuration.GetSection("WeTools")).ValidateDataAnnotations();
        }

        /// <summary>
        /// 注册服务配置选项，需要配置Workers节点
        /// </summary>
        /// <param name="services"></param>
        /// <param name="section">获取WeTools节点</param>
        public static void AddServiceOptions(this IServiceCollection services,IConfigurationSection section)
        {
            services.AddOptions<WeToolWorkersOption>().Bind(section).ValidateDataAnnotations();
        }

        /// <summary>
        /// 使用worker
        /// </summary>
        /// <param name="services"></param>
        public static void UseWorkers(this IServiceCollection services)
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var workerConfig = serviceProvider.GetRequiredService<IOptions<WeToolWorkersOption>>();

                StackTrace trace = new StackTrace();
                var frames = trace.GetFrames();
                var frame = frames.Last();
                MethodBase method = frame.GetMethod();
                string assFullName = method.ReflectedType.Assembly.FullName;
                string nameSpace = method.ReflectedType.Namespace;

                foreach (var item in workerConfig.Value.Workers)
                {
                    if (item.Enable)
                    {
                        string typeName = workerConfig.Value.Dir.Equals("/") ? $"{nameSpace}.{item.WorkerName},{assFullName}" : $"{nameSpace}.{workerConfig.Value.Dir}.{item.WorkerName},{assFullName}";
                        Type cls = Type.GetType(typeName);
                        Console.WriteLine($"获取到Worker类型{cls.FullName}");

                        services.AddSingleton(cls);
                        services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IHostedService), cls));
                    }
                }
            } 
        }

        /// <summary>
        /// 使用worker,自动解析WeTools节点下的Worker配置
        /// 使用此方法不需要再调用AddServiceOptions方法，也不需要配置Dir和Worker节点
        /// </summary>
        /// <param name="services"></param>
        /// <param name="hostBuilder"></param>
        public static void UseWorker(this IServiceCollection services, HostBuilderContext hostBuilder)
        {
            var section= hostBuilder.Configuration.GetSection($"WeTools");
            MakeWorker(services,section);
        }


        /// <summary>
        /// 使用worker
        /// 使用此方法不需要再调用AddServiceOptions方法，也不需要配置Dir和Worker节点
        /// </summary>
        /// <param name="services"></param>
        /// <param name="section">获取Worker配置的父节点，例如WeTools节点，程序会自动读取节点下worker的配置</param>
        public static void UseWorker(this IServiceCollection services, IConfigurationSection section)
        {
            MakeWorker(services, section);
        }

        private static void MakeWorker(IServiceCollection services, IConfigurationSection section,Func<string, IConfigurationSection> getSection=null)
        {
            var trace = new StackTrace();
            var frames = trace.GetFrames();
            var frame = frames.Last();
            var method = frame.GetMethod();
            var assembly = method.ReflectedType.Assembly;

            var types = assembly.GetTypes().Where(c => c.IsDefined(typeof(WorkerAttribute)) && c.BaseType == typeof(WeToolBackgroundService));

            foreach (var item in types)
            {
                var att = item.GetCustomAttribute<WorkerAttribute>();

                var config = getSection ==null? section.GetSection(att.Name): getSection(att.Name);

                if (config.Exists() && config.GetValue<bool>("Enable"))
                {
                    services.AddOptions<WeToolWorkerOption>(item.Name.ToLower()).Configure(op =>
                    {
                        op.Name = att.Name;
                        op.WorkerName = item.Name;

                    }).Bind(config).ValidateDataAnnotations();

                    Console.WriteLine($"获取到Worker类型{item.FullName}");
                    services.AddSingleton(item);
                    services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IHostedService), item));
                }
            }
        }

        /// <summary>
        /// 使用worker
        /// 使用此方法不需要再调用AddServiceOptions方法，也不需要配置Dir和Worker节点
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config">配置根节点</param>
        public static void UseWorker(this IServiceCollection services, IConfiguration config)
        {
            MakeWorker(services,null,p=>config.GetSection(p));
        }

    }
}
