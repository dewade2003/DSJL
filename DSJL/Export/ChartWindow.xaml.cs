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
using System.Windows.Threading;

namespace DSJL.Export
{
    /// <summary>
    /// 没用
    /// </summary>
    public partial class ChartWindow : Window
    {
        private int imgWidth = 3000;
        private int imgHeight = 1040;
        private int renderCount = 0;

        private Chart progressChart;
        private Chart maxColumnChart;
        private Chart maxLineChart;
        private Chart oddAvgChart;
        private Chart evenAvgChart;

        DispatcherTimer timer;

        public ChartWindow()
        {
            InitializeComponent();
            progressChart = new Chart();
            maxColumnChart = new Chart();
            maxLineChart = new Chart();
            oddAvgChart = new Chart();
            evenAvgChart = new Chart();
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            this.Left = screenWidth + 10;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(1);
            timer.Tick += new EventHandler(timer_Tick);
        }

        public DataSeriesCollection ProgressDS
        {
            get;
            set;
        }

        public DataSeriesCollection MaxColumnDS
        {
            get;
            set;

        }
        public DataSeriesCollection MaxLineDS
        {
            get;
            set;
        }

        public DataSeriesCollection OddAvgDS
        {
            get;
            set;
        }

        public DataSeriesCollection EvenAvgDS
        {
            get;
            set;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            SaveToImage(progressGrid, AppDomain.CurrentDomain.BaseDirectory + "\\progress.jpg");
            SaveToImage(maxColumnGrid, AppDomain.CurrentDomain.BaseDirectory + "\\maxcolumn.jpg");
            SaveToImage(maxLineGrid, AppDomain.CurrentDomain.BaseDirectory + "\\maxline.jpg");
            SaveToImage(oddAvgGrid, AppDomain.CurrentDomain.BaseDirectory + "\\oddavg.jpg");
            SaveToImage(evenAvgGrid, AppDomain.CurrentDomain.BaseDirectory + "\\evenavg.jpg");
            this.DialogResult = true;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            maxColumnChart.ScrollingEnabled = maxLineChart.ScrollingEnabled = oddAvgChart.ScrollingEnabled = evenAvgChart.ScrollingEnabled = progressChart.ScrollingEnabled = false;
            maxColumnChart.BorderThickness = maxLineChart.BorderThickness = oddAvgChart.BorderThickness = evenAvgChart.BorderThickness = progressChart.BorderThickness = new Thickness(0, 0, 0, 0);
            maxLineChart.AnimationEnabled = maxColumnChart.AnimationEnabled = oddAvgChart.AnimationEnabled = evenAvgChart.AnimationEnabled = progressChart.AnimationEnabled = false;

            progressChart.Loaded += new RoutedEventHandler(Chart_Rendered);
            maxColumnChart.Loaded += new RoutedEventHandler(Chart_Rendered);
            maxLineChart.Loaded += new RoutedEventHandler(Chart_Rendered);
            oddAvgChart.Loaded += new RoutedEventHandler(Chart_Rendered);
            evenAvgChart.Loaded += new RoutedEventHandler(Chart_Rendered);

            progressChart.Series = ProgressDS;
            maxColumnChart.Series = MaxColumnDS;
            maxLineChart.Series = MaxLineDS;
            oddAvgChart.Series = OddAvgDS;
            evenAvgChart.Series = EvenAvgDS;

            progressGrid.Children.Add(progressChart);
            maxColumnGrid.Children.Add(maxColumnChart);
            maxLineGrid.Children.Add(maxLineChart);
            oddAvgGrid.Children.Add(oddAvgChart);
            evenAvgGrid.Children.Add(evenAvgChart);
           
        }

        private void Chart_Rendered(object sender, EventArgs e)
        {
            Chart chart = sender as Chart;
            chart.PlotArea.ShadowEnabled = false;
            if (chart.Legends.Count > 0) {
                chart.Legends[0].ShadowEnabled = false;
                chart.Legends[0].BorderThickness = new Thickness(0, 0, 0, 0);
            }
            renderCount++;
            if (renderCount == 5) { //当5个图表控件全部加载完成之后开始执行导出图片
                timer.Start();
            }
        }

        private void SaveToImage(FrameworkElement ui, string fileName)
        {
            System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create);
            imgWidth = int.Parse(ui.Width.ToString()) * 4;
            imgHeight = int.Parse(ui.Height.ToString()) * 4;
            RenderTargetBitmap bmp = new RenderTargetBitmap(imgWidth, imgHeight, 384, 384, PixelFormats.Pbgra32);
            bmp.Render(ui);
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            encoder.Save(fs);
            fs.Close();
        }
    }
}
