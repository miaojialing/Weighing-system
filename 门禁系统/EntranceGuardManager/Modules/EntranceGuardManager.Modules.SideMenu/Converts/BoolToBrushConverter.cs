using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace EntranceGuardManager.Modules.SideMenu.Converts
{
    public class BoolToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value 
                ? (SolidColorBrush)Application.Current.Resources["PrimaryHueMidBrush"] 
                : new SolidColorBrush(Color.FromRgb(86, 86, 86));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}
