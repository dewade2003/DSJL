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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Visifire.Charts;
using System.Xml.Linq;
using DSJL.DataUtil;

namespace DSJL.Modules.Standard
{
    /// <summary>
    /// PageAvgCurve.xaml 的交互逻辑
    /// </summary>
    public partial class PageAvgCurve : Page
    {
        private Chart chart1;
        private Chart chart2;
        private List<Model.TestInfoModel> modelList;
        private XDocument xdoc;

        SolidColorBrush avgBrush = new SolidColorBrush(Color.FromRgb(61, 201, 54));//平均曲线颜色

        public PageAvgCurve()
        {
            InitializeComponent();
            chart1 = new Chart();
            chart2 = new Chart();
            chart2.ScrollingEnabled = chart1.ScrollingEnabled = false;
            chart1.BorderThickness = chart2.BorderThickness = new Thickness(0, 0, 0, 0);

            Axis yOACAxis = new Axis();
            Axis yEACAxis = new Axis();
            yOACAxis.Suffix = yEACAxis.Suffix = "(Nm)";
            chart1.AxesY.Add(yOACAxis);
            chart2.AxesY.Add(yEACAxis);

            Axis xAxisOAC = new Axis();
            Axis xAxisEAC = new Axis();
            xAxisEAC.AxisMinimum = xAxisOAC.AxisMinimum = 0;
            xAxisOAC.AxisMaximum = xAxisEAC.AxisMaximum = 100;
            xAxisEAC.Suffix = xAxisOAC.Suffix = "%";
            chart1.AxesX.Add(xAxisOAC);
            chart2.AxesX.Add(xAxisEAC);

            grid1.Children.Add(chart1);
            grid2.Children.Add(chart2);
        }

        /// <summary>
        /// 测试信息列表
        /// </summary>
        public List<Model.TestInfoModel> ModelList
        {
            set
            {
                modelList = value;
                chart1.Series.Clear();
                chart2.Series.Clear();
                if (modelList.Count > 0)
                {
                    RefrenshChart();
                }
            }
        }

        public void RefrenshChart() {
            chart1.Series.Clear();
            chart2.Series.Clear();
            List<List<XElement>> oddList = new List<List<XElement>>();
            List<List<XElement>> evenList = new List<List<XElement>>();
            //var groupdModelList = modelList.GroupBy(x => x.Ath_Code);
            //foreach (var item in groupdModelList)
            //{
            //    foreach (Model.TestInfoModel model in item)
            //    {
            //        string xmlFileName = Model.AppPath.RootPath + "\\AppData\\XmlData\\" + model.DataFileName;
            //        xdoc = XDocument.Load(xmlFileName);
            //        XElement oddAvgEle = xdoc.Descendants("oddavg").ElementAt(0);
            //        XElement evenAvgEle = xdoc.Descendants("evenavg").ElementAt(0);

            //        List<XElement> list1 = new List<XElement>();//动作1平均曲线节点
            //        List<XElement> list2 = new List<XElement>();//动作2平均曲线节点

            //        foreach (XElement xe in oddAvgEle.Elements())
            //        {
            //            list1.Add(xe);
            //        }
            //        oddList.Add(list1);

            //        if (model.Test_Mode != "6")
            //        {//如果为等长测试就不加载
            //            foreach (XElement xe in evenAvgEle.Elements())
            //            {
            //                list2.Add(xe);
            //            }
            //            evenList.Add(list2);
            //        }
            //    }
            //}
            foreach (Model.TestInfoModel model in modelList)
            {
                string xmlFileName = Model.AppPath.RootPath + "\\AppData\\XmlData\\" + model.DataFileName;
                xdoc = XDocument.Load(xmlFileName);
                XElement oddAvgEle = xdoc.Descendants("oddavg").ElementAt(0);
                XElement evenAvgEle = xdoc.Descendants("evenavg").ElementAt(0);

                List<XElement> list1 = new List<XElement>();//动作1平均曲线节点
                List<XElement> list2 = new List<XElement>();//动作2平均曲线节点

                foreach (XElement xe in oddAvgEle.Elements())
                {
                    list1.Add(xe);
                }
                oddList.Add(list1);

                if (model.Test_Mode != "6") {//如果为等长测试就不加载
                    foreach (XElement xe in evenAvgEle.Elements())
                    {
                        list2.Add(xe);
                    }
                    evenList.Add(list2);
                }
              
            }

            List<List<double>> oddValueList = ComputeAvgCurve.Compute(oddList);
            List<double> oddAvgValueList = ComputeAvgCurve.ComputeAvg(oddValueList);

            AddSeries(chart1, oddAvgValueList, true, "动作1平均",avgBrush);//平均曲线
            for (int i = 0; i < modelList.Count; i++) {//加载选中的测试信息的曲线
                if (modelList[i].IsChecked == true) {
                    AddSeries(chart1, oddValueList[i], true, modelList[i].Index + "." + modelList[i].Ath_Name,null);//加载每次都曲线
                }
            }

            if (evenList.Count > 0) {
                List<List<double>> evenValueList = ComputeAvgCurve.Compute(evenList);
                List<double> evenAvgValueList = ComputeAvgCurve.ComputeAvg(evenValueList);
                AddSeries(chart2, evenAvgValueList, true, "动作2平均",avgBrush);
                for (int i = 0; i < modelList.Count; i++) {
                    if (modelList[i].IsChecked == true) {
                        AddSeries(chart2, evenValueList[i], true, modelList[i].Index + "." + modelList[i].Ath_Name,null);
                    }
                }
            }
        }

        private void AddSeries(Chart c, List<double> list, bool showInLengend, string lengendText,Brush brush)
        {
            double allCount = 100;
            double value = allCount / list.Count;
            DataSeries ds = new DataSeries();
            ds.ShowInLegend = showInLengend;
            ds.RenderAs = RenderAs.QuickLine;
            if (showInLengend)
            {
                if (brush != null) {
                    ds.Color = avgBrush;
                }

                ds.LegendText = lengendText;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataPoint dp = new DataPoint() { YValue = Math.Abs(list[i]) * 0.1, XValue = i * value };
                ds.DataPoints.Add(dp);
            }
            c.Series.Add(ds);
        }
    }
}
