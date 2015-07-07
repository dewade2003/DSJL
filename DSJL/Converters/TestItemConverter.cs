using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace DSJL.Converters
{
    public class TestItemConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                int testID = (int)value;
                DSJL.Model.TB_TestManager testItem =BLL.TB_TestManager.TestItemList.Find(x => x.ID == testID);
                if (testItem != null)
                {
                    return testItem.TestName;
                }
                else {
                    return "";
                }
            }
            else {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
