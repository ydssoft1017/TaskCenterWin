using System;
using System.Collections.Generic;
using System.Text;
using TaskUtility;
using Quartz;
namespace TaskService
{
    [TaskServiceCaption("测试任务服务2")]
    
    public class DemoService2  : BaseService
    {
        public override void Execute(IJobExecutionContext context)
        {
            Console.WriteLine("DemoService2");
        }
    }
}
