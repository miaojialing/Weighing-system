using AJWPFAdmin.Core.Logger;
using AJWPFAdmin.Services.EF;
using Masuit.Tools.Reflection;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Services.Jobs
{
    public class CarExpiredCheckJob : IAJJob
    {
        public class CarExpiredCheckQuarztJob : IJob
        {
            public Task Execute(IJobExecutionContext context)
            {
                context.MergedJobDataMap.TryGetValue("container", out var container);

                if (container is IContainerProvider provider)
                {
                    var db = provider.Resolve<DbService>();
                    var logger = provider.Resolve<AJLog4NetLogger>();
                    var now = DateTime.Now;

                    var type = db.CarTypes.Where(p => p.SysRequired && p.Name == "临时车").Select(p => new
                    {
                        p.Id,
                        p.Name,
                    }).FirstOrDefault();

                    if (type == null)
                    {
                        logger.Warning($"{context.JobDetail.Description}执行失败:临时车类型数据异常");
                        return Task.CompletedTask;
                    }

                    return db.Cars.Where(p => now >= p.ExpireDate)
                        .ExecuteUpdateAsync(sp => sp.SetProperty(p => p.TypeId, type.Id)
                        .SetProperty(p => p.TypeName, type.Name));
                }

                return Task.CompletedTask;
            }
        }

        public void Init(IContainerProvider container, IScheduler scheduler)
        {
            var jobData = new Dictionary<string, object> { { "container", container } };
            var map = new JobDataMap
            {
                { "container", container }
            };

            var job = JobBuilder.Create<CarExpiredCheckQuarztJob>().WithIdentity(nameof(CarExpiredCheckJob))
                .SetJobData(map)
                .WithDescription("自动检查车辆信息是否到期").Build();

            var trigger = TriggerBuilder.Create().WithCronSchedule($"0 0 6 1/1 * ? *").Build();
            //var trigger = TriggerBuilder.Create().WithDailyTimeIntervalSchedule((b) =>
            //{
            //    b.WithIntervalInSeconds(10);
            //}).Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}
