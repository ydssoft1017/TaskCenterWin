using System;
using System.Collections.Generic;
using System.Text;

namespace TaskUtility
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TaskServiceCaption : Attribute
    {
        private string _provider;
        /// <summary>
        /// 初始化工作流事件描述
        /// </summary>
        /// <param name="cation">事件描述</param>
        public TaskServiceCaption(string cation)
        {
            Cation = cation;
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string Cation { get; set; }

        /// <summary>
        /// 实现类
        /// </summary>
        public Type InstanceType { get; internal set; }

        /// <summary>
        /// 实现类字符串
        /// </summary>
        public string Provider
        {
            get
            {
                if (string.IsNullOrEmpty(_provider))
                {
                    _provider = AssemblyHelper.GetTypeFullName(InstanceType);
                }
                return _provider;
            }
        }
    }
}
