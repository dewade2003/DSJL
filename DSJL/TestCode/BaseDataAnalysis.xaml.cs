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
using WPFHelper.Window;
using Visifire.Charts;
using System.IO;

namespace DSJL
{
    /// <summary>
    /// BaseDataAnalysis.xaml 的交互逻辑
    /// </summary>
    public partial class BaseDataAnalysis : Window
    {
        private Chart chart = new Chart();
        private Chart chart2 = new Chart();

        public BaseDataAnalysis()
        {
            InitializeComponent();
            WindowHelper.RepairWindowBehavior(this);
            chart2.ScrollingEnabled = chart.ScrollingEnabled = false;
            chartGrid.Children.Add(chart);
            chartGrid2.Children.Add(chart2);
        }

        #region 最大化 最小化 关闭
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

        //退出
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定退出软件吗？", "系统消息", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
        #endregion
        //拖动窗口
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized)
            {
                try
                {
                    Point mousePoint = e.GetPosition(this);

                    if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed && e.ChangedButton == System.Windows.Input.MouseButton.Left && mousePoint.Y <= 100)
                    {

                        this.DragMove();
                    }
                }
                catch { }
            }
        }

        SolidColorBrush oddBrush = new SolidColorBrush(Color.FromRgb(44, 90, 222));//奇数线条颜色
        SolidColorBrush evenBrush = new SolidColorBrush(Color.FromRgb(178, 44, 222));//偶数线条颜色

        SolidColorBrush defaultBrush = new SolidColorBrush(Color.FromRgb(181, 181, 185));

        private string baFileName = "";

        private bool isShowJD = false;
        private bool isShowSD = false;

        private void btnOpenDataFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Title = "选择原始数据文件";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                baFileName = ofd.FileName;
                RefenshChart();
            }
        }

        private void RefenshChart()
        {
            if (baFileName != "")
            {
                tbFileContent.Text = "";

                string[] fileLines = File.ReadAllLines(baFileName);

                foreach (string line in fileLines)
                {
                    tbFileContent.Text += line + "\r\n";
                }

                AddSeriesToChart(0, chart2, baFileName);
                AddSeriesToChart(20, chart, baFileName);
            }
        }

        private List<string[]> Smooth(int smoothValue, string[] baseDataArray)
        {
            List<string[]> smoothedDataList = new List<string[]>();
            int startIndex = 101;
            int count = baseDataArray.Length - startIndex + 1 - smoothValue;
            for (int i = startIndex; i < count; i++)
            {
                string[] currentDataArray = baseDataArray[i].Split(' ');
                if (smoothValue > 0)
                {
                    double sum = 0;
                    for (int j = 0; j < smoothValue; j++)
                    {
                        string ljStr = baseDataArray[i + j].Split(' ')[2];
                        sum += double.Parse(ljStr);
                    }
                    string ljAvgStr = Math.Round(sum / smoothValue).ToString();
                    currentDataArray[2] = ljAvgStr;
                }
                smoothedDataList.Add(currentDataArray);
            }
            return smoothedDataList;
        }

        private void AddSeriesToChart(int cjq, Chart c, string fileName)
        {
            string[] fileLines = File.ReadAllLines(fileName);
            c.Series.Clear();

            DataSeries dsPHLJ = new DataSeries();//平滑后的力矩曲线
            dsPHLJ.LegendText = "平滑后的力矩";
            dsPHLJ.Color = oddBrush;

            dsPHLJ.RenderAs = RenderAs.QuickLine;
            c.Series.Add(dsPHLJ);

            int currentindex = 1;
            List<string[]> smoothDataList = Smooth(cjq, fileLines);

            foreach (string[] dataArray in smoothDataList) {
                int index = int.Parse(dataArray[5]);
                if (index > currentindex)
                {
                    currentindex = index;
                    dsPHLJ = new DataSeries();
                    dsPHLJ.ShowInLegend = false;
                    dsPHLJ.RenderAs = RenderAs.QuickLine;
                    if (index % 2 == 0)
                    {
                        dsPHLJ.Color = evenBrush;
                    }
                    else
                    {
                        dsPHLJ.Color = oddBrush;
                    }
                    c.Series.Add(dsPHLJ);
                }
                double time = double.Parse(dataArray[0]);
                DataPoint dpPHLJ = new DataPoint() { YValue = double.Parse(dataArray[2]), XValue = time };
                dsPHLJ.DataPoints.Add(dpPHLJ);
            }
        }

        private void cbShowJD_Checked(object sender, RoutedEventArgs e)
        {
            isShowJD = cbShowJD.IsChecked == true ? true : false;
            isShowSD = cbShowSD.IsChecked == true ? true : false;
            RefenshChart();
        }
    }
}
