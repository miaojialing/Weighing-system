using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AJWPFAdmin.Core.Converters
{
    public class BooleanMapConverter : IValueConverter
    {
        
        public object TrueValue { get; set; }
        public object FalseValue { get; set; }
        public object NullValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return NullValue;
            }
            if (value is bool val)
            {
                return val ? TrueValue : FalseValue;
            }
            var nullableVal = value as bool?;

            return nullableVal == null ? NullValue : nullableVal.GetValueOrDefault() ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
