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
using DSJL.Tools;
using System.Threading;
using System.Threading.Tasks;

namespace DSJL.Modules.Test.Export
{
    /// <summary>
    /// ExportReportWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ExportReportWindow : Window
    {
        //private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);
        private static string choosePath = "";

        Boolean isExportReportCancled = false;

        public ExportReportWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            exportProgress.Minimum = 0;
            exportProgress.Maximum = TestInfoList.Count;
            exportProgress.Value = 0;

            tbProgress.Text = "0/" + TestInfoList.Count.ToString();

            btnChoosePath_Click(btnChoosePath, new RoutedEventArgs());
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
            if (ShowFileDialogTool.ShowSaveFileDialog(out choosePath, ShowFileDialogTool.pdfFilter, ShowFileDialogTool.pdfExt, "测试报告"))
            {
                choosePath = choosePath.Substring(0, choosePath.LastIndexOf("\\"));
                btnExport.IsEnabled = true;
                txtPath.Text = "导出报告位置：" + choosePath;
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            btnCancle.IsEnabled = true;
            btnExport.IsEnabled = false;
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            //double value = 0;
            //UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(exportProgress.SetValue);
            //List<ShowChartWindow> windowList = new List<ShowChartWindow>();

            Task exportReportTask = new Task(() =>
            {
                for (int i = 0; i < TestInfoList.Count; i++)
                {
                    //quxiao 
                    if (isExportReportCancled)
                    {
                        isExportReportCancled = false;
                        this.Dispatcher.Invoke(
                                new Action(() =>
                                        {
                                            btnExport.IsEnabled = true;
                                            btnCancle.IsEnabled = false;
                                        } )
                          );
                        break;
                    }

                    //gengxin jindu 
                    this.Dispatcher.Invoke(
                                        new Action(() =>
                                                {
                                                    tbCurrent.Text = "正在导出:" + TestInfoList[i].Ath_Name;
                                                    tbProgress.Text = (i + 1) + "/" + TestInfoList.Count;
                                                    exportProgress.Value += 1;

                                                    //diaoyong daochu
                                                    ShowChartWindow window = new ShowChartWindow();
                                                    window.DataModel = TestInfoList[i];
                                                    window.Left = screenWidth;
                                                    window.Visibility = Visibility.Hidden;
                                                    window.IsExport = true;
                                                    window.Show();
                                                    window.ExportReport(choosePath);
                                                    window.Close();
                                                    window = null;
                                                }
                                            )
                      );
            

                }//for end
                this.Dispatcher.Invoke
                  (
                      new Action(() =>
                              {
                                  MessageBox.Show("导出完成！", "系统信息");
                                  this.Close();
                              }
                        )
                   );
            });
            exportReportTask.Start();
        }

        private void btnCancle_Click(object sender, RoutedEventArgs e)
        {
            isExportReportCancled = true;
        }


    }
}
