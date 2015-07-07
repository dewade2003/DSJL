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
using DSJL.MonitorSetting;
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace DSJL.Modules.Setup
{
    /// <summary>
    /// MonitorSetupPage.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorSetupPage : Page
    {
        MonitorSettingUtil monitorUtil;
        ObservableCollection<MonitorDirModel> monitorDirModelList;

        public MonitorSetupPage()
        {
            InitializeComponent();
            monitorUtil = new MonitorSettingUtil();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            cbIsMonitor.IsChecked = monitorUtil.IsOpenMonitor;
            RefrenshDataGrid();
        }

        private void RefrenshDataGrid() {
            monitorDirModelList = new ObservableCollection<MonitorDirModel>(monitorUtil.DirModelList);
            dgDir.SetBinding(System.Windows.Controls.DataGrid.ItemsSourceProperty, new System.Windows.Data.Binding() { Source = monitorDirModelList });
        }

        private void btnAddDir_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "选择受监视的文件夹";
            fbd.ShowNewFolderButton = true;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                if (monitorDirModelList.ToList().Find(x => x.DirPath == fbd.SelectedPath) == null)
                {
                    monitorUtil.AddDir(fbd.SelectedPath);
                    RefrenshDataGrid();
                }
                else
                {
                    System.Windows.MessageBox.Show("该文件夹已在监视列表内！", "添加失败");
                }
            }
        }

        private void cbIsMonitor_Checked(object sender, RoutedEventArgs e)
        {
            monitorUtil.IsOpenMonitor = cbIsMonitor.IsChecked == true ? true : false;
        }

        private void btnDeleteCheckedItems_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmDelete("确认删除选择的文件夹记录吗？")) {
                List<int> indexs = new List<int>();
                foreach (MonitorDirModel model in monitorDirModelList)
                {
                    if (model.IsChecked)
                    {
                        indexs.Add(model.Index);
                    }
                }
                monitorUtil.DeleteCheckedDir(indexs.ToArray());
                RefrenshDataGrid();
            }
        }

        private void btnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmDelete("确认删除全部的文件夹记录吗？")) {
                monitorUtil.DeleteAllDir();
                RefrenshDataGrid();
            }
        }

        private void btnOpenDir_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;
            int index = int.Parse(button.Tag.ToString());
            MonitorDirModel model = monitorDirModelList.ToList().Find(x => x.Index == index);
            string path = model.DirPath;
            System.Diagnostics.Process.Start("explorer.exe", path);
        }

        private void btnDeleteDir_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmDelete("确认删除该文件夹记录吗？")) {
                System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;
                int index = int.Parse(button.Tag.ToString());
                monitorUtil.DeleteDir(index);
                RefrenshDataGrid();
            }
        }

        private void dgDir_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgDir.SelectedIndex >= 0)
            {
                foreach (MonitorDirModel model in monitorDirModelList)
                {
                    model.IsChecked = false;
                }
                if (dgDir.SelectedItems.Count > 1)
                {
                    foreach (var item in dgDir.SelectedItems)
                    {
                        MonitorDirModel model = item as MonitorDirModel;
                        model.IsChecked = true;
                    }
                }
                else
                {
                    MonitorDirModel model = dgDir.SelectedItem as MonitorDirModel;
                    model.IsChecked = true;
                }
            }
        }

        private void checkAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (MonitorDirModel model in monitorDirModelList) {
                model.IsChecked = true;
            }
        }

        private void checkAll_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (MonitorDirModel model in monitorDirModelList)
            {
                model.IsChecked = false;
            }
        }

        private bool ConfirmDelete(string msg) {
            if (System.Windows.MessageBox.Show(msg, "系统提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                return true;
            }
            else {
                return false;
            }
        }
    }
}
