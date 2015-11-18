using DSJL.Model;
using DSJL.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace DSJL.Modules.DB
{
    /// <summary>
    /// BackupProjectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BackupProjectWindow : Window
    {
        string backFileName = "";
        bool cancle = false;
        Task backupTask;

        BLL.TB_TestManager testManagerBLL;
        List<TB_TestManager> testManagerList;
        public BackupProjectWindow()
        {
            InitializeComponent();
            testManagerBLL = new BLL.TB_TestManager();
            testManagerList = new List<TB_TestManager>();
        }

        public string BackupName {
            get;
            set;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Point mousePoint = e.GetPosition(this);

                if (e.ButtonState == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Left && mousePoint.Y <= 30)
                {

                    this.DragMove();
                }
            }
            catch { }
        }

        private void btnChangeFolder_Click(object sender, RoutedEventArgs e)
        {
            ChooseBackupPath();
        }

        private void btnBackup_Click(object sender, RoutedEventArgs e)
        {
            backupTask = new Task(() =>
            {
                try
                {
                    //创建temp 目录
                    CreateDir(AppPath.TempDirPath);
                    //1、读取选择的项目的测试数据,包括人员信息和测试数据
                    List<TB_TestManager> checkedTestManager = testManagerList.FindAll(x => x.IsChecked == true);
                    Converter<TB_TestManager, int> testManagerIDConverter = new Converter<TB_TestManager, int>((TB_TestManager manager) =>
                    {
                        return manager.ID;
                    });
                    int[] checkedTestManagerIDs = checkedTestManager.ConvertAll<int>(testManagerIDConverter).ToArray();
                    string testManagerIDs = string.Join(",", checkedTestManagerIDs);
                    List<TB_AthleteInfo> athList = (new BLL.TB_AthleteInfo()).GetModelList(string.Format("ath_testid in ({0})", testManagerIDs));
                    Converter<TB_AthleteInfo, int> athIDConverter = new Converter<TB_AthleteInfo, int>((TB_AthleteInfo ath) =>
                    {
                        return ath.ID;
                    });
                    string athIDs = string.Join(",", athList.ConvertAll<int>(athIDConverter).ToArray());
                    List<TB_TestInfo> testInfoList = (new BLL.TB_TestInfo()).GetModelList(string.Format("ath_id in ({0})", athIDs));
                    //2、复制空数据库文件,复制数据xml文件
                    File.Copy(AppPath.DataPath + AppPath.EmptyDBName, AppPath.TempDirPath + AppPath.DBName);
                    //3、往空数据库文件中写入数据
                    DBUtility.DbHelperOleDb.SetDBPath(AppPath.TempDirPath + AppPath.DBName);//------------------

                    //4、创建压缩窗口，设置DataFilePath属性，并启动，Invoke
                    this.Dispatcher.Invoke(new Action(() => {
                        CompressProgress compress = new CompressProgress();
                        compress.DataFilePath = AppPath.TempDirPath;
                        compress.OutputPath = backFileName;
                        if (compress.ShowDialog() == true)
                        {
                            //删除temp目录
                            DeleteTempDir();
                            MessageBoxTool.ShowConfirmMsgBox("备份成功！");
                            this.Close();
                        }
                    }));
                }
                catch (Exception ee)
                {
                    MessageBoxTool.ShowConfirmMsgBox(string.Format("备份出现错误！\r\n异常类型：{0}\r\n异常信息:{1}", ee.GetType().ToString(), ee.Message));
                }
               
            });
            backupTask.Start();
            btnBackup.IsEnabled = btnChangeFolder.IsEnabled = false;
            btnCancle.IsEnabled = true;
        }

        //创建文件夹如果文件夹不存在,存在则先删除
        private void CreateDir(string dirFullName)
        {
            if (Directory.Exists(dirFullName))
            {
                Directory.Delete(dirFullName);
            }
            Directory.CreateDirectory(dirFullName);
        }

        //删除临时文件夹
        private void DeleteTempDir()
        {
            if (Directory.Exists(AppPath.TempDirPath))
            {
                Directory.Delete(AppPath.TempDirPath);
            }
        }

        private void Window_ContentRendered(object sender, System.EventArgs e)
        {
            if (!ChooseBackupPath())
            {
                this.Close();
            }
            else
            {
                testManagerList = testManagerBLL.GetModelList("");
                testItmesListBox.SetBinding(ListBox.ItemsSourceProperty, new Binding() { Source = testManagerList });
            }
        }

        private bool ChooseBackupPath()
        {
            if (ShowFileDialogTool.ShowSaveFileDialog(out backFileName,ShowFileDialogTool.zipFilter,ShowFileDialogTool.zipExt,BackupName)&&!backFileName.Equals(""))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void btnCancle_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmCancleBackup())
            {
                cancle = true;
                btnChangeFolder.IsEnabled = btnBackup.IsEnabled = true;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (backupTask?.Status == TaskStatus.Running)
            {
                if (ConfirmCancleBackup())
                {
                    cancle = true;
                    this.Close();
                }

            }
            else
            {
                this.Close();
            }
        }

        private bool ConfirmCancleBackup()
        {
            if (MessageBoxTool.ShowAskMsgBox("正在进行备份，确定要取消吗？")== MessageBoxResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void testItmesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            foreach (var item in e.RemovedItems)
            {
                TB_TestManager testManagerItem = (TB_TestManager)item;
                testManagerItem.IsChecked = false;
            }
            
            foreach (var item in e.AddedItems)
            {
                TB_TestManager testManagerItem = (TB_TestManager)item;
                testManagerItem.IsChecked = true;
            }
        }
    }
}
