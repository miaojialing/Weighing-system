using Prism.Ioc;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Services.Jobs
{
    public interface IAJJob
    {
        void Init(IContainerProvider container, IScheduler scheduler);
    }
}
