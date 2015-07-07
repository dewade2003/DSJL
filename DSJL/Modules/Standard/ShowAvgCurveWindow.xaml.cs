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
using DSJL.DataUtil;

namespace DSJL.Modules.Standard
{
    /// <summary>
    /// ShowAvgCurveWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ShowAvgCurveWindow : Window
    {
        private Chart chart1;
        private Chart chart2;
        private List<Model.TestInfoModel> modelList;
        private Model.TB_StandardInfo standardInfo;

        SolidColorBrush avgBrush = new SolidColorBrush(Color.FromRgb(61, 201, 54));//平均曲线颜色

        public ShowAvgCurveWindow()
        {
            InitializeComponent();
            chart1 = new Chart();
            chart2 = new Chart();
            chart2.ScrollingEnabled = chart1.ScrollingEnabled = false;
            grid1.Children.Add(chart1);
            grid2.Children.Add(chart2);
        }

        /// <summary>
        /// 测试信息列表
        /// </summary>
        public List<Model.TestInfoModel> ModelList {
            set {
                modelList = value;
            }
        }

        /// <summary>
        /// 测试标准信息
        /// </summary>
        public Model.TB_StandardInfo StandardInfo {
            set {
                standardInfo = value;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = tbTitle.Text = standardInfo.Stand_Name + "的平均曲线";
            XDocument xdoc;
            List<List<XElement>> oddList = new List<List<XElement>>();
            List<List<XElement>> evenList = new List<List<XElement>>();
            foreach (Model.TestInfoModel model in modelList) {
                string xmlFileName = Model.AppPath.RootPath + "\\AppData\\XmlData\\" + model.DataFileName;
                xdoc = XDocument.Load(xmlFileName);
                XElement oddAvgEle = xdoc.Descendants("oddavg").ElementAt(0);
                XElement evenAvgEle = xdoc.Descendants("evenavg").ElementAt(0);

                List<XElement> list1 = new List<XElement>();
                List<XElement> list2 = new List<XElement>();

                foreach (XElement xe in oddAvgEle.Elements()) {
                    list1.Add(xe);
                }

                foreach (XElement xe in evenAvgEle.Elements()) {
                    list2.Add(xe);
                }

                oddList.Add(list1);
                evenList.Add(list2);
            }

            List<List<double>> oddValueList = ComputeAvgCurve.Compute(oddList);
            List<List<double>> evenValueList = ComputeAvgCurve.Compute(evenList);

            List<double> oddAvgValueList = ComputeAvgCurve.ComputeAvg(oddValueList);
            List<double> evenAvgValueList = ComputeAvgCurve.ComputeAvg(evenValueList);

            AddSeries(chart1, oddAvgValueList,true,"动作1");//平均曲线
            foreach (List<double> list in oddValueList) {
                AddSeries(chart1, list,false,"");
            }

            AddSeries(chart2, evenAvgValueList,true,"动作2");
            foreach (List<double> list in evenValueList) {
                AddSeries(chart2, list,false,"");
            }
        }

        private void AddSeries(Chart c, List<double> list,bool showInLengend,string lengendText) {
            DataSeries ds = new DataSeries();
            ds.ShowInLegend = showInLengend;
            ds.RenderAs = RenderAs.QuickLine;
            if (showInLengend) {
                ds.Color = avgBrush;
                ds.LegendText = lengendText;
            }
            for (int i = 0; i < list.Count; i++)
            {
                DataPoint dp = new DataPoint() { YValue = Math.Abs(list[i]) };
                ds.DataPoints.Add(dp);
            }
            c.Series.Add(ds);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

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
    }
}
