using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.Utils
{
    /// <summary>
    /// 函数防抖
    /// </summary>
    public class Debounce
    {
        /// <summary>
        /// 函数防抖
        /// </summary>
        /// <param name="callback">回调函数</param>
        /// <param name="dueTime">延迟时间,以毫秒为单位</param>
        public Debounce(Action<object[]> callback, int dueTime)
        {
            this.DueTime = dueTime;
            this.Callback = callback;
            this.Timer = new Timer(ExecuteCallback, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// 计时器
        /// </summary>
        protected Timer Timer { get; }

        /// <summary>
        /// 延迟之前调用的回调方法时指定的时间量,以毫秒为单位
        /// </summary>
        protected int DueTime { get; }

        /// <summary>
        /// 回调函数
        /// </summary>
        protected Action<object[]> Callback { get; }

        /// <summary>
        /// 回调函数参数值
        /// </summary>
        private object[] _args;

        /// <summary>
        /// 触发函数防抖
        /// </summary>
        /// <param name="args">参数值</param>
        public void Trigger(params object[] args)
        {
            _args = args;// 重置延迟时间
            this.Timer.Change(this.DueTime, Timeout.Infinite);
        }

        /// <summary>
        /// 执行回调函数
        /// </summary>
        /// <param name="state"></param>
        private void ExecuteCallback(object state)
        {
            this.Callback(_args);
        }
    }
}
