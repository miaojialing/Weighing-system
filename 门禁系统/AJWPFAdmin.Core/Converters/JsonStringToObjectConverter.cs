using AJWPFAdmin.Core.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AJWPFAdmin.Core.Converters
{
    public class JsonStringToObjectConverter : IValueConverter
    {
        public JsonConvertToObjectType TargetType { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            Type tagType = null;
            switch (TargetType)
            {
                case JsonConvertToObjectType.StringArray:
                    tagType = typeof(string[]);
                    break;
                default:
                    break;
            }

            if (tagType == null)
            {
                return null;
            }

            if(CommonUtil.TryGetJSONObject(value.ToString(), tagType, out var result))
            {
                return result;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public enum JsonConvertToObjectType
    {
        StringArray
    }
}
