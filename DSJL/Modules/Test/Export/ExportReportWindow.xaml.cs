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
using Microsoft.Win32;
using DSJL.Modules.Test;

namespace DSJL.Modules.Test.Export
{
    /// <summary>
    /// ExportReportWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ExportReportWindow : Window
    {
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);
        private static string choosePath = "";

        public ExportReportWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            exportProgress.Minimum = 0;
            exportProgress.Maximum = TestInfoList.Count;
            exportProgress.Value = 0;
        }

        /// <summary>
        /// 测试信息list
        /// </summary>
        public List<Model.TestInfoModel> TestInfoList
        {
            get;
            set;
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //选择路径
        private void btnChoosePath_Click(object sender, RoutedEventArgs e)
        { 
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择保存文件的位置";
            dialog.ShowNewFolderButton = true;
            if (choosePath != "") {
                dialog.SelectedPath = choosePath;
            }
            System.Windows.Forms.DialogResult resultPath = dialog.ShowDialog();
            if (resultPath == System.Windows.Forms.DialogResult.OK)
            {
                choosePath = txtPath.Text = dialog.SelectedPath;
                btnExport.IsEnabled = true;
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double value = 0;
            UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(exportProgress.SetValue);
            List<ShowChartWindow> windowList = new List<ShowChartWindow>();
            for (int i = 0; i < TestInfoList.Count; i++) {
                tbCurrent.Text = "正在导出:" + TestInfoList[i].Ath_Name;

                ShowChartWindow window = new ShowChartWindow();
                window.DataModel = TestInfoList[i];
                window.Left = screenWidth;
                window.Visibility = Visibility.Hidden;
                window.IsExport = true;
                window.Show();
                window.ExportReport(choosePath);
                window.Close();
                window = null;

                tbProgress.Text = (i + 1) + "/" + TestInfoList.Count;
                value += 1;
                Dispatcher.Invoke(updatePbDelegate,
                    System.Windows.Threading.DispatcherPriority.Background,
                    new object[] { ProgressBar.ValueProperty, value });
            }
            System.GC.Collect();
            MessageBox.Show("导出完成！","系统信息");
            this.Close();
        }
       

    }
}
