using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeTools.WorkerService;
using WeTools.WorkerService.Attributes;
using WeTools.WorkerService.Options;

namespace WorkerServiceDemo
{
    [Worker("DemoWorker2")]
    public class Worker2 : WeToolBackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker2(ILogger<Worker> logger, IServiceProvider rootProvider, IOptionsMonitor<WeToolWorkerOption> workerOption) :base(rootProvider,logger,workerOption)
        {
            _logger = logger;
        }

        int id=0;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("Worker2执行了");

                await Task.Delay(5000, stoppingToken);

            }
            Console.WriteLine("退出执行了");
        }
    }
}
