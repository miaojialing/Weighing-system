using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AJWPFAdmin.Core.Converters
{
    public class DatetimeFormatConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "--";
            }

            var format = parameter?.ToString() ?? "yyyy-MM-dd HH:mm:ss";

            if (value is DateTime dt)
            {
                return dt.ToString(format);
            }

            return ((DateTime?)value).GetValueOrDefault().ToString(format);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
