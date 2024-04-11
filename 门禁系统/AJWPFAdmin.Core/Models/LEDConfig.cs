using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.Models
{
    /// <summary>
    /// LED配置信息
    /// </summary>
    public class LEDConfig
    {
        public string[] EntranceTextArray { get; set; }
        public string[] ExitTextArray { get; set; }

        public void Init()
        {
            EntranceTextArray = DefaultEntranceTextArray;
            ExitTextArray = DefaultExitTextArray;
        }

        public static readonly string[] DefaultEntranceTextArray
            = new string[] { "[日期]", "[时间]", "请扫码登记", "减速慢行" };

        public static readonly string[] DefaultExitTextArray
            = new string[] { "[日期]", "[时间]", "欢迎再来", "减速慢行" };
    }
}
