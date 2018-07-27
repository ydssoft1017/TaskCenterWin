using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
namespace TaskService
{
    public abstract class BaseService : IJob
    {
        public virtual void Execute(IJobExecutionContext context)
        {
            
        }
    }
}
