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
using System.IO;

namespace DSJL.Modules.Test
{
    /// <summary>
    /// SmoothDataWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SmoothDataWindow : Window
    {

        private string momentColumn = "c2";//力矩使用列
        private string dataFileName, tempDataFileName, smoothDataFileName;
        private Chart chart;

        private XDocument xdoc;

        private SolidColorBrush startPointBrush = new SolidColorBrush(Color.FromRgb(27, 91, 20));//起点颜色
        private SolidColorBrush blackBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));//黑色点
        private SolidColorBrush oddPointBrush = new SolidColorBrush(Color.FromRgb(131, 169, 254));//动作1的点的点颜色，淡蓝色
        private SolidColorBrush evenPointBrush = new SolidColorBrush(Color.FromRgb(254, 116, 116));//动作2的点点颜色，淡红色
        private SolidColorBrush dataseriesBrush = new SolidColorBrush(Color.FromRgb(198, 198, 198));//线条的颜色，灰色

        private DataUtil.BaseDataUtil baseDataUtil = new DataUtil.BaseDataUtil();

        private double smoothValue = 0;

        public SmoothDataWindow()
        {
            InitializeComponent();
            chart = new Chart();
            chart.BorderThickness = new Thickness(0, 0, 1, 0);
        }

        /// <summary>
        /// 数据文件名称
        /// </summary>
        public string DataFileName
        {
            set
            {
                dataFileName = AppDomain.CurrentDomain.BaseDirectory + "\\AppData\\XmlData\\" + value;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tempDataFileName = Model.AppPath.DataPath + "temp" + ".xml";
            smoothDataFileName = Model.AppPath.DataPath + "temp-smooth" + ".xml";
            File.Copy(dataFileName, tempDataFileName, true);
            File.Copy(dataFileName, smoothDataFileName, true);

            xdoc = XDocument.Load(tempDataFileName);

            XElement datasEle = xdoc.Descendants("datas").ElementAt(0);
            XAttribute smoothValueAtt = datasEle.Attribute("smoothvalue");
            if (smoothValueAtt != null)
            {
                smoothValue = double.Parse(smoothValueAtt.Value);
                smoothSlider.Value = smoothValue;
                tbCurrentSmoothValue.Text = "该数据平滑处理状态，已平滑" + smoothValueAtt.Value + "hz";
            }
            else
            {
                tbCurrentSmoothValue.Text = "该数据平滑状态，未平滑";
            }

            chart.AnimatedUpdate = false;
            chart.ScrollingEnabled = true;
            chart.AnimationEnabled = false;
            chart.ZoomingMode = ZoomingMode.MouseDragAndWheel;
            chart.ZoomingEnabled = true;
            chart.ShowAllText = "显示所有";
            chart.ZoomOutText = "缩小";


            Axis xAxis = new Axis();
            xAxis.Suffix = "(ms)";
            chart.AxesX.Add(xAxis);

            Axis yAxis = new Axis();
            yAxis.Suffix = "(Nm)";
            yAxis.AxisMinimum = -10;
            chart.AxesY.Add(yAxis);

            grid.Children.Add(chart);

            RefrenshChart();
        }

        private void RefrenshChart()
        {
            xdoc = XDocument.Load(smoothDataFileName);

            XElement datasEle = xdoc.Descendants("datas").ElementAt(0);
            int testCount = int.Parse(datasEle.Attribute("testcount").Value);//总测试次数

            List<XElement> dataList = xdoc.Descendants("data").ToList<XElement>();

            Binding b = new Binding() { Source = dataList };
            lv.SetBinding(ListView.ItemsSourceProperty, b);

            DataSeries ds = new DataSeries();
            ds.RenderAs = RenderAs.Line;
            ds.Color = dataseriesBrush;
            ds.LineThickness = 1;

            foreach (XElement xe in dataList)
            {
                DataPoint dp = new DataPoint() { YValue = Math.Abs(int.Parse(xe.Attribute(momentColumn).Value)) * 0.1, XValue = double.Parse(xe.Attribute("c0").Value) };

                int testIndex = int.Parse(xe.Attribute("c5").Value);
                if (testIndex % 2 == 0)
                {
                    dp.Color = evenPointBrush;
                }
                else
                {
                    dp.Color = oddPointBrush;
                }

                dp.UseLayoutRounding = true;

                ds.DataPoints.Add(dp);
            }
            XElement datasElement = xdoc.Descendants("datas").ElementAt(0);
            int startIndex = int.Parse(datasElement.Attribute("startindex").Value);
            ds.DataPoints[startIndex].Color = startPointBrush;

            chart.Series.Clear();
            chart.Series.Add(ds);
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

        //保存关闭
        private void btnSaveClose_Click(object sender, RoutedEventArgs e)
        {
            if (smoothValue != smoothSlider.Value)
            {
                string desc = string.Format("您将平滑系数从{0}hz提高到了{1}hz，该操作为不可逆操作。\r\n确定变更请点击\"是\"\r\n取消变更请点击\"否\"\r\n重新调整请点击\"取消\"", smoothValue, smoothSlider.Value);
                MessageBoxResult mbResult = MessageBox.Show(desc, "系统信息", MessageBoxButton.YesNoCancel);

                switch (mbResult)
                {
                    case MessageBoxResult.Yes:
                        XElement datasEle = xdoc.Descendants("datas").ElementAt(0);
                        XAttribute smoothValueAtt = datasEle.Attribute("smoothvalue");
                        smoothValueAtt.Value = smoothSlider.Value.ToString();
                        xdoc.Save(smoothDataFileName);
                        File.Copy(smoothDataFileName, dataFileName, true);
                        this.DialogResult = true;
                        DeleteTempFile();
                        this.Close();
                        break;
                    case MessageBoxResult.No:
                        this.DialogResult = false;
                        DeleteTempFile();
                        this.Close();
                        break;
                }
            }
            else
            {
                this.DialogResult = false;
                DeleteTempFile();
                this.Close();
            }

        }

        //取消关闭
        private void btnCancleClose_Click(object sender, RoutedEventArgs e)
        {
            if (smoothValue != smoothSlider.Value)
            {
                if (MessageBox.Show("确定要取消对数据的编辑吗？\r\n点击“确定”本次对数据的编辑将不会被应用。\r\n如果要应用本次编辑，请返回并点击“保存并关闭”按钮", "系统信息", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    DeleteTempFile();
                    this.DialogResult = false;
                    this.Close();
                }
            }
            else
            {
                DeleteTempFile();
                this.DialogResult = false;
                this.Close();
            }
        }

        //删除数据文件的拷贝
        private void DeleteTempFile()
        {
            File.Delete(tempDataFileName);
            File.Delete(smoothDataFileName);
        }

        //平滑
        private void smoothSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue >= smoothValue)
            {
                baseDataUtil.SmoothValue = (int)e.NewValue - (int)smoothValue;
                baseDataUtil.Smooth(tempDataFileName, smoothDataFileName);
                RefrenshChart();
            }
            else
            {
                smoothSlider.Value = e.OldValue;
            }
        }
    }
}
