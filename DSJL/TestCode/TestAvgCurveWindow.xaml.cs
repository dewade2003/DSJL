using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Visifire.Charts;

namespace DSJL
{
    /// <summary>
    /// TestAvgCurveWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestAvgCurveWindow : Window
    {
        Chart chart1;
        Chart chart2;
        public TestAvgCurveWindow()
        {
            InitializeComponent();
            chart1 = new Chart();
            chart2 = new Chart();

            List<List<double>> list = new List<List<double>>();

            int length = 189;
            int length2 = 134;
            int length3 = 123;
            Random r = new Random(1);
            List<double> list1 = new List<double>();
            List<double> list2 = new List<double>();
            List<double> list3 = new List<double>();
            for (int i = 0; i < length; i++) {
                //list1.Add(r.Next(500));
                list1.Add(i + 1);
            }
            for (int i = 0; i < length2; i++)
            {
                list2.Add(r.Next(500));
            }
            for (int i = 0; i < length3; i++)
            {
                list3.Add(r.Next(500));
            }
            list.Add(list1);
            list.Add(list2);
            list.Add(list3);

            for (int i = 0; i < list.Count; i++)
            {
                List<double> d = list[i];
                DataSeries ds = new DataSeries();
                ds.RenderAs = RenderAs.QuickLine;
                for (int j = 0; j < d.Count; j++)
                {
                    DataPoint dp = new DataPoint() { YValue = d[j], XValue = j };
                    ds.DataPoints.Add(dp);
                }
                chart1.Series.Add(ds);
            }

            List<List<double>> avgList = GetAvgPoint(list);
            string s = "";
            for (int i = 0; i < list.Count; i++)
            {
                List<double> d = avgList[i];
                DataSeries ds = new DataSeries();
                ds.RenderAs = RenderAs.QuickLine;
                for (int j = 0; j < d.Count; j++)
                {
                    DataPoint dp = new DataPoint() { YValue = d[j] };
                    ds.DataPoints.Add(dp);
                    if (i == 0)
                    {
                        s += d[j] + ",";
                    }
                }
                chart2.Series.Add(ds);
            }
            //Console.WriteLine("aaaa----"+s);
            chart2.ScrollingEnabled = chart1.ScrollingEnabled = false;
            grid1.Children.Add(chart1);
            grid2.Children.Add(chart2);
        }

        public List<List<double>> GetAvgPoint(List<List<double>> sourceList) {
            List<List<double>> resultList = new List<List<double>>();
            for (int i = 0; i < sourceList.Count; i++) {
                List<double> list = sourceList[i];
                List<double> newList = new List<double>();
                for (int j = 0; j < list.Count-1; j++) {
                    double d1 = list[j];
                    double d2 = list[j + 1];
                    double allowance = ComputeAllowance(d1, d2, 5);
                    newList.Add(d1);
                    for (int a = 0; a < 4; a++) {
                        if (allowance > 0)
                        {
                            newList.Add(d1 - (a + 1) * allowance);
                        }
                        else {
                            newList.Add(d1 + (a + 1) * Math.Abs(allowance));
                        }
                    }
                }
                newList.Add(list[list.Count-1]);
                newList = GetAAA(newList, 300);
                resultList.Add(newList);
            }
            return resultList;
        }

        public List<double> GetAAA(List<double> sourceList, int length) {
            double passCount = sourceList.Count - length;
            if (passCount == 0) {
                return sourceList;
            }
            double sourceCount = sourceList.Count;
            double p = sourceCount / passCount;
            double temp1 = -1;
            for (int i = 0; i < sourceList.Count; i++) {
                temp1++;
                if (temp1 >=p) {//过p个数去掉一个
                    sourceList.RemoveAt(i);
                    temp1 = temp1 - p;
                    i--;
                }
            }
            int newCount = sourceList.Count;
            return sourceList;
        }


        //public List<List<double>> GetAvgPoint(List<double[]> list) {
        //    int[] lengthArr = new int[list.Count];
        //    for (int i = 0; i < list.Count; i++) {
        //        lengthArr[i] = list[i].Length;
        //    }
        //    int maxLength = lengthArr.Max();//获取最大的长度，并把小于这个长度的曲线拉伸为这个长度
        //    List<List<double>> newList = new List<List<double>>();//新的曲线点
        //    for (int i = 0; i < list.Count; i++) {
        //        double[] baseDataArray = list[i];

        //        List<double> douList = new List<double>();
        //        if (baseDataArray.Length == maxLength)
        //        { //如果是最大长度的，直接填充新的坐标点数组
        //            for (int a = 0; a < maxLength; a++) {
        //                douList.Add(baseDataArray[a]);
        //            }
        //        }
        //        else {
        //            //假设旧数据的每段长度是1，计算放大后每段的长度
        //            double baselength = baseDataArray.Length - 1;
        //            double newLength = maxLength - 1;
        //            double allowance = baselength / newLength;//新的每段的长度
        //            int forCount = Convert.ToInt32(Math.Truncate(1 / allowance));//应该插入的数量
                   
        //            douList.Add(baseDataArray[0]);
        //            for (int j = 0; j < baseDataArray.Length-1; j++) {
        //                double startValue = baseDataArray[j];//起点的值
        //                double nextValue = baseDataArray[j + 1];
        //                if (j == baseDataArray.Length - 2) {
        //                    forCount -= 1;
        //                }
        //                double all = ComputeAllowance(startValue, nextValue, forCount + 1);
        //                for (int c = 0; c < forCount; c++) {
        //                    douList.Add(startValue + all * (c + 1));
        //                }
        //                douList.Add(nextValue);
        //            }
        //        }
        //        newList.Add(douList);
        //    }

        //    return newList;
        //}

        //计算公差
        private double ComputeAllowance(double startValue, double endValue, int count)
        {
            double value = 0;
            value = (Math.Abs(startValue) - Math.Abs(endValue)) / count;
            return value;
        }
    }
}
