using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Visifire.Charts;
using System.Xml.Linq;
using DSJL.DataUtil;
using DSJL.Caches.ChartCache;
using DSJL.Tools;
using System.Threading.Tasks;

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
        //private XDocument xdoc;

        SolidColorBrush twoSDBrush = new SolidColorBrush(Color.FromArgb(50,3, 3, 247));//平均曲线颜色，红色
        SolidColorBrush oneSDBrush = new SolidColorBrush(Color.FromArgb(50,3, 247, 9));//平均曲线上限颜色，绿色
        SolidColorBrush fTwoSDBrush = new SolidColorBrush(Color.FromArgb(50,255, 20, 2));//平均曲线下限颜色，红色
        SolidColorBrush fOneSDBrush = new SolidColorBrush(Color.FromArgb(50, 255, 246, 2));//平均曲线下限颜色，黄色
        SolidColorBrush avgBrush = new SolidColorBrush(Color.FromArgb(50, 0, 0, 0));//平均曲线下限颜色，黄色

        static Dictionary<int, Dictionary<DataPointsType, List<List<double>>>> dataPointsDict=new Dictionary<int, Dictionary<DataPointsType, List<List<double>>>>();
        static Dictionary<int, DataSeries> oddTestDataSeriseDict=new Dictionary<int, DataSeries>();//缓存测试信息的曲线
        static Dictionary<int, DataSeries> evenTestDataSeriseDict = new Dictionary<int, DataSeries>();//缓存测试信息的曲线

        static Dictionary<int, DataSeriesCollection> oddAvgSDSeriseDict = new Dictionary<int, DataSeriesCollection>();
        static Dictionary<int, DataSeriesCollection> evenAvgSDSeriseDict = new Dictionary<int, DataSeriesCollection>();

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
                //RefrenshChart();
            }
        }

        public Model.TB_StandardInfo CurrentStandardInfo {
            get;
            set;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        public void UpdateChart() {
            chart1.Series.Clear();
            chart2.Series.Clear();
            if (CurrentStandardInfo == null || modelList == null || modelList?.Count == 0)
            {
                return;
            }
            Task task = new Task(() =>
            {
                if (CurrentStandardInfo.Tag == -1)//导入的参考值
                {
                    Model.ExportStandModel standModel = Stand.StandConfig.GetStandModel(CurrentStandardInfo.StandFileName);
                    List<List<double>> oddavgsd = standModel.OddAvgSD;
                    List<List<double>> evenavgsd = standModel.EvenAvgSD;
                    DataSeriesCollection oddAvgSDCollection= GetAvgSDSeriseCollection(oddavgsd);
                    DataSeriesCollection evenAvgSDCollection = GetAvgSDSeriseCollection(evenavgsd);
                    Dispatcher.Invoke(new Action(() =>
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            var oddItem = oddAvgSDCollection[i];
                            chart1.Series.Add(oddItem);
                            var evenItem = evenAvgSDCollection[i];
                            chart2.Series.Add(evenItem);
                        }

                    }));
                }
                else
                {
                    Dictionary<DataPointsType, List<List<double>>> dataDict = null;
                    if (dataPointsDict.Keys.Contains(CurrentStandardInfo.ID))
                    {
                        dataDict = dataPointsDict[CurrentStandardInfo.ID];
                        if (dataDict[DataPointsType.ODD].Count != modelList.Count)
                        {
                            dataDict = StandardChartCache.GetStandardDataPoints(CurrentStandardInfo, modelList);
                            dataPointsDict[CurrentStandardInfo.ID] = dataDict;
                        }
                    }
                    else
                    {
                        dataDict = StandardChartCache.GetStandardDataPoints(CurrentStandardInfo, modelList);
                        dataPointsDict.Add(CurrentStandardInfo.ID, dataDict);
                    }


                    if (!oddAvgSDSeriseDict.Keys.Contains(CurrentStandardInfo.ID))
                    {
                        List<List<double>> oddavgsd = dataDict[DataPointsType.ODDAvgSD];
                        List<List<double>> evenavgsd = dataDict[DataPointsType.EVENAVGSD];
                        oddAvgSDSeriseDict.Add(CurrentStandardInfo.ID, GetAvgSDSeriseCollection(oddavgsd));
                        evenAvgSDSeriseDict.Add(CurrentStandardInfo.ID, GetAvgSDSeriseCollection(evenavgsd));
                    }
                    Dispatcher.Invoke(new Action(() =>
                    {

                        for (int i = 0; i < 5; i++)
                        {
                            var oddItem = oddAvgSDSeriseDict[CurrentStandardInfo.ID][i];
                            chart1.Series.Add(oddItem);
                            var evenItem = evenAvgSDSeriseDict[CurrentStandardInfo.ID][i];
                            chart2.Series.Add(evenItem);
                        }

                    }));
                }
            });
            task.Start();
        }

        public void RefrenshChart()
        {
            while (chart1.Series.Count>5) {
                chart1.Series.RemoveAt(5);
            }
            if (chart2.Series.Count >= 5)
            {
                while (chart2.Series.Count > 5)
                {
                    chart2.Series.RemoveAt(5);
                }
            }

            Dictionary<DataPointsType, List<List<double>>> dataDict = dataPointsDict[CurrentStandardInfo.ID];

            List<List<double>> odd = dataDict[DataPointsType.ODD];
            List<List<double>> even = dataDict[DataPointsType.EVEN];
            for (int i = 0; i < modelList.Count; i++)
            {
                if (modelList[i].IsChecked)
                {
                    DataSeries oddSerise = GetTestSerise(modelList[i], odd[i]);
                    chart1.Series.Add(oddSerise);
                    if (modelList[i].Test_Mode != "6")
                    {
                        DataSeries evenSerise = GetTestSerise(modelList[i], even[i]);
                        chart2.Series.Add(evenSerise);
                    }
                }
            }
        }

        private void AddSeriseToChart(Chart c, Model.TestInfoModel model, List<double> dataList) {
            if (!oddTestDataSeriseDict.Keys.Contains(model.TestID))
            {
                DataSeries ds = new DataSeries();
                DataSeriseUtil.InitLineDataSerise(ds, true, model.Index + model.Ath_Name, null);
                DataSeriseUtil.AddDataToDataSerise(ds, dataList);
                c.Series.Add(ds);
            }
            else
            {
                c.Series.Add(oddTestDataSeriseDict[model.TestID]);
            }
        }

        private DataSeries GetTestSerise(Model.TestInfoModel model, List<double> dataList)
        {
            DataSeries ds = null;
            if (oddTestDataSeriseDict.Keys.Contains(model.TestID))
            {
                return oddTestDataSeriseDict[model.TestID];
            }
         
            this.Dispatcher.Invoke(new Action(() =>
            {
                ds = new DataSeries();
                if (!oddTestDataSeriseDict.Keys.Contains(model.TestID))
                {
                    DataSeriseUtil.InitLineDataSerise(ds, true, model.Index + model.Ath_Name, null);
                    //ds.LineThickness = 3;
                    DataSeriseUtil.AddDataToDataSerise(ds, dataList);
                }
                else
                {
                    ds = oddTestDataSeriseDict[model.TestID];
                }
            }));
           
            return ds;
        }

        private void AddAvgSDSeriseToChart(Chart chart, List<List<double>> dataLists) {
            AddAvgSeries(chart, dataLists[2], true, "2sd", twoSDBrush);//平均曲线
            AddAvgSeries(chart, dataLists[1], true, "1sd", oneSDBrush);//平均曲线
            AddAvgSeries(chart, dataLists[3], true, "-1sd", fOneSDBrush);//平均曲线
            AddAvgSeries(chart, dataLists[4], true, "-2sd", fTwoSDBrush);//平均曲线
            AddSeries(chart, dataLists[0], true, "平均", null);
          
        }

        private DataSeriesCollection GetAvgSDSeriseCollection( List<List<double>> dataLists)
        {
            DataSeriesCollection dsc = new DataSeriesCollection()
            {
                GetAvgSDSeries(dataLists[2], true, "2sd", twoSDBrush,RenderAs.Area),
                GetAvgSDSeries(dataLists[1], true, "1sd", oneSDBrush,RenderAs.Area),
                GetAvgSDSeries(dataLists[3], true, "-1sd", fOneSDBrush,RenderAs.Area),
                GetAvgSDSeries(dataLists[4], true, "-2sd", fTwoSDBrush,RenderAs.Area),
               GetAvgSDSeries(dataLists[0], true, "平均", avgBrush,RenderAs.QuickLine)
             };
            return dsc;
            //DataSeries avgSerise =
            //DataSeries sd2Serise =
            //dsc.Add(sd2Serise);//平均曲线
            //AddAvgSeries(chart, dataLists[1], true, "1sd", null);//平均曲线
            //AddAvgSeries(chart, dataLists[3], true, "-1sd", fOneSDBrush);//平均曲线
            //AddAvgSeries(chart, dataLists.Last(), true, "-2sd", fTwoSDBrush);//平均曲线
            //AddSeries(chart, dataLists[0], true, "平均", null);

        }

        private DataSeries GetAvgSDSeries(List<double> list, bool showInLengend, string lengendText, Brush brush,RenderAs render)
        {
            DataSeries ds = null;
            Dispatcher.Invoke(new Action(() =>
            {
                ds= new DataSeries();
                DataSeriseUtil.InitDataSerise(ds, true, lengendText, brush,render);
                DataSeriseUtil.AddDataToDataSerise(ds, list);
            }));
         
            return ds;
        }


        private void AddAvgSeries(Chart c, List<double> list, bool showInLengend, string lengendText, Brush brush)
        {
            DataSeries ds = new DataSeries();
            DataSeriseUtil.InitDataSerise(ds, true, lengendText, brush,RenderAs.Area);
            DataSeriseUtil.AddDataToDataSerise(ds, list);
            c.Series.Add(ds);
        }

        public void RefrenshChartOld() {
            //chart1.PlotArea.Background = chart2.PlotArea.Background = aboveTowSDBrush;
            chart1.Series.Clear();
            chart2.Series.Clear();
            if (modelList==null|| modelList?.Count==0)
            {
                return;
            }
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
                //string xmlFileName = Model.AppPath.RootPath + "\\AppData\\XmlData\\" + model.DataFileName;
                //xdoc = Caches.TestData.TestDataCenter.GetTestDataByFileName(model).XDoc;
                Model.TestData.TestData testData = Caches.TestData.TestDataCenter.GetTestDataByFileName(model);
                XElement oddAvgEle = testData.GetOddAvgElement();
                XElement evenAvgEle = testData.GetEvenAvgElement();

                List<XElement> list2 = new List<XElement>();//动作2平均曲线节点
                oddList.Add(oddAvgEle.Elements().ToList());

                if (model.Test_Mode != "6") {//如果为等长测试就不加载
                    evenList.Add(evenAvgEle.Elements().ToList());
                }
            }

            List<List<double>> oddValueList = ComputeAvgCurve.Compute(oddList);
            ComputeAvgSDAndAddSeries(chart1, oddValueList);
         

            for (int i = 0; i < modelList.Count; i++) {//加载选中的测试信息的曲线
                if (modelList[i].IsChecked == true) {
                    AddSeries(chart1, oddValueList[i], true, modelList[i].Index + "." + modelList[i].Ath_Name,null);//加载每次都曲线
                }
            }

            if (evenList.Count > 0) {
                List<List<double>> evenValueList = ComputeAvgCurve.Compute(evenList);
                //List<double> evenAvgValueList = ComputeAvgCurve.ComputeAvg(evenValueList);
                ComputeAvgSDAndAddSeries(chart2, evenValueList);
                for (int i = 0; i < modelList.Count; i++) {
                    if (modelList[i].IsChecked == true) {
                        AddSeries(chart2, evenValueList[i], true, modelList[i].Index + "." + modelList[i].Ath_Name,null);
                    }
                }
            }


        }


        public void ComputeAvgSDAndAddSeries(Chart chart,List<List<double>> source)
        {
            //计算标准差
            List<double> avgValueList = new List<double>();
            List<double> twosdValueList = new List<double>();
            List<double> fTwoSDValueList = new List<double>();
            List<double> fOneSDValueList = new List<double>();
            int pointCount = source[0].Count;
            for (int i = 0; i < pointCount; i++)
            {
                List<double> pointsI = new List<double>();
                foreach (var item in source)
                {
                    pointsI.Add(item[i]);
                }
                double avg = Tools.Math.average(pointsI);
                avgValueList.Add(avg);
                double stdev = DSJL.Tools.Math.Stdev(pointsI);
                twosdValueList.Add(avg + 2*stdev);
                fTwoSDValueList.Add(avg -2* stdev);
                fOneSDValueList.Add(avg-stdev);
            }
            //List<double> oddAvgValueList = ComputeAvgCurve.ComputeAvg(oddValueList);
            AddSeries(chart, avgValueList, true, "平均曲线",null);
            AddAvgSeries(chart, twosdValueList, true, "2sd", twoSDBrush);//平均曲线
            AddAvgSeries(chart, fOneSDValueList, true, "-1sd", fOneSDBrush);//平均曲线
            AddAvgSeries(chart, fTwoSDValueList, true, "-2sd", fTwoSDBrush);//平均曲线
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
                    ds.Color = twoSDBrush;
                }

                ds.LegendText = lengendText;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataPoint dp = new DataPoint() { YValue = System.Math.Abs(list[i]) * 0.1, XValue = i * value };
                ds.DataPoints.Add(dp);
            }
            c.Series.Add(ds);
        }

    }
}
