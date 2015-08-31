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
using DSJL.Modules.Athletes;
using DSJL.Compoments;
using System.IO;
using DSJL.Tools;

namespace DSJL.Modules
{
    /// <summary>
    /// PageAthleteManager.xaml 的交互逻辑
    /// </summary>
    public partial class PageAthleteManager : Page
    {
        List<Model.TB_AthleteInfo> athleteList=new List<Model.TB_AthleteInfo>();
        BLL.TB_AthleteInfo athleteBLL;
        BLL.TB_TestInfo testInfoBLL;

        ContextMenu menu;//列表右键菜单
        MenuItem miEdit = new MenuItem();
        MenuItem miShowTestInfo = new MenuItem();

        public PageAthleteManager()
        {
            InitializeComponent();
            athleteBLL = new BLL.TB_AthleteInfo();
            testInfoBLL = new BLL.TB_TestInfo();

            menu = new ContextMenu();

            miEdit.Header = "编辑信息";
            miEdit.Click += new RoutedEventHandler(miEdit_Click);
            menu.Items.Add(miEdit);


            miShowTestInfo.Header = "查看测试信息";
            miShowTestInfo.Click += new RoutedEventHandler(miShowTestInfo_Click);
            menu.Items.Add(miShowTestInfo);

            Separator sp = new Separator();
            menu.Items.Add(sp);

            MenuItem miHidden = new MenuItem();
            miHidden.Header = "设为隐藏";
            menu.Items.Add(miHidden);
            miHidden.Click += new RoutedEventHandler(btnHidden_Click);

            MenuItem miDelete = new MenuItem();
            miDelete.Header = "删除";
            menu.Items.Add(miDelete);
            miDelete.Click += new RoutedEventHandler(btnDelete_Click);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //RefrenshAthleteList();
        }

        #region 右键菜单功能
        //显示测试信息
        void miShowTestInfo_Click(object sender, RoutedEventArgs e)
        {
            Model.TB_AthleteInfo selectedAth = dgAthlete.SelectedValue as Model.TB_AthleteInfo;
            DSJL.Modules.Test.ShowTestInfoWindow stiWindow = new Test.ShowTestInfoWindow();
            stiWindow.Owner = Application.Current.MainWindow;
            stiWindow.AthleteInfo = selectedAth;
            stiWindow.TestManager = testManager.SelectedItem;
            stiWindow.ShowDialog();
        }
      
        //单条记录设置隐藏
        void miHidden_Click(object sender, RoutedEventArgs e)
        {
            Model.TB_AthleteInfo selectedAth = dgAthlete.SelectedValue as Model.TB_AthleteInfo;
            try
            {
                if (BLL.TB_Setting.OldPwd == "")
                {
                    AddHiddenPwdWindow pwdWindow = new AddHiddenPwdWindow();
                    pwdWindow.Owner = Application.Current.MainWindow;
                    if (pwdWindow.ShowDialog() == true)
                    {
                        athleteBLL.HiddenData(selectedAth.ID.ToString(), true);
                        RefrenshAthleteList();
                    }
                }
                else
                {
                    athleteBLL.HiddenData(selectedAth.ID.ToString(), true);
                    RefrenshAthleteList();
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("隐藏出错！\r\n" + ee.Message, "系统错误");
            }
        }

        //单条记录删除
        void miDelete_Click(object sender, RoutedEventArgs e)
        {
            Model.TB_AthleteInfo selectedAth = dgAthlete.SelectedValue as Model.TB_AthleteInfo;
            if (MessageBox.Show("删除信息将会删除该运动员的测试数据，改变应用该受试者测试数据建立的测试参考值。\r\n删除的信息将不能恢复，请确认要删除选择的信息吗？", "删除受试者信息确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                List<Model.TB_TestInfo> testInfoList = testInfoBLL.GetModelList("ath_id=" + selectedAth.ID);
                DeleteDataFile(testInfoList);
                if (athleteBLL.Delete(selectedAth.ID))
                {
                    RefrenshAthleteList();
                }
                else {
                    MessageBox.Show("删除出错！", "系统错误");
                }
            }
        }

        //单条记录编辑
        void miEdit_Click(object sender, RoutedEventArgs e)
        {
            Model.TB_AthleteInfo selectedAth = dgAthlete.SelectedValue as Model.TB_AthleteInfo;
            if (selectedAth.Hidden != "0")
            {
                MessageBox.Show("该人员信息已被设置为隐藏，请在设置中选择“显示所有数据”后进行编辑", "系统信息");
                return;
            }
            AddOrEditAthlete window = new AddOrEditAthlete();
            window.Athlete = selectedAth;
            window.Owner = Application.Current.MainWindow;
            if (window.ShowDialog() == true)
            {
                RefrenshAthleteList();
            }
        }
        #endregion

        private void RefrenshAthleteList() {
            if (testManager.SelectedItem != null)
            {
                athleteList = athleteBLL.GetModelList("ath_testid=" + testManager.SelectedItem.ID);
                dgAthlete.SetBinding(DataGrid.ItemsSourceProperty, new Binding() { Source = athleteList });
            }
            else {
                athleteList = new List<Model.TB_AthleteInfo>();
                dgAthlete.SetBinding(DataGrid.ItemsSourceProperty, new Binding() { Source = athleteList });
            }
        }

        //添加人员信息
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddOrEditAthlete window = new AddOrEditAthlete();
            window.TestItem = testManager.SelectedItem;
            window.Owner = Application.Current.MainWindow;
            if (window.ShowDialog() == true)
            {
                RefrenshAthleteList();
            }
        }

        //批量导入人员信息
        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            ImportAthlete window = new ImportAthlete();
            window.TestItem = testManager.SelectedItem;
            window.Owner = Application.Current.MainWindow;
            if (window.ShowDialog() == true)
            {
                testManager.RefrenshList();
                //RefrenshAthleteList();
            }
        }

        //导出人员信息
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            List<Model.TB_AthleteInfo> selectedAthleteList = athleteList.FindAll(model => model.IsChecked == true);
            if (selectedAthleteList.Count == 0 )
            {
                MessageBox.Show("请选择要导出的受试者信息！");
                return;
            }
            string fileName;
            string excelName = testManager.SelectedItem.TestName + "的受试者信息";
            if (ShowFileDialog.ShowSaveFileDialog(out fileName,ShowFileDialog.excelFilter,ShowFileDialog.excelExt,excelName)) {
                string tempFileName = AppDomain.CurrentDomain.BaseDirectory + "\\AppTemplate\\listofnames.xls";
                string[,] contents=new string[selectedAthleteList.Count,15];
                for (int i = 0; i < selectedAthleteList.Count; i++)
                {
                    Model.TB_AthleteInfo item=selectedAthleteList[i];
                    contents[i,0] = item.Ath_TestDate.ToString("yyyy年MM月dd日");
                    contents[i,1] = item.Ath_Name;
                    contents[i,2] = item.Ath_Sex;
                    contents[i,3] = ((DateTime)item.Ath_Birthday).ToString("yyyy年MM月dd日");
                    contents[i,4] = item.Ath_Height;
                    contents[i,5] = item.Ath_Weight;
                    contents[i,6] = item.Ath_Project;
                    contents[i,7] = item.Ath_MainProject;
                    contents[i,8] = item.Ath_TrainYears;
                    contents[i,9] = item.Ath_Level;
                    contents[i,10] = item.Ath_Team;
                    contents[i,11] = item.Ath_TestAddress;
                    contents[i,12] = item.Ath_TestMachine;
                    contents[i,13] = item.Ath_TestState;
                    contents[i,14] = item.Ath_Remark;
                }
                try
                {
                    ExcelDao.ExcelUtil.SaveExcelFile(fileName, tempFileName, contents, 1, 3);
                    MessageBox.Show("导出成功！", "系统信息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ee)
                {
                    MessageBox.Show("导出受试者信息出错！\r\n" + ee.Message, "系统错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
              
            }
        }

        //编辑
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            List<Model.TB_AthleteInfo> selectedAthleteList = athleteList.FindAll(model => model.IsChecked == true);
            if (selectedAthleteList.Count == 0 || selectedAthleteList.Count > 1)
            {
                MessageBox.Show("请选择一个测试者信息编辑！");
                return;
            }
            Model.TB_AthleteInfo selectedAth = selectedAthleteList[0];
            if (selectedAth.Hidden != "0") {
                MessageBox.Show("该人员信息已被设置为隐藏，请在设置中选择“显示所有数据”后进行编辑","系统信息");
                return;
            }
            AddOrEditAthlete window = new AddOrEditAthlete();
            window.Athlete = selectedAth;
            window.Owner = Application.Current.MainWindow;
            if (window.ShowDialog() == true)
            {
                RefrenshAthleteList();
            }
        }

        //测试事项选择改变事件
        private void testManager_ItemSelectionChangedEvent(Model.TB_TestManager selectedItem)
        {
            RefrenshAthleteList();
        }

        //全选
        private void checkAll_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.IsChecked == true)
            {
                foreach (Model.TB_AthleteInfo bi in athleteList)
                {
                    bi.IsChecked = true;
                }
                dgAthlete.SelectAll();
            }
            else
            {
                foreach (Model.TB_AthleteInfo bi in athleteList)
                {
                    bi.IsChecked = false;
                }
                dgAthlete.UnselectAll();
            }
        }

        //列表单击
        private void dgAthlete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgAthlete.SelectedIndex >= 0) {
                foreach (Model.TB_AthleteInfo bi in athleteList)
                {
                    bi.IsChecked = false;
                }
                if (dgAthlete.SelectedItems.Count > 1)
                {
                    foreach (var selectedItem in dgAthlete.SelectedItems)
                    {
                        Model.TB_AthleteInfo selectedAth = selectedItem as Model.TB_AthleteInfo;
                        selectedAth.IsChecked = true;
                    }
                }
                else {
                    Model.TB_AthleteInfo selectedAth = dgAthlete.SelectedItem as Model.TB_AthleteInfo;
                    selectedAth.IsChecked = true;
                }
            }
        }

        //搜索
        private void txtKeyWord_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtKeyWord.Text.Trim() != "")
            {
                athleteList = athleteBLL.GetModelList("ath_name like '%" + txtKeyWord.Text.Trim() + "%'");
                dgAthlete.ItemsSource = athleteList;
            }
            else
            {
                RefrenshAthleteList();
            }
        }

        //删除
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            List<Model.TB_AthleteInfo> selectedAthList = athleteList.FindAll(x => x.IsChecked == true);
            if (selectedAthList.Count == 0)
            {
                MessageBox.Show("请至少选择一条信息删除！", "系统信息");
                return;
            }
            if (MessageBox.Show("删除信息将会删除该运动员的测试数据，改变应用该受试者测试数据建立的测试参考值。\r\n删除的信息将不能恢复，请确认要删除选择的信息吗？", "删除受试者信息确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string ids = "";
                foreach (Model.TB_AthleteInfo ath in selectedAthList)
                {
                    ids += ath.ID.ToString();
                    if (ath != selectedAthList.Last())
                    {
                        ids += ",";
                    }
                }
                List<Model.TB_TestInfo> testInfoList = testInfoBLL.GetModelList("ath_id in (" + ids + ")");
                DeleteDataFile(testInfoList);
                if (athleteBLL.DeleteList(ids))
                {
                    RefrenshAthleteList();
                }
                else
                {
                    MessageBox.Show("删除出错！", "系统错误");
                }
            }
        }

        //隐藏数据
        private void btnHidden_Click(object sender, RoutedEventArgs e)
        {
            string selectedIDs = GetSelectedID();
            if (selectedIDs == "") {
                MessageBox.Show("请至少选择一条信息！", "系统信息");
                return;
            }
            try
            {
                if (BLL.TB_Setting.OldPwd == "")
                {
                    AddHiddenPwdWindow pwdWindow = new AddHiddenPwdWindow();
                    pwdWindow.Owner = Application.Current.MainWindow;
                    if (pwdWindow.ShowDialog() == true)
                    {
                        athleteBLL.HiddenData(selectedIDs, true);
                        RefrenshAthleteList();
                    }
                }
                else
                {
                    athleteBLL.HiddenData(selectedIDs, true);
                    RefrenshAthleteList();
                }
            }
            catch (Exception ee) {
                MessageBox.Show("隐藏出错！\r\n" + ee.Message, "系统错误");
            }
        }

        //获取选择的项
        private string GetSelectedID() {
            string ids = "";
            List<Model.TB_AthleteInfo> selectedAthList = athleteList.FindAll(x => x.IsChecked == true);
            if (selectedAthList.Count != 0) {
                foreach (Model.TB_AthleteInfo ath in selectedAthList)
                {
                    ids += ath.ID.ToString();
                    if (ath != selectedAthList.Last())
                    {
                        ids += ",";
                    }
                }
            }
            return ids;
        }

        //隐藏管理
        private void btnHiddenManager_Click(object sender, RoutedEventArgs e)
        {
            CheckHiddenPwdWindow checkWindow = new CheckHiddenPwdWindow();
            checkWindow.Owner = Application.Current.MainWindow;
            if (checkWindow.ShowDialog() == true) {
                HiddenManagerWindow managerWindow = new HiddenManagerWindow();
                managerWindow.Owner = Application.Current.MainWindow;
                if (managerWindow.ShowDialog() == true) {
                    RefrenshAthleteList();
                }
            }
        }

        //添加右键菜单
        private void dgAthlete_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //e.Row.ToolTip = "右键编辑";
            //e.Row.MouseRightButtonDown += (s, a) =>
            //{
            //    a.Handled = true;
            //    DataGridRow dgr = s as DataGridRow;
            //    (sender as DataGrid).SelectedIndex = dgr.GetIndex();
            //    dgr.Focus();
            //    menu.PlacementTarget = dgr;
            //    menu.IsOpen = true;
            //};
        }

        //删除信息时同时删除数据文件
        private void DeleteDataFile(List<Model.TB_TestInfo> testInfoList) {
            foreach (Model.TB_TestInfo testInfo in testInfoList) {
                string dataFileName = Model.AppPath.XmlDataDirPath + testInfo.DataFileName;
                if (File.Exists(dataFileName)) {
                    File.Delete(dataFileName);
                }
            }
        }

        private void dgAthlete_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            List<Model.TB_AthleteInfo> checkedAthList = athleteList.FindAll(x => x.IsChecked == true);
            if (checkedAthList.Count!=0)
            {
                if (checkedAthList.Count>1)
                {
                    miEdit.IsEnabled = false;
                    miShowTestInfo.IsEnabled = false;
                }
                else
                {
                    miEdit.IsEnabled = true;
                    miShowTestInfo.IsEnabled = true;
                }
                menu.IsOpen = true;
            }
         
        }

        
    }
}
