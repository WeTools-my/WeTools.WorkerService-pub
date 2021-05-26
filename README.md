# WeTools.WorkerService
.Net core  Worker Service 扩展库，目的为更易控制每一个worker 的运行。

提供根据配置文件对每一个Worker的停止、启动和自动解析注册Worker。

获取配置的方式不限于 使用配置文件，也可以使用数据库，也可以使用 [WeTools.SqlSugarDBConfigProvider]([NuGet Gallery | WeTools.SqlSugarDBConfigProvider 1.0.0](https://www.nuget.org/packages/WeTools.SqlSugarDBConfigProvider/)) nuget包，读取数据库配置。

未来计划 增加一个轻量级的服务配置中心，更方便的管理服务。

## 方式1，worker 类型通过配置文件解析
1. 在配置文件添加节点

```
"WeTools": {
    "Dir": "/", //worker 所在文件夹, 根目录为 / ;
    "Workers": [
      {
        "Name": "Worker2",
        "WorkerName": "Worker2",//具体的实现类名
        "Enable": true //此节点控制worker 的运行
      },
      {
        "Name": "Worker",
        "WorkerName": "Worker",
        "Enable": true
      }

    ]
  }
```
2. 新建worker，并继承WeToolBackgroundService。

3. 在Program类 ConfigureServices 里注册服务

  ```
  services.AddServiceOptions(hostContext);
  services.UseWorkers();
  ```


## 方式2 worker 通过特性解析
1. 在配置文件添加节点
```
"WeTools": {
    "Worker": {
      "name":"testworker",//可选，默认为特性输入的名称
      "workername":"",//可选，默认为特性解析的worker类名
      "Enable": true
    },
    "Worker2":{
     "Enable": true
    }
  }
 或者
 自定义配置节点
 "myconfig": {
    "DemoWorker": {
      "name": "adf123",
      "Enable": true
    }
  }
 或者
 根节点下直接添加
 "DemoWorker": {
    "Enable": true
  },
  "DemoWorker2": {
    "Enable": true
  },
```

2. 新建worker，继承WeToolBackgroundService 并在worker类添加特性

   ```
    	[Worker("DemoWorker")]
       public class TestWorker : WeToolBackgroundService
       {
       }
   ```

3.在Program类 ConfigureServices 里注册服务

   参数对应 1 中的配置 选择不同的方法。

   这里不需要调用 AddServiceOptions 方法。

   ```
   services.UseWorker(hostContext.Configuration);
   services.UseWorker(hostContext);
   services.UseWorker(hostContext.Configuration.GetSection("myconfig"));
   ```

   

 现在即可启动程序。