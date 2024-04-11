using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AJWPFAdmin.Core.Converters
{
    public class DefaultImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var defaultImg = parameter?.ToString();
            defaultImg = string.IsNullOrWhiteSpace(parameter?.ToString()) ? "/images/media-empty.png" : defaultImg;
            return string.IsNullOrWhiteSpace(value?.ToString()) ? defaultImg : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
}
