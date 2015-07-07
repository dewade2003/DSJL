using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Test
{
    class Program
    {
        const int weight = 50;
        static void Main(string[] args)
        {
            //List<int> list = new List<int>();
            //for (int i = 0; i < 25; i++) {
            //    list.Add(100 + (i * 2));
            //}
            //Console.WriteLine("峰值力矩:"+list.Max());
            //Console.WriteLine("平均峰力矩:"+list.Average());
            //Console.WriteLine("相对峰力矩：" + Math.Round((double)(list.Max() / weight), 2));
            //Console.WriteLine("相对平均峰力矩：" + Math.Round(list.Average() / weight, 2));
            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.ShortDatePattern = "yyyy-MM-dd";
            dtFormat.ShortTimePattern = "HH:mm";
            string s = "2012-12";
            DateTime dt = Convert.ToDateTime(s, dtFormat);
            Console.WriteLine(dt.ToString());
            Console.ReadLine();
        }
    }
}
