using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace TaskCenter.DOS
{
    public class DosRunTask
    {
        public void doTask<T>() where T : IJob
        {
            //1.首先创建一个作业调度池
            ISchedulerFactory schedf = new StdSchedulerFactory();
            IScheduler sched = schedf.GetScheduler();
            //2.创建出来一个具体的作业
            IJobDetail job = JobBuilder.Create<T>().Build();
            //3.创建并配置一个触发器
            ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create().WithSimpleSchedule(x => x.WithIntervalInSeconds(10).WithRepeatCount(int.MaxValue)).Build();
            //4.加入作业调度池中
            sched.ScheduleJob(job, trigger);
            //5.开始运行
            sched.Start();

            //Console.ReadKey();
        }
    }
}
