using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace DSJL.DataUtil
{
    public class ComputeAvgCurve
    {
        /// <summary>
        /// 把xml节点转换成double,并转换为同样的长度
        /// </summary>
        /// <param name="sourceEles"></param>
        /// <returns></returns>
        public static List<List<double>> Compute(List<List<XElement>> sourceEles) {
            List<List<double>> resultEles = new List<List<double>>();
            resultEles = InsertFourPoint(sourceEles);
            int minLength = resultEles.Min(x => x.Count);
            foreach (List<double> list in resultEles) {
                GetSomeLengthPoint(list, minLength);
            }
            return resultEles;
        }

        /// <summary>
        /// 把点转换为同样的长度
        /// </summary>
        /// <param name="sourceEles"></param>
        /// <returns></returns>
        public static List<List<double>> Compute(List<List<double>> sourceEles)
        {
            List<List<double>> resultEles = new List<List<double>>();
            resultEles = InsertFourPoint(sourceEles);
            int minLength = resultEles.Min(x => x.Count);
            foreach (List<double> list in resultEles)
            {
                GetSomeLengthPoint(list, minLength);
            }
            return resultEles;
        }

        /// <summary>
        /// 计算多条曲线的平均曲线
        /// </summary>
        /// <param name="sourceList"></param>
        /// <returns></returns>
        public static List<double> ComputeAvg(List<List<double>> sourceList) {
            List<double> list = new List<double>();
            int sourceCount = sourceList.Count;
            int oneCount = sourceList[0].Count;
            for (int i = 0; i < oneCount; i++) {
                list.Add(sourceList.Average(x => x[i]));
            }
            return list;
        }

        public static List<List<double>> InsertFourPoint(List<List<XElement>> sourceEles) {
            List<List<double>> resultList = new List<List<double>>();

            for (int i = 0; i < sourceEles.Count; i++)
            {
                List<XElement> list = sourceEles[i];
                List<double> newList = new List<double>();
                for (int j = 0; j < list.Count - 1; j++)
                {
                    double d1 = Math.Abs(double.Parse(list[j].Attribute("c2").Value));
                    double d2 = Math.Abs(double.Parse(list[j + 1].Attribute("c2").Value));
                    double allowance = ComputeAllowance(d1, d2, 5);
                    newList.Add(d1);
                    for (int a = 0; a < 4; a++)
                    {
                        if (allowance > 0)
                        {
                            newList.Add(d1 - (a + 1) * allowance);
                        }
                        else
                        {
                            newList.Add(d1 + (a + 1) * Math.Abs(allowance));
                        }
                    }
                }
                newList.Add(double.Parse(list[list.Count - 1].Attribute("c2").Value));
                resultList.Add(newList);
            }

            return resultList;
        }

        public static List<List<double>> InsertFourPoint(List<List<double>> sourceEles)
        {
            List<List<double>> resultList = new List<List<double>>();

            for (int i = 0; i < sourceEles.Count; i++)
            {
                List<double> list = sourceEles[i];
                List<double> newList = new List<double>();
                for (int j = 0; j < list.Count - 1; j++)
                {
                    double d1 = list[j];
                    double d2 = list[j + 1];
                    double allowance = ComputeAllowance(d1, d2, 5);
                    newList.Add(d1);
                    for (int a = 0; a < 4; a++)
                    {
                        if (allowance > 0)
                        {
                            newList.Add(d1 - (a + 1) * allowance);
                        }
                        else
                        {
                            newList.Add(d1 + (a + 1) * Math.Abs(allowance));
                        }
                    }
                }
                newList.Add(list[list.Count - 1]);
                resultList.Add(newList);
            }

            return resultList;
        }

        private static List<double> GetSomeLengthPoint(List<double> sourceList, int length) {
            double passCount = sourceList.Count - length;
            if (passCount == 0)
            {
                return sourceList;
            }
            double sourceCount = sourceList.Count;
            double p = sourceCount / passCount;
            double temp1 = -1;
            for (int i = 0; i < sourceList.Count; i++)
            {
                temp1++;
                if (temp1 >= p)
                {//过p个数去掉一个
                    sourceList.RemoveAt(i);
                    temp1 = temp1 - p;
                    i--;
                }
            }
            sourceList.RemoveAt(sourceList.Count - 2);
            return sourceList;
        }
        //计算公差
        public static double ComputeAllowance(double startValue, double endValue, int count)
        {
            double value = 0;
            value = (Math.Abs(startValue) - Math.Abs(endValue)) / count;
            return value;
        }
    }
}
