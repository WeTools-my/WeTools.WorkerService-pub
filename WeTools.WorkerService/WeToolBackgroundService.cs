using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeTools.WorkerService.Model;
using WeTools.WorkerService.Options;

namespace WeTools.WorkerService
{
    #region
    //StackTrace trace = new();
    //var frames = trace.GetFrames().ToList();
    //var frame = frames.Where(c => c.GetMethod().ReflectedType.GetInterface("IGroupWorker") is not null).First();
    //MethodBase method = frame.GetMethod();
    //_clsName = method.ReflectedType.Name;
    //Console.WriteLine($"Telegram-bot调用者：{_clsName}");
    #endregion

    #region
    //_timer = new Timer(p => {
    //    CancellationTokenSource cts = new CancellationTokenSource(0);

    //    cts.Token.Register(() => Console.WriteLine("任务已取消!"));

    //    StopAsync(cts.Token);

    //},null,10000, 10000000);

    //_timer2 = new Timer(p => {
    //    CancellationTokenSource cts = new CancellationTokenSource(); 
    //    StartAsync(cts.Token); }, null, 15000, 1000000);

    #endregion
    public abstract class WeToolBackgroundService : BackgroundService
    {
        private string _currentWorkerClassName = null;
        private readonly IServiceScope _scope;
        public IServiceProvider Provider { get; }

        public ILogger<WeToolBackgroundService> Logger { get; }


        public WeToolWorkerConfig WorkerConfig { get; set; }

        public WeToolBackgroundService() { }

        #region workers
        public WeToolBackgroundService(IServiceProvider rootProvider)
        {
            var scop = rootProvider.CreateScope();
            Provider = scop.ServiceProvider;
            
        }
        public WeToolBackgroundService(IOptionsMonitor<WeToolWorkersOption> workersOption)
        {
            InitConfig(workersOption);
            ChangeConfig(workersOption);
        }

        public WeToolBackgroundService(ILogger<WeToolBackgroundService> logger)
        {
            Logger = logger;
        }

        public WeToolBackgroundService(IServiceProvider rootProvider,
                                       ILogger<WeToolBackgroundService> logger)
        {
            Logger = logger;
            _scope = rootProvider.CreateScope();
            Provider = _scope.ServiceProvider;
        }

        public WeToolBackgroundService(ILogger<WeToolBackgroundService> logger,
                                       IOptionsMonitor<WeToolWorkersOption> workersOption)
        {

            Logger = logger;

            InitConfig(workersOption);
            ChangeConfig(workersOption);
        }

        public WeToolBackgroundService(IServiceProvider rootProvider,
                                      IOptionsMonitor<WeToolWorkersOption> workersOption)
        {

            _scope = rootProvider.CreateScope();
            Provider = _scope.ServiceProvider;

            InitConfig(workersOption);
            ChangeConfig(workersOption);
        }

        public WeToolBackgroundService(IServiceProvider rootProvider,
                                       ILogger<WeToolBackgroundService> logger,
                                       IOptionsMonitor<WeToolWorkersOption> workersOption)
        {

            Logger = logger;
            _scope = rootProvider.CreateScope();
            Provider = _scope.ServiceProvider;

            InitConfig(workersOption);
            ChangeConfig(workersOption);
        }

        private void InitConfig(IOptionsMonitor<WeToolWorkersOption> workersOption)
        {
            _currentWorkerClassName = GetType().Name.ToLower();
            Console.WriteLine($"当前Worker：{_currentWorkerClassName}");
            WorkerConfig = workersOption.CurrentValue.Workers.Where(c => c.WorkerName.ToLower().Equals(_currentWorkerClassName)).First();
        }

        private void ChangeConfig(IOptionsMonitor<WeToolWorkersOption> workersOption)
        {
            workersOption.OnChange(op => {
                Console.WriteLine($"{WorkerConfig.Name}配置已修改");
                var oldConfig = WorkerConfig;
                WorkerConfig = op.Workers.Where(c => c.WorkerName.ToLower().Equals(_currentWorkerClassName)).First();

                ManageTask(oldConfig);
            });
        }

        #endregion

        #region worker
        public WeToolBackgroundService(IOptionsMonitor<WeToolWorkerOption> workerOption)
        {
            InitConfig(workerOption);
            ChangeConfig(workerOption);
        }



        public WeToolBackgroundService(ILogger<WeToolBackgroundService> logger,
                                       IOptionsMonitor<WeToolWorkerOption> workerOption)
        {

            Logger = logger;

            InitConfig(workerOption);
            ChangeConfig(workerOption);
        }

        public WeToolBackgroundService(IServiceProvider rootProvider,
                                      IOptionsMonitor<WeToolWorkerOption> workerOption)
        {

            _scope = rootProvider.CreateScope();
            Provider = _scope.ServiceProvider;

            InitConfig(workerOption);
            ChangeConfig(workerOption);
        }

        public WeToolBackgroundService(IServiceProvider rootProvider,
                                       ILogger<WeToolBackgroundService> logger,
                                       IOptionsMonitor<WeToolWorkerOption> workerOption)
        {

            Logger = logger;
            _scope = rootProvider.CreateScope();
            Provider = _scope.ServiceProvider;

            InitConfig(workerOption);
            ChangeConfig(workerOption);
        }

        private void InitConfig(IOptionsMonitor<WeToolWorkerOption> workerOption)
        {
            _currentWorkerClassName = GetType().Name.ToLower();
            Console.WriteLine($"当前Worker：{_currentWorkerClassName}");
            WorkerConfig = workerOption.Get(_currentWorkerClassName);
        }

        private void ChangeConfig(IOptionsMonitor<WeToolWorkerOption> workerOption)
        {
            workerOption.OnChange((op,name) =>
            {
                if (name.ToLower().Equals(_currentWorkerClassName))
                {
                    Console.WriteLine($"{WorkerConfig.Name}配置已修改");
                    var oldConfig = WorkerConfig;
                    WorkerConfig = op;
                    ManageTask(oldConfig);
                }
            });
        }


        #endregion

        private void ManageTask(WeToolWorkerConfig oldConfig)
        {
            if (WorkerConfig.Enable && !oldConfig.Enable)
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                StartAsync(cts.Token);
            }

            if (!WorkerConfig.Enable && oldConfig.Enable)
            {
                CancellationTokenSource cts = new CancellationTokenSource(0);
                cts.Token.Register(() => Console.WriteLine($"{WorkerConfig.Name} 任务已取消!"));

                StopAsync(cts.Token);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken) 
        {
            Console.WriteLine($"{WorkerConfig.Name}开始了");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"{WorkerConfig.Name}停止了");
            return base.StopAsync(cancellationToken);
        }
        protected abstract override Task ExecuteAsync(CancellationToken stoppingToken);

        public override void Dispose() {

            if (_scope!=null) _scope.Dispose();
            
            base.Dispose();
        }
    }
}
