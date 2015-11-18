using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Visifire.Charts;
using System.Xml.Linq;
using DSJL.DataUtil;
using WPFHelper.Window;
using System.IO;

namespace DSJL.Modules.Test
{
    /// <summary>
    /// AvgCurveCompare.xaml 的交互逻辑
    /// </summary>
    public partial class AvgCurveCompareWindow : Window
    {
        private BLL.TB_Dict dictBLL;

        BLL.TB_TestInfo testInfoBLL = new BLL.TB_TestInfo();
        BLL.TB_AthleteInfo athleteInfoBLL = new BLL.TB_AthleteInfo();
        BLL.TB_StandTestRefe refeBLL = new BLL.TB_StandTestRefe();
        //List<Model.TestInfoModel> testInfoModelList = new List<Model.TestInfoModel>();

        List<Model.TestInfoModel> standTestInfoModelList = new List<Model.TestInfoModel>();

        private Model.TestInfoModel testInfoModel;
        //private Model.TB_Dict actionModel;
        private Chart chartOdd;
        private Chart chartEven;

        private XDocument xdoc;
        private XDocument avgDoc;

        //SolidColorBrush oddBrush = new SolidColorBrush(Color.FromRgb(44, 90, 222));//奇数线条颜色
        SolidColorBrush standBrush = new SolidColorBrush(Color.FromRgb(178, 44, 222));//偶数线条颜色

        SolidColorBrush avgBrush = new SolidColorBrush(Color.FromRgb(61, 201, 54));//平均曲线颜色

        public Model.TestInfoModel InfoModel
        {
            set
            {
                testInfoModel = value;
            }
        }

        /// <summary>
        /// 测试信息列表
        /// </summary>
        public List<Model.TestInfoModel> TestInfoModelList
        {
            get;
            set;
        }

        public AvgCurveCompareWindow()
        {
            InitializeComponent();
            WindowHelper.RepairWindowBehavior(this);

            dictBLL = new BLL.TB_Dict();

            chartOdd = new Chart();
            chartEven = new Chart();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //重新定义序号

            List<Model.TestInfoModel> timList = new List<Model.TestInfoModel>();
            for (int i = 0; i < TestInfoModelList.Count; i++)
            {
                Model.TestInfoModel tim = (Model.TestInfoModel)TestInfoModelList[i].Clone();
                tim.Index = i + 1;
                timList.Add(tim);
            }
            TestInfoModelList = timList;

            dgTestInfo.ItemsSource = TestInfoModelList;
            //初始化
            stand.ItemSelectionChangedEvent += new Compoments.TestStandardManager.ItemSelectionChangedDelegate(stand_ItemSelectionChangedEvent);

            tbTitle.Text = "平均曲线对比";

            //添加图表
            chartOdd.BorderThickness = chartEven.BorderThickness = new Thickness(0, 0, 0, 0);
            chartOdd.ScrollingEnabled = chartEven.ScrollingEnabled = false;

            //添加坐标轴
            Axis yAxisOAC = new Axis();
            Axis yAxisEAC = new Axis();
            yAxisOAC.Suffix = yAxisEAC.Suffix = "(Nm)";
            chartOdd.AxesY.Add(yAxisOAC);
            chartEven.AxesY.Add(yAxisEAC);

            Axis xAxisOAC = new Axis();
            Axis xAxisEAC = new Axis();
            xAxisEAC.AxisMinimum = xAxisOAC.AxisMinimum = 0;
            xAxisOAC.AxisMaximum = xAxisEAC.AxisMaximum = 100;
            xAxisEAC.Suffix = xAxisOAC.Suffix = "%";

            chartOdd.AxesX.Add(xAxisOAC);
            chartEven.AxesX.Add(xAxisEAC);

            gridOdd.Children.Add(chartOdd);
            gridEven.Children.Add(chartEven);

            RefrenshChart();
        }
        //参考值选择改变事件，刷新图表
        private void stand_ItemSelectionChangedEvent(Model.TB_StandardInfo selectedItem)
        {
            if (stand.SelectedItem != null)
            {
                standTestInfoModelList.Clear();
                standTestInfoModelList = Caches.Util.AthTestInfoModelUtil.AthTestUtil(refeBLL.GetStandTestInfoModelList(stand.SelectedItem.ID));
                RefrenshChart();
            }

        }

        private void RefrenshChart()
        {
            chartOdd.Series.Clear();
            chartEven.Series.Clear();

            //标准数据
            List<List<XElement>> oddStandList = new List<List<XElement>>();
            List<List<XElement>> evenStandList = new List<List<XElement>>();

            //计算平均曲线
            if (stand.SelectedItem != null)
            {
                if (standTestInfoModelList.Count > 0)
                { //检查测试标准里是否有测试信息
                    foreach (Model.TestInfoModel model in standTestInfoModelList)
                    {
                        string xmlFileName = Model.AppPath.RootPath + "\\AppData\\XmlData\\" + model.DataFileName;
                        avgDoc = XDocument.Load(xmlFileName);
                        XElement oddAvgEle1 = avgDoc.Descendants("oddavg").ElementAt(0);
                        XElement evenAvgEle1 = avgDoc.Descendants("evenavg").ElementAt(0);

                        List<XElement> list1 = oddAvgEle1.Elements().ToList<XElement>();
                        oddStandList.Add(list1);

                        if (model.Test_Mode != "6")
                        {//如果为等长测试不加载偶数测试
                            List<XElement> list2 = evenAvgEle1.Elements().ToList<XElement>();
                            evenStandList.Add(list2);
                        }

                    }
                }
            }
            List<List<XElement>> oddList = new List<List<XElement>>();
            List<List<XElement>> evenList = new List<List<XElement>>();
            for (int i = 0; i < TestInfoModelList.Count; i++)
            {
                xdoc = XDocument.Load(Model.AppPath.XmlDataDirPath + TestInfoModelList[i].DataFileName);

                XElement oddAvgEle = xdoc.Descendants("oddavg").ElementAt(0);
                XElement evenAvgEle = xdoc.Descendants("evenavg").ElementAt(0);

                List<XElement> oddEles = oddAvgEle.Elements().ToList<XElement>();
                oddList.Add(oddEles);
                if (TestInfoModelList[i].Test_Mode != "6")
                {//如果为等长测试不加载偶数测试
                    List<XElement> evenEles = evenAvgEle.Elements().ToList<XElement>();
                    evenList.Add(evenEles);
                }
            }

            MergeAndAddDS(oddStandList, oddList, chartOdd);
            MergeAndAddDS(evenStandList, evenList, chartEven);
        }

        private void MergeAndAddDS(List<List<XElement>> standList, List<List<XElement>> list, Chart chart)
        {
            //合并标准和选择的信息
            List<List<XElement>> allList = new List<List<XElement>>();
            allList.AddRange(standList);
            allList.AddRange(list);
            if (allList.Count > 0)
            {
                //把所有曲线计算成相同长度
                List<List<double>> allValueList = ComputeAvgCurve.Compute(allList);

                List<List<double>> standValueList = new List<List<double>>();
                List<List<double>> checkedValueList = new List<List<double>>();//选择的测试信息的点列表
                //取出标准信息的数据,并创建选择的测试信息的曲线
                for (int i = 0; i < allValueList.Count; i++)
                {
                    if ((i + 1) <= standList.Count)
                    {
                        standValueList.Add(allValueList[i]);
                    }
                    else
                    {
                        checkedValueList.Add(allValueList[i]);
                        Model.TestInfoModel mo = TestInfoModelList[i - standList.Count];
                        if (mo.IsChecked == true)
                        {
                            string leText = string.Format("{0}.{1}", mo.Index, mo.Ath_Name);
                            chart.Series.Add(CreateSeries(allValueList[i], leText));//补全写入legendtest
                        }

                    }
                }
                //添加选择的信息的平均曲线
                List<double> checkedInfoAvgValueList = ComputeAvgCurve.ComputeAvg(checkedValueList);
                DataSeries avgds = CreateSeries(checkedInfoAvgValueList, "平均曲线");
                avgds.Color = avgBrush;
                chart.Series.Add(avgds);

                //添加标准曲线
                if (standValueList.Count > 0)
                {
                    List<double> avgValueList = ComputeAvgCurve.ComputeAvg(standValueList);
                    DataSeries standds = CreateSeries(avgValueList, stand.SelectedItem.Stand_Name);
                    standds.Color = standBrush;
                    chart.Series.Add(standds);
                }
            }

        }

        private DataSeries CreateSeries(List<double> list, string legendTest)
        {
            double allCount = 100;
            double value = allCount / list.Count;//增量
            DataSeries ds = new DataSeries();
            ds.RenderAs = RenderAs.QuickLine;
            ds.LegendText = legendTest;
            for (int i = 0; i < list.Count; i++)
            {
                DataPoint dp = new DataPoint() { YValue = Math.Abs(list[i]) * 0.1, XValue = i * value };

                ds.DataPoints.Add(dp);
            }
            return ds;
        }

        public static List<double> InsertSomePoint(List<XElement> sourceEles, int count)
        {
            List<double> resultList = new List<double>();
            for (int j = 0; j < sourceEles.Count - 1; j++)
            {
                double d1 = double.Parse(sourceEles[j].Attribute("c2").Value);
                double d2 = double.Parse(sourceEles[j + 1].Attribute("c2").Value);
                double allowance = ComputeAllowance(d1, d2, count + 1);
                resultList.Add(d1);
                for (int a = 0; a < count; a++)
                {
                    if (allowance > 0)
                    {
                        resultList.Add(d1 - (a + 1) * allowance);
                    }
                    else
                    {
                        resultList.Add(d1 + (a + 1) * Math.Abs(allowance));
                    }
                }
            }
            resultList.Add(double.Parse(sourceEles[sourceEles.Count - 1].Attribute("c2").Value));

            return resultList;
        }

        //计算公差
        public static double ComputeAllowance(double startValue, double endValue, int count)
        {
            double value = 0;
            value = (Math.Abs(startValue) - Math.Abs(endValue)) / count;
            return value;
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

        //关闭
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //最小化
        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
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

        //数据列表单击
        private void dgTestInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgTestInfo.SelectedIndex >= 0)
            {
                foreach (Model.TestInfoModel model in TestInfoModelList)
                {
                    model.IsChecked = false;
                }
                if (dgTestInfo.SelectedItems.Count > 1)
                {
                    foreach (var item in dgTestInfo.SelectedItems)
                    {
                        Model.TestInfoModel testModel = item as Model.TestInfoModel;
                        testModel.IsChecked = true;
                    }
                }
                else
                {
                    Model.TestInfoModel testModel = dgTestInfo.SelectedItem as Model.TestInfoModel;
                    testModel.IsChecked = true;
                }

                RefrenshChart();
            }
        }

        //全选
        private void checkAll_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.IsChecked == true)
            {
                foreach (Model.TestInfoModel bi in TestInfoModelList)
                {
                    bi.IsChecked = true;
                }
                dgTestInfo.SelectAll();
            }
            else
            {
                foreach (Model.TestInfoModel bi in TestInfoModelList)
                {
                    bi.IsChecked = false;
                }
                dgTestInfo.UnselectAll();
            }
        }

        //导出报告
        private void btnExportReport_Click(object sender, RoutedEventArgs e)
        {

            //foreach (Model.TestInfoModel model in TestInfoModelList)
            //{
            //    model.IsChecked = true;
            //}
            //RefrenshChart();
            try
            {
                //生成数据xml对象，供导出报告使用
                //string title = "平均曲线对比-" + DateTime.Now.ToString("yyyyMMddHHmmss");

                //DSJL.Export.GenerateAvgReportXML garxml = new DSJL.Export.GenerateAvgReportXML();
                //garxml.CurrentTitle = "平均曲线对比报告";
                //garxml.TestInfoModelList = TestInfoModelList;
                //System.Xml.Linq.XDocument xdoc = garxml.Generate();

                //System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
                //fbd.Description = "选择保存报告的位置";
                //fbd.ShowNewFolderButton = true;
                //if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                //{
                //    //生成平均曲线图片
                //    DSJL.Export.SaveUIElementToImage.SaveToImage(avgPanel, AppDomain.CurrentDomain.BaseDirectory + "avg.jpg");
                //    //开始导出
                //    DSJL.Export.ExportAvgReport exportReport = new DSJL.Export.ExportAvgReport(xdoc);
                //    exportReport.Export(fbd.SelectedPath + "\\" + title + ".pdf");

                //    //删除平均曲线图片
                //    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "avg.jpg");
                //    MessageBox.Show("导出成功！", "系统信息");
                //}
                List<Model.TestInfoModel> testInfoModelList = GetSortedTestInfoModelList();

                string pdfFileName = "";
                string reportTitle = "";
                string standName = "";
                DSJL.Export.ExportModeEnum exportMode = DSJL.Export.ExportModeEnum.Mode1;
                if (rbMode1.IsChecked == true)//互相对比报告
                {
                    exportMode = DSJL.Export.ExportModeEnum.Mode1;
                    pdfFileName = "等速肌力互相对比报告(" + DateTime.Now.ToString("yyyyMMddHHmmss") + ")";
                    reportTitle = "等速肌力互相对比报告";
                }
                else if (rbMode2.IsChecked == true)//与平均曲线对比报告
                {
                    exportMode = DSJL.Export.ExportModeEnum.Mode2;
                    pdfFileName = "等速肌力个人与平均曲线报告(" + DateTime.Now.ToString("yyyyMMddHHmmss") + ")";
                    reportTitle = "等速肌力个人与平均曲线报告";
                }
                else if (rbMode3.IsChecked == true)//与参考值对比报告
                {
                    if (stand.SelectedItem == null)
                    {
                        MessageBox.Show("请选择测试参考值！", "系统信息");
                        return;
                    }
                    else
                    {
                        standName = stand.SelectedItem.Stand_Name;
                    }
                    exportMode = DSJL.Export.ExportModeEnum.Mode3;

                    BLL.TB_StandardInfo standInfoBLL = new BLL.TB_StandardInfo();
                    Model.TB_StandardInfo parentStandInfo = standInfoBLL.GetModel((int)stand.SelectedItem.Stand_ParentID);
                    foreach (Model.TestInfoModel testInfoModel in testInfoModelList) {
                        pdfFileName += testInfoModel.Ath_Name;
                    }
                    pdfFileName += "与" + parentStandInfo.Stand_Name + stand.SelectedItem.Stand_Name + "对比报告(" + DateTime.Now.ToString("yyyyMMddHHmmss") + ")";
                    //pdfFileName = "等速肌力个人与参考值对比报告(" + DateTime.Now.ToString("yyyyMMddHHmmss") + ")";
                    reportTitle = "等速肌力个人与参考值对比报告";
                }

                System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
                saveFileDialog.AddExtension = true;
                saveFileDialog.OverwritePrompt = true;
                saveFileDialog.Title = "选择保存报告的位置";
                saveFileDialog.FileName = pdfFileName;
                saveFileDialog.DefaultExt = "pdf";
                saveFileDialog.Filter = "PDF文件(*.pdf)|*.pdf";
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                   
                    //生成数据xml对象，供导出报告使用
                    DSJL.Export.GenerateCompareResportXml garxml = new DSJL.Export.GenerateCompareResportXml(exportMode);
                    garxml.CurrentTitle = reportTitle;
                    garxml.TestInfoModelList = GetSortedTestInfoModelList();
                    garxml.StandardTestInfoModelList = standTestInfoModelList;
                    garxml.StandName = standName;
                    System.Xml.Linq.XDocument xdoc = garxml.GenerateXDoc();


                    //生成平均曲线图片
                    DSJL.Export.SaveUIElementToImage.SaveToImage(avgPanel, AppDomain.CurrentDomain.BaseDirectory + "avg.jpg");
                    //开始导出
                    DSJL.Export.ExportCompareReport exportReport = new DSJL.Export.ExportCompareReport(xdoc);
                    exportReport.Export(saveFileDialog.FileName);

                    //删除平均曲线图片
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "avg.jpg");
                    MessageBox.Show("导出成功！", "系统信息");
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("导出出错！\r\n" + ee.Message, "系统错误");
            }
        }

        /// <summary>
        ///
        /// </summary>
        private List<Model.TestInfoModel> GetSortedTestInfoModelList()
        {
            List<Model.TestInfoModel> models = new List<Model.TestInfoModel>();
            foreach (var item in dgTestInfo.Items)
            {
                Model.TestInfoModel tim = item as Model.TestInfoModel;
                //if (tim.IsChecked) {
                models.Add(tim);
                //}
            }
            return models;
        }

    }
}
