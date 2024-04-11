using Prism.Ioc;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Services.Jobs
{
    public class DatabaseBackupJob : IAJJob
    {
        public class DatatbaseBackupQuarztJob :IJob
        {
            public Task Execute(IJobExecutionContext context)
            {
                context.MergedJobDataMap.TryGetValue("container", out var container);

                if (container is IContainerProvider provider)
                {
                    var backupSvc = provider.Resolve<AJDatabaseckupService>();
                    var cfgSvc = provider.Resolve<AJConfigService>();

                    backupSvc.Backup(null, cfgSvc.Config["DatabaseAutoBack:BackupPath"]);
                }

                return Task.CompletedTask;
            }
        }

        public void Init(IContainerProvider container, IScheduler scheduler)
        {
            var cfgSvc = container.Resolve<AJConfigService>();

            var jobData = new Dictionary<string, object> { { "container", container } };
            var map = new JobDataMap
            {
                { "container", container }
            };

            var job = JobBuilder.Create<DatatbaseBackupQuarztJob>().WithIdentity(nameof(DatabaseBackupJob))
                .SetJobData(map)
                .WithDescription("自动备份数据库").Build();

            var date = DateTime.Parse(cfgSvc.Config["DatabaseAutoBack:BackupDate"]);

            var trigger = TriggerBuilder.Create().WithCronSchedule($"0 {date.Minute} {date.Hour} 1/1 * ? *").Build();
            //var trigger = TriggerBuilder.Create().WithDailyTimeIntervalSchedule((b) =>
            //{
            //    b.WithIntervalInSeconds(10);
            //}).Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}
