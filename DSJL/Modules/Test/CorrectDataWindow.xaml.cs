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
using System.Windows.Controls.Primitives;
using System.IO;

namespace DSJL.Modules.Test
{
    /// <summary>
    /// CorrectDataWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CorrectDataWindow : Window
    {
        private string momentColumn = "c2";//力矩使用列
        private Chart chart;
        private string fileName;
        private string tempFileName;
        private XDocument xdoc;

        private SolidColorBrush startEndPointBrush = new SolidColorBrush(Color.FromRgb(27, 91, 20));//起点终点颜色
        private SolidColorBrush blackBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));//黑色点
        private SolidColorBrush oddPointBrush = new SolidColorBrush(Color.FromRgb(44, 90, 222));//动作1的点的点颜色，蓝色
        private SolidColorBrush evenPointBrush = new SolidColorBrush(Color.FromRgb(178, 44, 222));//动作2的点点颜色，紫色
        private SolidColorBrush dataseriesBrush = new SolidColorBrush(Color.FromRgb(198, 198, 198));//线条的颜色，灰色
        private SolidColorBrush prePressedPointBrush = null;//上次点击的点的点颜色

        ContextMenu dpMenu = new System.Windows.Controls.ContextMenu();

        List<XElement> dataList;

        DataPoint startDataPoint, endDataPoint,checkStartDataPoint, checkEndDataPoint, tempDataPoint;

        XElement datasElement;

        MenuItem miSetStart = new MenuItem();
        MenuItem miSetEnd = new MenuItem();
        MenuItem miNextStart;//设为下一次起点菜单
        MenuItem miPreEnd;//设为上一次终点菜单
        MenuItem miCheckStart = new MenuItem();
        MenuItem miCheckEnd = new MenuItem();

        int testCount = 0;//测试总次数

        private TrendLine tlStart = new TrendLine();
        TrendLine tlEnd = new TrendLine();
        private TrendLine tlSelectedPoint = new TrendLine();

        public CorrectDataWindow()
        {
            InitializeComponent();
            chart = new Chart();
            chart.BorderThickness = new Thickness(0, 0, 1, 0);

            tlEnd.LineThickness= tlStart.LineThickness = 1.5;
            tlSelectedPoint.LineThickness = 0.5;
            tlEnd.LineColor=tlEnd.LabelFontColor= tlStart.LineColor = tlStart.LabelFontColor = new SolidColorBrush(Colors.Green);
            tlSelectedPoint.LineColor = tlSelectedPoint.LabelFontColor = new SolidColorBrush(Colors.Black);
            tlEnd.Background= tlStart.Background = null;
            tlEnd.Orientation= tlSelectedPoint.Orientation = tlStart.Orientation = Orientation.Vertical;
            tlStart.LabelText = "起点";
            tlEnd.LabelText = "终点";

            MenuItem miGroup1 = new MenuItem();
            miGroup1.Header = "测试起终点判定";

            MenuItem miGroup2 = new MenuItem();
            miGroup2.Header = "数据平滑处理";

            MenuItem miGroup3 = new MenuItem();
            miGroup3.Header = "单次起终点校准";

            miSetStart.Header = "设为测试起点";
            miSetEnd.Header = "设为测试终点";
            miGroup1.Items.Add(miSetStart);
            miGroup1.Items.Add(miSetEnd);

           
            miCheckStart.Header = "设为平滑处理起点";
            miCheckEnd.Header = "设为平滑处理终点";
            miGroup2.Items.Add(miCheckStart);
            miGroup2.Items.Add(miCheckEnd);

            miNextStart = new MenuItem();
            miNextStart.Header = "设为下一次起点";
            miPreEnd = new MenuItem();
            miPreEnd.Header = "设为上一次终点";
            miGroup3.Items.Add(miNextStart);
            miGroup3.Items.Add(miPreEnd);
         

            dpMenu.Items.Add(miGroup1);
            Separator sp = new Separator();
            dpMenu.Items.Add(sp);
            dpMenu.Items.Add(miGroup2);
            Separator sp1 = new Separator();
            dpMenu.Items.Add(sp1);
            dpMenu.Items.Add(miGroup3);

            miSetStart.Click += new RoutedEventHandler(miSetStart_Click);
            miSetEnd.Click += btnSetEnd_Click;
            miCheckStart.Click += new RoutedEventHandler(checkStart_Click);
            miCheckEnd.Click += new RoutedEventHandler(checkEnd_Click);

            miPreEnd.Click += new RoutedEventHandler(miPreEnd_Click);
            miNextStart.Click += new RoutedEventHandler(miNextStart_Click);
        }

        /// <summary>
        /// 是否重力补偿，1代表是
        /// </summary>
        public string Gravitycomp {
            set {
                if (value.Trim() == "1") {
                    momentColumn = "c2";
                }
            }
        }

        /// <summary>
        /// 数据文件名称
        /// </summary>
        public string FileName {
            set {
                fileName = AppDomain.CurrentDomain.BaseDirectory + "\\AppData\\XmlData\\" + value;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tempFileName = Model.AppPath.DataPath + "temp" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
            File.Copy(fileName, tempFileName, true);

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

            //grid.MouseRightButtonUp += new MouseButtonEventHandler(dp_MouseRightButtonUp);//右键功能已取消

            RefrenshChart();
        }

        private void RefrenshChart() {
            xdoc = XDocument.Load(tempFileName);

            XElement datasEle = xdoc.Descendants("datas").ElementAt(0);
            testCount = int.Parse(datasEle.Attribute("testcount").Value);//总测试次数
            dataList = xdoc.Descendants("data").ToList<XElement>();

            Binding b = new Binding() { Source = dataList };
            lv.SetBinding(ListView.ItemsSourceProperty, b);

            DataSeries ds = new DataSeries();
            ds.RenderAs = RenderAs.Spline;
            ds.Color = dataseriesBrush;
            ds.LineThickness = 1;

            foreach (XElement xe in dataList)
            {
                DataPoint dp = new DataPoint() { YValue = Math.Abs(int.Parse(xe.Attribute(momentColumn).Value))*0.1, XValue = double.Parse(xe.Attribute("c0").Value) };

                dp.MouseLeftButtonUp += new MouseButtonEventHandler(dp_MouseLeftButtonUp);
                dp.Tag = xe;
                
                int testIndex = int.Parse(xe.Attribute("c5").Value);
                if (testIndex % 2 == 0)
                {
                    dp.Color = evenPointBrush;
                }
                else {
                    dp.Color = oddPointBrush;
                }
                
                dp.UseLayoutRounding = true;
                
                ds.DataPoints.Add(dp);
            }
            datasElement = xdoc.Descendants("datas").ElementAt(0);
            int startIndex = int.Parse(datasElement.Attribute("startindex").Value);
            int endIndex = dataList.Count() - 1;
            if (datasElement.Attribute("endindex")!=null)
            {
                endIndex = int.Parse(datasElement.Attribute("endindex").Value);
            }
            startDataPoint = (DataPoint)ds.DataPoints[startIndex];
            endDataPoint = (DataPoint)ds.DataPoints[endIndex];
            ds.DataPoints[startIndex].Color = startEndPointBrush;

            tlStart.Value = startDataPoint.XValue;
            tlEnd.Value = endDataPoint.XValue;

            chart.Series.Clear();
            chart.Series.Add(ds);

            chart.TrendLines.Clear();
            chart.TrendLines.Add(tlStart);
            chart.TrendLines.Add(tlEnd);

            tempDataPoint = checkStartDataPoint = checkEndDataPoint = null;
            prePressedPointBrush = null;

            btnSetEnd.IsEnabled= btnSetStart.IsEnabled = btnSetSmoothStart.IsEnabled = btnSetSmoothEnd.IsEnabled = btnSetPreEnd.IsEnabled = btnSetNextStart.IsEnabled = false;
        }
        //单击数据点时
        void dp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DataPoint dp = (DataPoint)sender;
            tlSelectedPoint.Value = dp.XValue;
            if (!chart.TrendLines.Contains(tlSelectedPoint)) {
                chart.TrendLines.Add(tlSelectedPoint);
            }
            //定位到右边的数据列表
            lv.SelectedIndex = dataList.IndexOf(dp.Tag as XElement);
            lv.ScrollIntoView(lv.SelectedItem);

            if (prePressedPointBrush != null) {//检测上次是否点击了某个点
                if (checkStartDataPoint == null && checkEndDataPoint == null) {//检测是否设置了平滑处理的起点和终点，都没有设置时把上次点击的点的颜色还原
                    tempDataPoint.Color = prePressedPointBrush;
                }
            }
            tempDataPoint = dp;
            prePressedPointBrush = (SolidColorBrush)tempDataPoint.Color;
            tempDataPoint.Color = blackBrush;
            XElement xe = (XElement)tempDataPoint.Tag;
            int checkedTestIndex = int.Parse(xe.Attribute("c5").Value);
            if (checkedTestIndex > 1)
            {
                btnSetStart.IsEnabled = miSetStart.IsEnabled = false;

            }
            else
            {
                btnSetStart.IsEnabled = miSetStart.IsEnabled = true;
            }

            int startIndex = chart.Series[0].DataPoints.IndexOf(startDataPoint);
            int tempIndex = chart.Series[0].DataPoints.IndexOf(tempDataPoint);
            if (startIndex > tempIndex)
            { //如果单击的数据点在起点的前边，禁用平滑处理和单次起终点校准
                miNextStart.IsEnabled = false;
                miPreEnd.IsEnabled = false;
                miCheckStart.IsEnabled = false;
                miCheckEnd.IsEnabled = false;

                btnSetNextStart.IsEnabled = false;
                btnSetPreEnd.IsEnabled = false;
                btnSetSmoothStart.IsEnabled = false;
                btnSetSmoothEnd.IsEnabled = false;
            }
            else
            {
                miCheckEnd.IsEnabled = true;
                miCheckStart.IsEnabled = true;

                btnSetSmoothEnd.IsEnabled = btnSetSmoothStart.IsEnabled = true;

                if (checkedTestIndex == 1)
                {//点击的是第一次的测试，设置上次终点禁用
                    miPreEnd.IsEnabled = false;

                    btnSetPreEnd.IsEnabled = false;
                }
                else
                {
                    miPreEnd.IsEnabled = true;

                    btnSetPreEnd.IsEnabled = true;
                }
                if (checkedTestIndex == testCount)
                {//点击的是最后一次测试的点，设置下次起点禁用
                    miNextStart.IsEnabled = false;

                    btnSetNextStart.IsEnabled = false;

                    miSetEnd.IsEnabled= btnSetEnd.IsEnabled = true;

                }
                else
                {
                    miNextStart.IsEnabled = true;

                    btnSetNextStart.IsEnabled = true;

                    miSetEnd.IsEnabled = btnSetEnd.IsEnabled = false ;
                }
            }
        }
        //右键数据点------------取消
        void dp_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (tempDataPoint==null)
            {
                return;
            }
            //目标  
            this.dpMenu.PlacementTarget = chart;
            //显示菜单  
            this.dpMenu.IsOpen = true;  
        }
        #region 右键菜单功能
        //平滑处理终点
        void checkEnd_Click(object sender, RoutedEventArgs e)
        {
            SetPointAsCheckEnd();
        }
        //平滑处理起点
        void checkStart_Click(object sender, RoutedEventArgs e)
        {
            SetPointAsCheckStart();
        }

        //设置起点
        void miSetStart_Click(object sender, RoutedEventArgs e)
        {
            ResetPointAsStartPoint();
        }
        //设置下一次起点
        void miNextStart_Click(object sender, RoutedEventArgs e)
        {
            SetPointAsNextTestIndexStart();
        }
        //设置上一次终点
        void miPreEnd_Click(object sender, RoutedEventArgs e)
        {
            SetPointAsPreTestIndexEnd();
        }
        #endregion

        private void btnSetPreEnd_Click(object sender, RoutedEventArgs e)
        {
            SetPointAsPreTestIndexEnd();
        }

        private void btnSetNextStart_Click(object sender, RoutedEventArgs e)
        {
            SetPointAsNextTestIndexStart();
        }

        private void btnSetSmoothEnd_Click(object sender, RoutedEventArgs e)
        {
            SetPointAsCheckEnd();
        }

        private void btnSetSmoothStart_Click(object sender, RoutedEventArgs e)
        {
            SetPointAsCheckStart();
        }

        private void btnSetStart_Click(object sender, RoutedEventArgs e)
        {
            ResetPointAsStartPoint();
        }

        private void btnSetEnd_Click(object sender, RoutedEventArgs e)
        {
            endDataPoint = tempDataPoint;
            //tempDataPoint.Color = new SolidColorBrush(Color.FromRgb(175, 220, 35));

            int endIndex = chart.Series[0].DataPoints.IndexOf(endDataPoint);
            if (datasElement.Attribute("endindex")==null)
            {
             
                datasElement.SetAttributeValue("endindex", endIndex);
            }
            else
            {
                datasElement.Attribute("endindex").Value = endIndex.ToString();
            }
            xdoc.Save(tempFileName);

            RefrenshChart();
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

        //保存关闭
        private void btnSaveClose_Click(object sender, RoutedEventArgs e)
        {
            File.Copy(tempFileName, fileName, true);
            DeleteTempFile();
            this.DialogResult = true;
            this.Close();
        }

        //取消关闭
        private void btnCancleClose_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定要取消对数据的编辑吗？\r\n点击“确定”本次对数据的编辑将不会被应用。\r\n如果要应用本次编辑，请返回并点击“保存并关闭”按钮", "系统信息", MessageBoxButton.YesNo)==MessageBoxResult.Yes) {
                DeleteTempFile();
                this.DialogResult = false;
                this.Close();
            }
        }

        //删除数据文件的拷贝
        private void DeleteTempFile() {
            File.Delete(tempFileName);
        }

        /// <summary>
        /// 重设为本次测试的起点
        /// </summary>
        private void ResetPointAsStartPoint() {
            startDataPoint = tempDataPoint;
            //tempDataPoint.Color = new SolidColorBrush(Color.FromRgb(175, 220, 35));

            int startIndex = chart.Series[0].DataPoints.IndexOf(startDataPoint);
            datasElement.Attribute("startindex").Value = startIndex.ToString();

            xdoc.Save(tempFileName);

            RefrenshChart();
        }

        /// <summary>
        /// 设置为数据平滑处理的起点
        /// </summary>
        private void SetPointAsCheckStart() {
            checkStartDataPoint = tempDataPoint;
            tempDataPoint.Color = blackBrush;
            Smooth();
        }

        /// <summary>
        /// 设置为数据平滑处理的终点
        /// </summary>
        private void SetPointAsCheckEnd() {
            checkEndDataPoint = tempDataPoint;
            tempDataPoint.Color = blackBrush;
            Smooth();
        }

        //平滑处理
        private void Smooth()
        {
            if (checkStartDataPoint != null && checkEndDataPoint != null)
            {
                XElement startEle, endEle;
                double startValue, endValue = 0;

                int startTime = int.Parse(checkStartDataPoint.XValue.ToString());
                int endTime = int.Parse(checkEndDataPoint.XValue.ToString());

                int startEleIndex, endEleIndex = chart.Series[0].DataPoints.IndexOf(checkStartDataPoint);
                if (startTime > endTime)
                {
                    startEleIndex = chart.Series[0].DataPoints.IndexOf(checkEndDataPoint);
                    endEleIndex = chart.Series[0].DataPoints.IndexOf(checkStartDataPoint);

                    startValue = checkEndDataPoint.YValue;
                    endValue = checkStartDataPoint.YValue;
                }
                else
                {
                    startEleIndex = chart.Series[0].DataPoints.IndexOf(checkStartDataPoint);
                    endEleIndex = chart.Series[0].DataPoints.IndexOf(checkEndDataPoint);

                    startValue = checkStartDataPoint.YValue;
                    endValue = checkEndDataPoint.YValue;
                }

                startEle = dataList[startEleIndex];
                endEle = dataList[endEleIndex];
                if (startEle.Attribute("c5").Value != endEle.Attribute("c5").Value)
                {
                    MessageBox.Show("选择的起点和终点不在一个测试内，请重新选择！", "系统信息");
                    return;
                }


                int count = (endTime - startTime) / 5;
                double allowance = ComputeAllowance(startValue, endValue, count);//公差

                double currentValue = double.Parse(startEle.Attribute(momentColumn).Value);
                int allSmoothCount = startEleIndex + count - 1;
                for (int i = startEleIndex; i < allSmoothCount; i++)
                {
                    XElement nextEle = dataList[i + 1];

                    if (currentValue < 0)
                    {
                        currentValue = currentValue - allowance;
                    }
                    else
                    {
                        currentValue = currentValue + allowance;
                    }

                    nextEle.Attribute(momentColumn).Value = (currentValue).ToString();
                }
                xdoc.Save(tempFileName);
                RefrenshChart();

            }
        }

        //计算公差
        private double ComputeAllowance(double startValue, double endValue, int count)
        {
            double value = 0;
            value = Math.Abs((Math.Abs(startValue) - Math.Abs(endValue)) / count);
            value = Math.Round(value, 0);
            return value;
        }

        /// <summary>
        /// 设置为下次测试的起点
        /// </summary>
        private void SetPointAsNextTestIndexStart() {
            int testIndex = int.Parse(((XElement)tempDataPoint.Tag).Attribute("c5").Value);
            int tempIndex = chart.Series[0].DataPoints.IndexOf(tempDataPoint);
            for (int i = tempIndex; i < dataList.Count; i++)
            {
                XElement xe = dataList[i];
                if (xe.Attribute("c5").Value == (testIndex + 1).ToString())//如果到下一次则跳出
                {
                    break;
                }
                else
                {
                    xe.Attribute("c5").Value = (testIndex + 1).ToString();
                }
            }
            xdoc.Save(tempFileName);
            RefrenshChart();
        }

        /// <summary>
        /// 设置为上次测试的终点
        /// </summary>
        private void SetPointAsPreTestIndexEnd() {
            int testIndex = int.Parse(((XElement)tempDataPoint.Tag).Attribute("c5").Value);
            int tempIndex = chart.Series[0].DataPoints.IndexOf(tempDataPoint);
            for (int i = tempIndex; i >= 0; i--)
            {
                XElement xe = dataList[i];
                if (xe.Attribute("c5").Value == (testIndex - 1).ToString())//如果到下一次则跳出
                {
                    break;
                }
                else
                {
                    xe.Attribute("c5").Value = (testIndex - 1).ToString();
                }
            }
            xdoc.Save(tempFileName);
            RefrenshChart();
        }

      

    }
}
