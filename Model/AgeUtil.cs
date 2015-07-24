using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSJL.Model
{
    public class AgeUtil
    {
        public static int GetAge(DateTime firstDT, DateTime secondDT)
        {
            int age = 0;
            int dayGap = firstDT.Day - secondDT.Day;
            int monthGap = firstDT.Month - secondDT.Month;
            int yearGap = firstDT.Year - secondDT.Year;
            if (dayGap < 0)
            {
                dayGap = dayGap + 30;
                monthGap = monthGap - 1;
                if (monthGap < 0)
                {
                    monthGap = monthGap + 12;
                    yearGap = yearGap - 1;
                }

            }
            else if (monthGap < 0)
            {
                monthGap = monthGap + 12;
                yearGap = yearGap - 1;
            }
            age =Math.Abs( yearGap);
            return age;
        }
    }
}
