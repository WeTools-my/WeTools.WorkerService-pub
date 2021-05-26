using Microsoft.Extensions.Hosting;
using WeTools.WorkerService;

namespace WorkerServiceDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    //services.AddServiceOptions(hostContext);

                    //services.UseWorker(hostContext.Configuration);
                    //services.UseWorker(hostContext);
                    services.UseWorker(hostContext.Configuration.GetSection("WeTools2"));

                    //services.AddOptions<WeToolWorkerOption>().Bind(hostContext.Configuration.GetSection("Service"));
                    //services.AddHostedService<Worker>();
                    //services.AddHostedService<Worker2>();
                });
    }
}
