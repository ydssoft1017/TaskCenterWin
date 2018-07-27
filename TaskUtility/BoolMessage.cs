using System;
using System.Collections.Generic;
using System.Text;

namespace TaskUtility
{
    /// <summary>
    /// 封装布尔消息，消息中封装两个字段：一个表示操作是否成功，一个表示操作消息
    /// </summary>
    public class BoolMessage
    {
        private readonly bool _success;
        private readonly string _message;

        /// <summary>
        /// 成功消息，消息内容为空
        /// </summary>
        public static readonly BoolMessage True = new BoolMessage(true);

        /// <summary>
        /// 失败消息，消息内容为空
        /// </summary>
        public static readonly BoolMessage False = new BoolMessage(false);

        /// <summary>
        /// 从指定的布尔状态来初始化布尔消息
        /// </summary>
        /// <param name="success">布尔状态</param>
        public BoolMessage(bool success)
        {
            _success = success;
        }

        /// <summary>
        /// 从指定的布尔状态和状态消息来初始化布尔消息
        /// </summary>
        /// <param name="success">布尔状态</param>
        /// <param name="message">状态消息</param>
        public BoolMessage(bool success, string message)
        {
            _success = success;
            _message = message.Replace("\r", "").Replace("\n", ""); ;
        }

        /// <summary>
        /// 成功状态
        /// </summary>
        public bool Success
        {
            get { return _success; }
        }

        /// <summary>
        /// 失败状态
        /// </summary>
        public bool Failure
        {
            get { return !_success; }
        }

        /// <summary>
        /// 状态消息
        /// </summary>
        public string Message
        {
            get { return _message; }
        }
    }

    /// <summary>
    /// 封装布尔消息，消息中封装两个字段：一个表示操作是否成功，一个表示操作消息
    /// </summary>
    public class BoolMessageItem : BoolMessage
    {
        /// <summary>
        /// 数据对象
        /// </summary>
        public object DataObject { get; set; }

        /// <summary>
        /// 异常对象
        /// </summary>
        public Exception ExptionObject { get; set; }

        /// <summary>
        /// 从指定的布尔状态来初始化布尔消息
        /// </summary>
        /// <param name="success">操作是否成功</param>
        public BoolMessageItem(bool success)
            : base(success)
        {
        }

        /// <summary>
        /// 从指定的布尔状态来初始化布尔消息
        /// </summary>
        /// <param name="success">操作是否成功</param>
        /// <param name="message">操作消息</param>
        public BoolMessageItem(bool success, string message)
            : base(success, message)
        {
        }

        /// <summary>
        /// 从指定的布尔状态来初始化布尔消息
        /// </summary>
        /// <param name="success">操作是否成功</param>
        /// <param name="message">操作消息</param>
        /// <param name="dataObject">操作数据对象</param>
        public BoolMessageItem(bool success, string message, object dataObject)
            : base(success, message)
        {
            this.DataObject = dataObject;
        }

        /// <summary>
        /// 从指定的布尔状态来初始化布尔消息
        /// </summary>
        /// <param name="success">操作是否成功</param>
        /// <param name="message">操作消息</param>
        /// <param name="dataObject">操作数据对象</param>
        /// <param name="exptionObject">操作异常对象</param>
        public BoolMessageItem(bool success, string message, object dataObject, Exception exptionObject)
            : this(success, message, dataObject)
        {
            this.ExptionObject = exptionObject;
        }
    }
}
