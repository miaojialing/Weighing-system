using AJWPFAdmin.Services.Jobs;
using Prism.Ioc;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Services
{
    public class QuarztTaskService
    {
        private AJConfigService _cfgSvc;
        private IScheduler _scheduler;
        public QuarztTaskService(AJConfigService cfgSvc)
        {
            _cfgSvc = cfgSvc;
        }

        public async Task StartAsync(IContainerProvider container,IList<IAJJob> jobs)
        {
            _scheduler = await StdSchedulerFactory.GetDefaultScheduler();

            if (jobs.Count  > 0)
            {
                foreach (IAJJob job in jobs)
                {
                    job.Init(container,_scheduler);
                }
            }

            await _scheduler.Start();
        }

        public async Task StopAsync()
        {
            await _scheduler?.Shutdown();
        }
    }
}
