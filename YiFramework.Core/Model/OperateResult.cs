﻿
using System;
using Newtonsoft.Json;
namespace YiFramework.Core
{
    /// <summary>
    /// 操作结果实体
    /// </summary>
    public partial class OperateResult
    {
        public OperateResult()
        {
            success = true;
            message = "操作成功";
        }
        public bool success { get; set; }
        public object data { get; set; }
        public string message { get; set; }

        [JsonIgnoreAttribute]
        public Exception Exception { get; set; }
        /// <summary>
        /// 设置返回消息
        /// </summary>
        /// <param name="okMsg"></param>
        /// <param name="failMsg"></param>
        public void SetMessage(string okMsg, string failMsg) 
        {
            this.message = this.success ? okMsg : failMsg;
        }
        public void SetMessage() 
        {
            SetMessage("操作成功！", "操作失败!");
        }
    }
}