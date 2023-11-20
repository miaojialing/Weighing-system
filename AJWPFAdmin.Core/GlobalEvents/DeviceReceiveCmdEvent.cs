using AJWPFAdmin.Core.Enums;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.GlobalEvents
{
    /// <summary>
    /// 设备被动接收来自其他组件的通知事件
    /// </summary>
    public class DeviceReceiveCmdEvent : PubSubEvent<DeviceReceiveCmdEventArgs>
    {
    }

    public class DeviceReceiveCmdEventArgs : EventArgs
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public long DeviceId { get; set; }

        /// <summary>
        /// 命令数据, 根据业务逻辑自行处理
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 命令类型
        /// </summary>
        public DeviceReceiveCmdType Type { get; set; }

        public DeviceReceiveCmdEventArgs()
        {

        }
    }

    public enum DeviceReceiveCmdType : short
    {
        开闸
    }

    public class DeviceOpenGateCmdParameter
    {
        /// <summary>
        /// 进出方向
        /// </summary>
        public PassagewayDirection Direction { get; set; }

        /// <summary>
        /// 是否开闸
        /// </summary>
        public bool Open { get; set; }

        /// <summary>
        /// LED文案数组
        /// </summary>
        public string[] LEDTextLines { get; set; }

        /// <summary>
        /// TTS语音文本
        /// </summary>
        public string TTSText { get; set; }

        public void PreProcessLEDTextLines()
        {
            var now = DateTime.Now;

            FormatDateTimeLEDText(LEDTextLines?.ElementAtOrDefault(0), 0, now);
            FormatDateTimeLEDText(LEDTextLines?.ElementAtOrDefault(1), 1, now);
        }

        private void FormatDateTimeLEDText(string text, int index, DateTime date)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            if (text == "[日期]")
            {
                LEDTextLines[index] = date.ToString("yyyy年MM月dd日");
            }

            if (text == "[时间]")
            {
                LEDTextLines[index] = date.ToString("HH时mm分");
            }
        }
    }
}
