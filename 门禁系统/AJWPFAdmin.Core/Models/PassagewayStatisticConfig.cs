using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AJWPFAdmin.Core.Models
{
    /// <summary>
    /// 通道统计配置模型
    /// </summary>
    public class PassagewayStatisticConfig
    {
        public PassagewayStatisticType Type { get; set; }

        public PassagewayStatisticConfig()
        {
            Type = PassagewayStatisticType.Once;
        }
    }

    public enum PassagewayStatisticType : short
    {
        /// <summary>
        /// 只要通过一次就算统计
        /// </summary>
        Once,
        /// <summary>
        /// 一进一出才算一次统计
        /// </summary>
        Twice
    }

    public class PassagewayStatisticTypeDescConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
            {
                return string.Empty;
            }
            var enumType = (PassagewayStatisticType)value;
            return enumType switch
            {
                PassagewayStatisticType.Once => "只要车辆通过就算统计次数",
                PassagewayStatisticType.Twice => "只有先进入后出去之后才算统计次数",
                _ => string.Empty,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
