using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace DSJL.Converters
{
    public class StringUnknowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                String valueConverter = value.ToString();
                if (valueConverter.Equals("-1"))
                {
                    return "";
                }
                else {
                    return value;
                }
            }
            else {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                try
                {
                    int convValue = int.Parse(value.ToString());
                    return convValue.ToString();
                }
                catch (Exception)
                {

                    return "-1";
                }
        

            }
            else {
                return "-1";
            }
        }
    }
}
