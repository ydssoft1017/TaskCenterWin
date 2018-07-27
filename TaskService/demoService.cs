using System;
using TaskUtility;
using Quartz;
using System.Threading;

namespace TaskService
{
    [TaskServiceCaption("测试任务服务")]
    public class DemoService : BaseService
    {
        public override void  Execute(IJobExecutionContext context)
        {
             Console.WriteLine("执行DemoService");
        }
    }
}
