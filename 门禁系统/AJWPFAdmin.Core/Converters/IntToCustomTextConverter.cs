using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AJWPFAdmin.Core.Converters
{
    public class IntToCustomTextConverter : IValueConverter
    {
        public string LessThanZeroText { get; set; }

        public string EqualZeroText { get; set; }

        public string GreaterThanZeroText { get; set; }

        public bool ReturnBindValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is int val)
            {
                if (val < 0)
                {
                    return LessThanZeroText ?? (ReturnBindValue ? val.ToString() : string.Empty);
                }
                if (val == 0)
                {
                    return EqualZeroText ?? (ReturnBindValue ? val.ToString() : string.Empty);
                }
                if (val > 0)
                {
                    return GreaterThanZeroText ?? (ReturnBindValue ? val.ToString() : string.Empty);
                }
            }

            return ReturnBindValue ? (value?.ToString() ?? string.Empty) : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
