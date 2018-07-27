using System;
using System.Collections.Generic;
using System.Text;

namespace TaskUtility
{
    public abstract class DefaultTaskService : ITaskService
    {
        /// <summary>
        /// 执行任务方法
        /// </summary>
        /// <returns></returns>
        public virtual BoolMessage OnServiceRun()
        {
            return BoolMessage.True;
        }
    }
}
