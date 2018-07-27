using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskService;
using TaskUtility;

namespace TaskCenter.DOS
{
    class Program
    {
        static void Main(string[] args)
        {
            DosRunTask dk = new DosRunTask();
            //dk.doTask<DemoService>();
            //dk.doTask<DemoService2>();
            var service = ReflectionHelper.CreateInstance<BaseService>("DemoService");
            dk.doTask<BaseService>();
        }
    }
}
