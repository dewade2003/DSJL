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
using System.Xml.Linq;
using System.Xml;
using System.IO;
using DSJL.Export;
using DSJL.DataUtil;
using Microsoft.Win32;
using System.Data;
using DSJL.DBUtility;
using WPFHelper.Window;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace DSJL.Modules.Test
{
    /// <summary>
    /// ShowChartWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ShowChartWindow : Window
    {
        private Chart progressChart;//过程曲线图表
        private Chart maxMomentChart;//最大力矩图表
        private Chart maxMomentLineChart;//最大力矩折现图
        private Chart oddAvgCurveChart;//动作1平均曲线图表
        private Chart evenAvgCurveChart;//动作2平均曲线

        private List<Model.TestCountModel> oddTestCountList;
        private List<Model.TestCountModel> evenTestCountList;

        private ExportReport exportReport;
        private XDocument dataDoc;

        private Model.TB_Dict actionModel;
        private BLL.TB_Dict dictBLL;

        private string momentColumn = "c2";//力矩列

        SolidColorBrush oddBrush = new SolidColorBrush(Color.FromRgb(44, 90, 222));//奇数线条颜色
        SolidColorBrush evenBrush = new SolidColorBrush(Color.FromRgb(178, 44, 222));//偶数线条颜色

        SolidColorBrush otherBrush = new SolidColorBrush(Color.FromRgb(134, 131, 130));//灰色线条颜色
        
        public ShowChartWindow()
        {
            InitializeComponent();
            WindowHelper.RepairWindowBehavior(this);
            progressChart = new Chart();
            maxMomentChart = new Chart();
            maxMomentLineChart = new Chart();
            oddAvgCurveChart = new Chart();
            evenAvgCurveChart = new Chart();

            dictBLL = new BLL.TB_Dict();
        }

        /// <summary>
        /// 如果只是导出，设置window不可见
        /// </summary>
        public bool IsExport
        {
            set;
            get;
        }

        public bool IsOne
        {
            set;
            get;
        }


        public Model.TestInfoModel DataModel
        {
            get;
            set;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //if (DataModel.Gravitycomp.Trim() == "1")
            //{
            //    momentColumn = "c2";
            //}

            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            if (!IsExport)//不是只导出报告，则居中
            {
                if (IsOne) {
                    this.Left = (screenWidth - this.Width) / 2;
                    this.Top = (screenHeight - this.Height) / 2;
                }
             
            }
            else {
                this.Left = screenWidth;
                this.Visibility = Visibility.Hidden;
            }

            this.Title = tbTitle.Text = DataModel.Ath_Name + " 测试日期：" + DataModel.TestDate.ToString("yyyy-MM-dd ") + DataModel.TestTime.ToString("HH:mm");

            actionModel = dictBLL.GetModel(DataModel.Joint, DataModel.Plane, DataModel.Test_Mode);

            txtActionODD.Text = actionModel.actionone;
            txtActionEven.Text = actionModel.actiontwo;
            maxMomentLineChart.ScrollingEnabled = maxMomentChart.ScrollingEnabled = oddAvgCurveChart.ScrollingEnabled = evenAvgCurveChart.ScrollingEnabled = progressChart.ScrollingEnabled = false;
            maxMomentLineChart.BorderThickness = maxMomentChart.BorderThickness = oddAvgCurveChart.BorderThickness = evenAvgCurveChart.BorderThickness = progressChart.BorderThickness = new Thickness(0, 0, 0, 0);
            maxMomentChart.AnimationEnabled = maxMomentLineChart.AnimationEnabled = oddAvgCurveChart.AnimationEnabled = evenAvgCurveChart.AnimationEnabled = progressChart.AnimationEnabled = false;

            Axis xAxis = new Axis();
            //Axis xAxisOAC = new Axis();
            //Axis xAxisEAC = new Axis();
            //xAxis.Suffix = xAxisOAC.Suffix = xAxisEAC.Suffix = "(ms)";
            xAxis.Suffix = "(ms)";
            progressChart.AxesX.Add(xAxis);
            //oddAvgCurveChart.AxesX.Add(xAxisOAC);
            //evenAvgCurveChart.AxesX.Add(xAxisEAC);

            Axis yPCAxis=new Axis();
            Axis yMMAxis=new Axis();
            Axis yMMLAxis=new Axis();
            Axis yOACAxis = new Axis();
            Axis yEACAxis = new Axis();
            yPCAxis.Suffix = yMMAxis.Suffix = yMMLAxis.Suffix = yOACAxis.Suffix = yEACAxis.Suffix = "(Nm)";
            progressChart.AxesY.Add(yPCAxis);
            maxMomentChart.AxesY.Add(yMMAxis);
            maxMomentLineChart.AxesY.Add(yMMLAxis);
            oddAvgCurveChart.AxesY.Add(yOACAxis);
            evenAvgCurveChart.AxesY.Add(yEACAxis);

            Axis xAxisOAC = new Axis();
            Axis xAxisEAC = new Axis();
            xAxisEAC.AxisMinimum = xAxisOAC.AxisMinimum = 0;
            xAxisOAC.AxisMaximum = xAxisEAC.AxisMaximum = 100;
            //xAxisEAC.Suffix = xAxisOAC.Suffix = "%";
            oddAvgCurveChart.AxesX.Add(xAxisOAC);
            evenAvgCurveChart.AxesX.Add(xAxisEAC);

            RefrenshChart();

            chartGrid.Children.Add(progressChart);
            maxMomentGrid.Children.Add(maxMomentChart);
            maxMomentLineGrid.Children.Add(maxMomentLineChart);
            oddAvgCurveGrid.Children.Add(oddAvgCurveChart);
            evenAvgCurveGrid.Children.Add(evenAvgCurveChart);
        }

        //刷新过程曲线
        private void RefrenshChart() {
            oddTestCountList = new List<Model.TestCountModel>();
            evenTestCountList = new List<Model.TestCountModel>();

            progressChart.Series.Clear();

            dataDoc = XDocument.Load(Model.AppPath.XmlDataDirPath + DataModel.DataFileName);
            List<XElement> dataEles = dataDoc.Descendants("data").ToList<XElement>();
            XElement datasEle = dataDoc.Descendants("datas").ElementAt(0);
            XElement oddActionEle = dataDoc.Descendants("action1").ElementAt(0);
            XElement evenActionEle = dataDoc.Descendants("action2").ElementAt(0);

            Binding b1 = new Binding(".") { Source = oddActionEle.Elements().ToList<XElement>() };
            oddLV.SetBinding(ListView.ItemsSourceProperty, b1);
            Binding b2 = new Binding(".") { Source = evenActionEle.Elements().ToList<XElement>() };
            evenLV.SetBinding(ListView.ItemsSourceProperty, b2);

            string oddTestCountStr = oddActionEle.Attribute("index").Value;
            string evenTestCountStr = evenActionEle.Attribute("index").Value;
            int startIndex = int.Parse(datasEle.Attribute("startindex").Value);
            int testCount = int.Parse(datasEle.Attribute("testcount").Value) / 2;

            for (int i = 1; i <= testCount; i++)
            {
                Model.TestCountModel oddCountModel = new Model.TestCountModel();
                Model.TestCountModel evenCountModel = new Model.TestCountModel();
                evenCountModel.Count = oddCountModel.Count = i.ToString();
                if (oddTestCountStr.Contains(i.ToString()))
                {
                    oddCountModel.IsChecked = true;
                }
                if (evenTestCountStr.Contains(i.ToString()))
                {
                    evenCountModel.IsChecked = true;
                }
                oddTestCountList.Add(oddCountModel);
                evenTestCountList.Add(evenCountModel);
            }

            Binding oddTestCountBind = new Binding() { Source = oddTestCountList };
            oddCountLB.SetBinding(ListBox.ItemsSourceProperty, oddTestCountBind);
            Binding evenTestCountBind = new Binding() { Source = evenTestCountList };
            evenCountLB.SetBinding(ListBox.ItemsSourceProperty, evenTestCountBind);

            int testIndex = 0;

            DataSeries ds = new DataSeries();
            bool isAddOddLegend = false;
            bool isAddEvenLegend = false;
            for (int i = startIndex; i < dataEles.Count; i++)
            {
                XElement xe = dataEles.ElementAt(i);
                DataPoint dp = new DataPoint() { YValue = Math.Abs(double.Parse(xe.Attribute(momentColumn).Value)) * 0.1, XValue = double.Parse(xe.Attribute("c0").Value) };

                int driction = int.Parse(xe.Attribute("c5").Value);
                if (driction > testIndex)
                {
                    testIndex = driction;
                    if (ds.DataPoints.Count != 0)
                    {
                        progressChart.Series.Add(ds);
                        if (driction % 2 == 0)
                        {
                            if (isAddEvenLegend)
                            {//如果已经添加动作2的legend就不再添加
                                ds.ShowInLegend = false;
                            }
                            isAddEvenLegend = true;
                          
                        }
                        else {
                            if (isAddOddLegend)
                            {//如果已经添加动作1的legend就不再添加
                                ds.ShowInLegend = false;
                            }
                            isAddOddLegend = true;
                        }
                    }
                    ds = new DataSeries();
                    ds.RenderAs = RenderAs.QuickLine;
                    if (driction % 2 == 0)
                    {
                        ds.Color = evenBrush;
                        ds.LegendText = actionModel.actiontwo;
                      
                    }
                    else
                    {
                        ds.Color = oddBrush;
                        ds.LegendText = actionModel.actionone;
                       
                    }
                }
                if (driction % 2 == 0)
                {
                    if (evenTestCountStr.Contains((driction / 2).ToString()))
                    {
                        ds.DataPoints.Add(dp);
                    }
                    else
                    {
                        XElement last = dataEles.FindLast(x => x.Attribute("c5").Value == driction.ToString());
                        i = dataEles.IndexOf(last) + 1;
                        continue;
                    }
                }
                else
                {
                    if (oddTestCountStr.Contains(((driction + 1) / 2).ToString()))
                    {
                        ds.DataPoints.Add(dp);
                    }
                    else
                    {
                        XElement last = dataEles.FindLast(x => x.Attribute("c5").Value == driction.ToString());
                        i = dataEles.IndexOf(last) + 1;
                        continue;
                    }
                }

            }
            if (isAddOddLegend || isAddEvenLegend) {
                ds.ShowInLegend = false;
            }
            progressChart.Series.Add(ds);

            RefrenshMaxMomentChart();
            RefrenshAvgCurveChart();
        }

        //刷新最大力矩曲线
        private void RefrenshMaxMomentChart() {
            maxMomentChart.Series.Clear();
            maxMomentLineChart.Series.Clear();
            DataSeries oddColumnDS = new DataSeries();
            DataSeries evenColumnDS = new DataSeries();
            DataSeries oddLineDS = new DataSeries();
            DataSeries evenLineDS = new DataSeries();

            List<double> oddMaxMomentList = new List<double>();
            List<double> evenMaxMomentList = new List<double>();

            string[] oddTestCount = dataDoc.Descendants("action1").ElementAt(0).Attribute("index").Value.Split(',');
            string[] evenTestCount = dataDoc.Descendants("action2").ElementAt(0).Attribute("index").Value.Split(',');

            List<XElement> dataEleList = dataDoc.Descendants("data").ToList<XElement>();
            int testCount = int.Parse(dataDoc.Descendants("datas").ElementAt(0).Attribute("testcount").Value);
            for (int i = 1; i <= testCount; i++) {
                List<XElement> ixe = (from x in dataEleList where x.Attribute("c5").Value == i.ToString() select x).ToList<XElement>();
                var maxEle = ixe.OrderByDescending(x => Math.Abs(int.Parse(x.Attribute(momentColumn).Value))).ElementAt(0);

                if (i % 2 == 0)//偶数次数
                {
                    if (evenTestCount.Contains((i / 2).ToString()))
                    {
                        evenMaxMomentList.Add(Math.Abs(double.Parse(maxEle.Attribute(momentColumn).Value)) * 0.1);
                    }
                    else {
                        evenMaxMomentList.Add(0);
                    }
                }
                else
                {//奇数次数
                    if (oddTestCount.Contains(((i + 1) / 2).ToString()))
                    {
                        oddMaxMomentList.Add(Math.Abs(double.Parse(maxEle.Attribute(momentColumn).Value)) * 0.1);
                    }
                    else {
                        oddMaxMomentList.Add(0);
                    }
                }
            }

            foreach (double d in oddMaxMomentList) {
                DataPoint dp = new DataPoint() { YValue = d };
                oddColumnDS.DataPoints.Add(dp);
                if (d != 0) {
                    DataPoint dp1 = new DataPoint() { YValue = d };
                    oddLineDS.DataPoints.Add(dp1);
                }
            }

            foreach (double d in evenMaxMomentList)
            {
                DataPoint dp = new DataPoint() { YValue = d };
                evenColumnDS.DataPoints.Add(dp);
                if (d != 0) {
                    DataPoint dp1 = new DataPoint() { YValue = d };
                    evenLineDS.DataPoints.Add(dp1);
                }
            }

            oddColumnDS.RenderAs = evenColumnDS.RenderAs = RenderAs.Column;
            oddLineDS.RenderAs = evenLineDS.RenderAs = RenderAs.Line;
            oddLineDS.LineThickness = evenLineDS.LineThickness = 2;
            oddColumnDS.Color = oddLineDS.Color = oddBrush;
            evenColumnDS.Color = evenLineDS.Color = evenBrush;

            oddColumnDS.LegendText = oddLineDS.LegendText = actionModel.actionone;
            evenColumnDS.LegendText = evenLineDS.LegendText = actionModel.actiontwo;

            maxMomentChart.Series.Add(oddColumnDS);
            maxMomentChart.Series.Add(evenColumnDS);

            maxMomentLineChart.Series.Add(oddLineDS);
            maxMomentLineChart.Series.Add(evenLineDS);
        }

        //刷新平均曲线
        private void RefrenshAvgCurveChart() {
            oddAvgCurveChart.Series.Clear();
            evenAvgCurveChart.Series.Clear();
            XElement oddEle = dataDoc.Descendants("odd").ElementAt(0);
            XElement evenEle = dataDoc.Descendants("even").ElementAt(0);
            XElement oddAvgEle = dataDoc.Descendants("oddavg").ElementAt(0);
            XElement evenAvgEle = dataDoc.Descendants("evenavg").ElementAt(0);

            //添加每次都曲线
            foreach (XElement xe in oddEle.Elements())
            {
                DataSeries ds = CreateAvgCurveSeries(xe);
                ds.Color = otherBrush;
                ds.ShowInLegend = false;
                oddAvgCurveChart.Series.Add(ds);
            }
            foreach (XElement xe in evenEle.Elements())
            {
                DataSeries ds = CreateAvgCurveSeries(xe);
                ds.ShowInLegend = false;
                ds.Color = otherBrush;
                evenAvgCurveChart.Series.Add(ds);
            }
            //添加平均曲线
            DataSeries oddAvgDS = CreateAvgCurveSeries(oddAvgEle);
            oddAvgDS.Color = oddBrush;
            oddAvgDS.LegendText = actionModel.actionone;
            oddAvgCurveChart.Series.Add(oddAvgDS);
            DataSeries evenAvgDS = CreateAvgCurveSeries(evenAvgEle);
            evenAvgDS.Color = evenBrush;
            evenAvgDS.LegendText = actionModel.actiontwo;
            evenAvgCurveChart.Series.Add(evenAvgDS); 
        }

        private DataSeries CreateSeries(List<double> list)
        {
            DataSeries ds = new DataSeries();
            ds.RenderAs = RenderAs.QuickLine;
            for (int i = 0; i < list.Count; i++)
            {
                DataPoint dp = new DataPoint() { YValue = Math.Abs(list[i]) };
                ds.DataPoints.Add(dp);
            }
            return ds;
        }

        private DataSeries CreateAvgCurveSeries(XElement rootEle) {
            List<XElement> list = rootEle.Elements().ToList<XElement>();
            DataSeries ds = new DataSeries();
            ds.RenderAs = RenderAs.QuickLine;
            double allCount = 100;
            double value = allCount / list.Count;
            for (int i = 0; i < list.Count; i++) {
                XElement xe = list[i];
                double d = Math.Abs(double.Parse(xe.Attribute("c2").Value)) * 0.1;
                DataPoint dp = new DataPoint() { YValue = d, XValue = i * value };
                ds.DataPoints.Add(dp);
            }
            return ds;
        }

        //关闭
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //导出报告
        private void btnExportReport_Click(object sender, RoutedEventArgs e)
        {
            //string fileName = "";

            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.Description = "选择保存报告的位置";
            fbd.ShowNewFolderButton = true;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                ExportReport(fbd.SelectedPath);
                MessageBox.Show("导出成功!", "系统信息");
            }
        }

        public void ExportReport(string savePath) {
            string fileName = savePath + "\\" + DataModel.Ath_Name + DataModel.DJoint + DataModel.DPlane.Replace('/', 'v') + DataModel.Speed1 + "(" + DataModel.BaseFileName + ")" + ".pdf";
            tab.SelectedIndex = 1;
            tab.UpdateLayout();
            tab.SelectedIndex = 2;
            tab.UpdateLayout();
            tab.SelectedIndex = 3;
            tab.UpdateLayout();
            tab.SelectedIndex = 0;
            tab.UpdateLayout();
            try
            {
                XDocument reportDoc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "\\AppTemplate\\report.xml");
                XElement titleEle = reportDoc.Descendants("title").ElementAt<XElement>(0);
                titleEle.Value = DataModel.Ath_Name + "单项测试报告";
                IEnumerable<XElement> tableEles = reportDoc.Descendants("table");
                XElement athInfoTable = tableEles.ElementAt<XElement>(0);
                IEnumerable<XElement> athInfoCells = athInfoTable.Descendants("cell");
                athInfoCells.ElementAt(0).Attribute("value").Value = DataModel.Ath_Name;
                athInfoCells.ElementAt(1).Attribute("value").Value = DataModel.Ath_Sex;
                athInfoCells.ElementAt(2).Attribute("value").Value = DataModel.Ath_Birthday.ToString("yyyy年MM月dd");
                athInfoCells.ElementAt(3).Attribute("value").Value = DataModel.Ath_Height;
                athInfoCells.ElementAt(4).Attribute("value").Value = DataModel.Ath_Weight;
                athInfoCells.ElementAt(5).Attribute("value").Value = DataModel.Ath_Project;
                athInfoCells.ElementAt(6).Attribute("value").Value = DataModel.Ath_MainProject;
                athInfoCells.ElementAt(7).Attribute("value").Value = DataModel.Ath_TrainYears;
                athInfoCells.ElementAt(8).Attribute("value").Value = DataModel.Ath_Level;
                athInfoCells.ElementAt(8).Attribute("value").Value = DataModel.Ath_Team;

                XElement testInfoEle = tableEles.ElementAt<XElement>(1);
                IEnumerable<XElement> testInfoCells = testInfoEle.Descendants("cell");
                testInfoCells.ElementAt(0).Attribute("value").Value = DataModel.Ath_TestAddress;
                testInfoCells.ElementAt(1).Attribute("value").Value = DataModel.Ath_TestState;
                testInfoCells.ElementAt(2).Attribute("value").Value = DataModel.Ath_TestDate.ToString("yyyy年MM月dd");

                testInfoCells.ElementAt(3).Attribute("value").Value = DataModel.DJoint;
                testInfoCells.ElementAt(4).Attribute("value").Value = DataModel.DJointSide;
                testInfoCells.ElementAt(5).Attribute("value").Value = DataModel.DTestMode;

                testInfoCells.ElementAt(6).Attribute("value").Value = DataModel.DPlane;
                testInfoCells.ElementAt(7).Attribute("value").Value = DataModel.MotionRange;
                testInfoCells.ElementAt(8).Attribute("value").Value = DataModel.Speed;

                testInfoCells.ElementAt(9).Attribute("value").Value = DataModel.Break;
                testInfoCells.ElementAt(10).Attribute("value").Value = DataModel.NOOfSets;
                testInfoCells.ElementAt(11).Attribute("value").Value = DataModel.NOOfRepetitions;

                testInfoCells.ElementAt(12).Attribute("value").Value = DataModel.Ath_Remark;

                //统计信息节点
                XElement statisticsInfoEle = tableEles.ElementAt<XElement>(2);
                IEnumerable<XElement> rowEles = statisticsInfoEle.Descendants("row");
                //填充动作1和动作2的名称
                XElement actionNameRowEle = rowEles.ElementAt(0);
                actionNameRowEle.Elements().ElementAt(1).Attribute("label").Value = actionModel.actionone;
                actionNameRowEle.Elements().ElementAt(3).Attribute("label").Value = actionModel.actiontwo;
                //填充测试次数
                XElement choosedTestCountRowEle = rowEles.ElementAt(2);

                XElement action1Ele = dataDoc.Descendants("action1").ElementAt(0);
                XElement action2Ele = dataDoc.Descendants("action2").ElementAt(0);

                choosedTestCountRowEle.Elements().ElementAt(1).Attribute("label").Value = action1Ele.Attribute("index").Value;
                choosedTestCountRowEle.Elements().ElementAt(3).Attribute("label").Value = action2Ele.Attribute("index").Value;

                IEnumerable<XElement> oddValues = action1Ele.Elements();
                IEnumerable<XElement> evenValue = action2Ele.Elements();
                for (int i = 3; i < rowEles.Count(); i++)
                {
                    IEnumerable<XElement> cellEles = rowEles.ElementAt(i).Elements();

                    XElement oddValueEle = oddValues.ElementAt(i - 3);
                    XElement evenValueEle = evenValue.ElementAt(i - 3);
                    cellEles.ElementAt(1).Attribute("label").Value = oddValueEle.Attribute("max").Value;
                    cellEles.ElementAt(2).Attribute("label").Value = oddValueEle.Attribute("avg").Value;
                    cellEles.ElementAt(3).Attribute("label").Value = evenValueEle.Attribute("max").Value;
                    cellEles.ElementAt(4).Attribute("label").Value = evenValueEle.Attribute("avg").Value;
                }

                SaveToImage(chartGrid, AppDomain.CurrentDomain.BaseDirectory + "progress.jpg");
                SaveToImage(maxMomentGrid, AppDomain.CurrentDomain.BaseDirectory + "maxcolumn.jpg");
                SaveToImage(maxMomentLineGrid, AppDomain.CurrentDomain.BaseDirectory + "maxline.jpg");
                SaveToImage(avgPanel, AppDomain.CurrentDomain.BaseDirectory + "avg.jpg");

                //ChartWindow chartWindow = new ChartWindow();
                //chartWindow.ProgressDS = progressChart.Series;
                //chartWindow.MaxColumnDS = maxMomentChart.Series;
                //chartWindow.MaxLineDS = maxMomentLineChart.Series;
                //chartWindow.OddAvgDS = oddAvgCurveChart.Series;
                //chartWindow.EvenAvgDS = evenAvgCurveChart.Series;
                //if (chartWindow.ShowDialog() == true) {

                //}

                exportReport = new ExportReport(reportDoc);
                exportReport.Export(fileName);
                //progressChart.Series = chartWindow.ProgressDS;
                //maxMomentChart.Series = chartWindow.MaxColumnDS;
                //maxMomentLineChart.Series = chartWindow.MaxLineDS;
                //oddAvgCurveChart.Series = chartWindow.OddAvgDS;
                //evenAvgCurveChart.Series = chartWindow.EvenAvgDS;

                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "progress.jpg");
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "maxcolumn.jpg");
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "maxline.jpg");
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "avg.jpg");
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message, "系统错误");
            }
        }

        ////保存图标为图片
        private void SaveToImage(FrameworkElement ui, string fileName)
        {
            System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create);
            int imgWidth = int.Parse(ui.Width.ToString()) * 4;
            int imgHeight = int.Parse(ui.Height.ToString()) * 4;
            RenderTargetBitmap bmp = new RenderTargetBitmap(imgWidth, imgHeight, 384, 384, PixelFormats.Pbgra32);
            bmp.Render(ui);
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            encoder.Save(fs);
            fs.Close();
        }

        //测试次数选择和不选择
        private void testCountCB_Checked(object sender, RoutedEventArgs e)
        {
            List<Model.TestCountModel> oddCheckedCountModel = oddTestCountList.FindAll(x => x.IsChecked == true);
            List<Model.TestCountModel> evenCheckedCountModel = evenTestCountList.FindAll(x => x.IsChecked == true);
            if (oddCheckedCountModel.Count == 0||evenCheckedCountModel.Count==0) {
                MessageBox.Show("奇数或偶数测试次数都应至少保留一个！", "系统信息");
                (sender as CheckBox).IsChecked = true;
                return;
            }
            string oddCheckedCount = "";
            for (int i = 0; i < oddCheckedCountModel.Count; i++) {
                oddCheckedCount += oddCheckedCountModel[i].Count;
                if (i != oddCheckedCountModel.Count - 1) {
                    oddCheckedCount += ",";
                }
            }
            string evenCheckedCount = "";
            for (int i = 0; i < evenCheckedCountModel.Count; i++)
            {
                evenCheckedCount += evenCheckedCountModel[i].Count;
                if (i != evenCheckedCountModel.Count - 1)
                {
                    evenCheckedCount += ",";
                }
            }

            dataDoc.Descendants("action1").ElementAt(0).Attribute("index").Value = oddCheckedCount;
            dataDoc.Descendants("action2").ElementAt(0).Attribute("index").Value = evenCheckedCount;

            dataDoc.Save(AppDomain.CurrentDomain.BaseDirectory + "\\AppData\\XmlData\\" + DataModel.DataFileName);

            BaseDataUtil du = new BaseDataUtil();
            du.Weight = DataModel.Ath_Weight;
            if (DataModel.Gravitycomp.Trim() != "") {
                du.Gravitycomp = DataModel.Gravitycomp;
            }
       
            du.ComputeParams(AppDomain.CurrentDomain.BaseDirectory + "\\AppData\\XmlData\\" + DataModel.DataFileName);

            RefrenshChart();
        }

        //校正数据
        private void btnCorrectData_Click(object sender, RoutedEventArgs e)
        {
            CorrectDataWindow cdw = new CorrectDataWindow();
            cdw.FileName = DataModel.DataFileName;
            cdw.Owner = Application.Current.MainWindow;
            if (cdw.ShowDialog() == true) {
                BaseDataUtil du = new BaseDataUtil();
                du.Weight = DataModel.Ath_Weight;
                if (DataModel.Gravitycomp.Trim() != "")
                {
                    du.Gravitycomp = DataModel.Gravitycomp;
                }
                du.ComputeParams(AppDomain.CurrentDomain.BaseDirectory + "\\AppData\\XmlData\\" + DataModel.DataFileName);

                RefrenshChart();
            }
        }

        //拖动
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Point mousePoint = e.GetPosition(this);

                if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed && e.ChangedButton == System.Windows.Input.MouseButton.Left && mousePoint.Y <= 30)
                {

                    this.DragMove();
                }
            }
            catch { }
        }

        //最大化
        private void btnMax_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                double screenHeight = SystemParameters.PrimaryScreenHeight;
                double screenWidth = SystemParameters.PrimaryScreenWidth;

                this.Left = (screenWidth - this.Width) / 2;
                this.Top = (screenHeight - this.Height) / 2;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void btnSmooth_Click(object sender, RoutedEventArgs e)
        {
            SmoothDataWindow smoothDataWindow = new SmoothDataWindow();
            smoothDataWindow.DataFileName = DataModel.DataFileName;
            smoothDataWindow.Owner = Application.Current.MainWindow;
            if (smoothDataWindow.ShowDialog() == true) {
                BaseDataUtil du = new BaseDataUtil();
                du.Weight = DataModel.Ath_Weight;
                if (DataModel.Gravitycomp.Trim() != "")
                {
                    du.Gravitycomp = DataModel.Gravitycomp;
                }
                du.ComputeParams(AppDomain.CurrentDomain.BaseDirectory + "\\AppData\\XmlData\\" + DataModel.DataFileName);

                RefrenshChart();
            }
        }

    }
}
