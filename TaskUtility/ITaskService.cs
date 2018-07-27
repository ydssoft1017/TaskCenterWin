using System;
using System.Collections.Generic;
using System.Text;

namespace TaskUtility
{
    public interface  ITaskService
    {
        /// <summary>
        /// 执行任务方法
        /// </summary>
        /// <returns></returns>
        BoolMessage OnServiceRun();
    }
}
