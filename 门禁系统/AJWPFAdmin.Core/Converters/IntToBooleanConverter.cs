using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AJWPFAdmin.Core.Converters
{
    public class IntToBooleanConverter : IValueConverter
    {
        public bool LessThanZero { get; set; }

        public bool EqualZero { get; set; }

        public bool GreaterThanZero { get; set; }

        public bool Default { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is int val)
            {
                if (val < 0)
                {
                    return LessThanZero;
                }
                if (val == 0)
                {
                    return EqualZero;
                }
                if (val > 0)
                {
                    return GreaterThanZero;
                }
            }

            return Default;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
