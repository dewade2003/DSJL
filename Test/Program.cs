using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;

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
            //DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            //dtFormat.ShortDatePattern = "yyyy-MM-dd";
            //dtFormat.ShortTimePattern = "HH:mm";
            //string s = "2012-12";
            //DateTime dt = Convert.ToDateTime(s, dtFormat);
            //Console.WriteLine(dt.ToString());
            //Console.ReadLine();

            //string s = "3232";
            //Console.WriteLine(s.Contains(""));

            //TestLinqGroup.TestGroup();
            Program p = new Program();
            Task t = new Task(p.FirstTask);
            t.Start();
            Console.ReadLine();
        }
        Boolean cancled = false;
        public void FirstTask() {
            for (int i = 0; i < 100; i++)
            {
                if (cancled)
                {
                    Console.WriteLine("cancled");
                    break;
                }
                Console.WriteLine("first task for " + i.ToString());
                Task task = new Task(SecondTask);
                Console.WriteLine("second task status:" + task.Status);
                task.Start();
                Console.WriteLine("second task status:" + task.Status);
                task.Wait();
                Console.WriteLine("second task status:"+task.Status);

                if (i==50)
                {
                    cancled = true;
                }
            }
            Console.WriteLine("first task end");
        }

        int secondTaskCount = 0;
        public void SecondTask() {
            Console.WriteLine("second task :"+secondTaskCount.ToString());
            secondTaskCount++;
        }
    }
}
