using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSJL.Tools
{
    class Math
    {
        public static double average(List<double> source) {
            return source.Average();
        }

        public static double Stdev(List<double> source)
        {
            double avg = average(source);
            var sumSquares = source.Aggregate(0, (double current, double next) => current + System.Math.Pow((next - avg), 2));
            var variance = sumSquares / source.Count;
            double sd = System.Math.Round(System.Math.Pow(variance, 0.5), 2);
            return sd;
        
            //double sumstdev = 0;
            //foreach (double d in source)
            //{
            //    sumstdev = sumstdev + (d - avg) * (d - avg);
            //}
            //double stdeval = System.Math.Sqrt(sumstdev);
            //return System.Math.Round(stdeval, 2);
        }
    }
}
