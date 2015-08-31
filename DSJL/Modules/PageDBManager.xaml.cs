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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using ICSharpCode.SharpZipLib.Zip;
using DSJL.Modules.DB;
using System.Threading;

namespace DSJL.Modules
{
    /// <summary>
    /// PageDBManager.xaml 的交互逻辑
    /// </summary>
    public partial class PageDBManager : Page
    {
        BLL.TB_BackupInfo backInfoBLL;

        ObservableCollection<Model.TB_BackupInfo> backupInfoCollection;

        public PageDBManager()
        {
            InitializeComponent();
            backInfoBLL = new BLL.TB_BackupInfo();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ReloadData();
        }
        //备份
        private void btnBackup_Click(object sender, RoutedEventArgs e)
        {
            if (txtBackupName.Text.Trim() == "")
            {
                MessageBox.Show("备份名称不能为空，请重新输入！");
                return;
            }
            string fileName = txtBackupName.Text.Trim() + "-" + DateTime.Now.ToString("yyyyMMdd");

            SaveFileDialog ofd = new SaveFileDialog();
            ofd.Title = "请选择保存文件的路径";
            ofd.DefaultExt = "zip";
            ofd.FileName = fileName;
            ofd.OverwritePrompt = true;
            ofd.AddExtension = true;
            ofd.Filter = "压缩文件(*.zip)|*.zip";
            if (ofd.ShowDialog() == true)
            {
                string path = ofd.FileName;
                try
                {
                    CompressProgress progressWindow = new CompressProgress();
                    progressWindow.OutputPath = path;
                    progressWindow.Owner = Application.Current.MainWindow;
                    progressWindow.ShowDialog();

                    Model.TB_BackupInfo backInfo = new Model.TB_BackupInfo();
                    backInfo.BackupDate = DateTime.Now.ToString("yyyy-MM-dd");
                    backInfo.BackupName = txtBackupName.Text.Trim();
                    backInfo.BackupPath = path;
                    backInfoBLL.Add(backInfo);
                    ReloadData();
                    txtBackupName.Text = "";
                    MessageBox.Show("备份成功！");

                }
                catch (Exception ee)
                {
                    MessageBox.Show("备份出错，请稍候重试！\r\n" + ee.Message);
                }
            }
        }

        private void ReloadData()
        {
            backupInfoCollection = new ObservableCollection<Model.TB_BackupInfo>(backInfoBLL.GetModelList(""));
            dgBackupInfo.SetBinding(DataGrid.ItemsSourceProperty, new Binding() { Source = backupInfoCollection });
        }

        private void checkAll_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.IsChecked == true)
            {
                foreach (Model.TB_BackupInfo bi in backupInfoCollection)
                {
                    bi.IsChecked = true;
                }
                dgBackupInfo.SelectAll();
            }
            else
            {
                foreach (Model.TB_BackupInfo bi in backupInfoCollection)
                {
                    bi.IsChecked = false;
                }
                dgBackupInfo.UnselectAll();
            }
        }

        private void dgBackupInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgBackupInfo.SelectedIndex >= 0)
            {
                foreach (var model in backupInfoCollection)
                {
                    model.IsChecked = false;
                }
                if (dgBackupInfo.SelectedItems.Count > 1)
                {
                    foreach (var item in dgBackupInfo.SelectedItems)
                    {
                        Model.TB_BackupInfo model = item as Model.TB_BackupInfo;
                        model.IsChecked = true;
                    }
                }
                else
                {
                    Model.TB_BackupInfo model = dgBackupInfo.SelectedItem as Model.TB_BackupInfo;
                    model.IsChecked = true;
                }
            }
        }
        //还原
        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {

            if (MessageBox.Show("还原数据库会把现有的数据库删除，且不可恢复，确定要还原吗？", "系统信息", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "还原数据库";
                ofd.Filter = "压缩文件(*.zip)|*.zip";
                if (ofd.ShowDialog() == true)
                {
                    ExtractProgressWindow extractWindow = new ExtractProgressWindow();
                    extractWindow.BackupFile = ofd.FileName;
                    extractWindow.Owner = Application.Current.MainWindow;
                    extractWindow.ShowDialog();
                   
                    ReloadData();
                    DSJL.Tools.DBUpgrade.Upgrade();
                    MessageBox.Show("还原成功！");
                }
            }
        }
        //删除记录
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var checkedItems = backupInfoCollection.Where(item => item.IsChecked == true);
            if (checkedItems.Count() > 0)
            {
                if (MessageBox.Show("确定删除该备份记录吗？", "系统信息", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
                string ids = "";
                foreach (Model.TB_BackupInfo info in checkedItems)
                {
                    ids += info.ID;
                    if (info != checkedItems.Last())
                    {
                        ids += ",";
                    }
                }
                if (backInfoBLL.DeleteList(ids))
                {
                    ReloadData();
                }
                else
                {
                    MessageBox.Show("删除出错！");
                }
            }
            else
            {
                MessageBox.Show("请至少选择一条记录删除！");
            }
        }

        private void btnMerge_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "选择数据库备份文件";
            ofd.Filter = "压缩文件(*.zip)|*.zip";
            if (ofd.ShowDialog() == true)
            {
                string archiveFileName = ofd.FileName;
                MergeDB merge = new MergeDB();
                merge.ArchiveFile = archiveFileName;
                merge.Show();
            }
        }
    }
}
