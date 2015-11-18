using System;
using System.Windows.Data;

namespace DSJL
{
    public class CheckConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                bool ischeck = bool.Parse(value.ToString());
                Uri uri = null;
                if (ischeck)
                {
                    uri = new Uri("pack://application:,,,/DSJL;component/Assets/Images/check.png", UriKind.Absolute);
                }
                else {
                    uri = new Uri("pack://application:,,,/DSJL;component/Assets/Images/uncheck.png", UriKind.Absolute);
                }
                return uri;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
