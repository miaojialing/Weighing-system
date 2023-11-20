using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.Models
{
    /// <summary>
    /// 接口统一返回结果
    /// </summary>
    public class ProcessResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// ok 或 错误信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public object Data { get; set; }

        public void SetSuccess(object data = null, string message = "ok")
        {
            Success = true;
            Data = data;
            Message = message;
        }

        public void SetError(string message, object data = null)
        {
            Success = false;
            Data = data;
            Message = message;
        }
    }
}
