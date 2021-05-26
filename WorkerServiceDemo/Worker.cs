using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using WeTools.WorkerService;
using WeTools.WorkerService.Attributes;
using WeTools.WorkerService.Options;

namespace WorkerServiceDemo
{
    [Worker("DemoWorker")]
    public class Worker : WeToolBackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger, IOptionsMonitor<WeToolWorkerOption> workerOption) :base(workerOption)
        {
            _logger = logger;
        }

        int id=0;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("Workerִ����");

                await Task.Delay(5000, stoppingToken);

            }
            Console.WriteLine("�˳�ִ����");
        }
    }
}
