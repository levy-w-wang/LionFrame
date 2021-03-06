﻿using System.Threading.Tasks;
using LionFrame.CoreCommon;
using LionFrame.Quartz.Listeners;
using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.AdoJobStore;
using Quartz.Impl.AdoJobStore.Common;
using Quartz.Impl.Matchers;
using Quartz.Logging;
using Quartz.Simpl;
using Quartz.Util;

namespace LionFrame.Quartz
{
    /// <summary>
    /// 官网
    /// https://www.quartz-scheduler.net/documentation/quartz-3.x/configuration/reference.html#main-configuration
    /// </summary>
    public class SchedulerFactory
    {
         private IConfiguration Configuration { get; set; }
        public SchedulerFactory(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public async Task<IScheduler> GetScheduler()
        {
            LogProvider.SetCurrentLogProvider(new QuartzLogProvider());
            IScheduler _scheduler;
            string driverDelegateType;
            var db = Configuration.GetSection("DB").Value;
            switch (db)
            {
                case "MsSql":
                    DBConnectionManager.Instance.AddConnectionProvider("default", new DbProvider("SqlServer", Configuration["ConnectionStrings:MsSqlConnection"]));
                    driverDelegateType = typeof(SqlServerDelegate).AssemblyQualifiedName;
                    break;
                case "MySql":
                    DBConnectionManager.Instance.AddConnectionProvider("default", new DbProvider("MySql", Configuration["ConnectionStrings:MySqlConnection"]));
                    driverDelegateType = typeof(MySQLDelegate).AssemblyQualifiedName;
                    break;
                default:
                    DBConnectionManager.Instance.AddConnectionProvider("default", new DbProvider("SqlServer", Configuration["ConnectionStrings:MsSqlConnection"]));
                    driverDelegateType = typeof(SqlServerDelegate).AssemblyQualifiedName;
                    break;
            }

            var serializer = new JsonObjectSerializer();
            serializer.Initialize();
            var jobStore = new JobStoreTX
            {
                DataSource = "default",
                TablePrefix = "QRTZ_",
                InstanceId = "AUTO",
                DriverDelegateType = driverDelegateType ?? string.Empty,  //SQLServer存储
                ObjectSerializer = serializer,
                //Clustered = true, //集群标志
                //AcquireTriggersWithinLock = true,   //如果是集群 建议设为true
                InstanceName = "Lion",
            };
            var threadPool = new DefaultThreadPool
            {
                ThreadCount = 20,
            };
            DirectSchedulerFactory.Instance.CreateScheduler("Scheduler", "AUTO", threadPool, jobStore);
            _scheduler = await SchedulerRepository.Instance.Lookup("Scheduler");
            _scheduler.ListenerManager.AddJobListener(new MyJobListener(), GroupMatcher<JobKey>.AnyGroup());
            //_scheduler.ListenerManager.AddTriggerListener(new MyTriggerListener(), GroupMatcher<TriggerKey>.AnyGroup());
            _scheduler.ListenerManager.AddSchedulerListener(new MySchedulerListener());

            return _scheduler;
        }
    }
}
