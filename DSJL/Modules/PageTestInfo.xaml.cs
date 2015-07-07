using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.IO;
using DSJL.DBUtility;
using System.Data;
using DSJL.Modules.Test;
using DSJL.Modules.Standard;
using DSJL.Modules.Test.Export;

namespace DSJL.Modules
{
    /// <summary>
    /// PageTestInfo.xaml 的交互逻辑
    /// </summary>
    public partial class PageTestInfo : Page
    {
        BLL.TB_Dict dictBLL = new BLL.TB_Dict();
        List<Model.TB_Dict> jointDictList;//测试关节字典列表
        List<Model.TB_Dict> jointsideDictList;//测试侧字典列表
        List<Model.TB_Dict> testmodeDictList;//测试模式字典列表
        BLL.TB_TestInfo testInfoBLL = new BLL.TB_TestInfo();
        List<Model.TestInfoModel> testInfoModelList=new List<Model.TestInfoModel>();
        BLL.TB_AthleteInfo athleteInfoBLL = new BLL.TB_AthleteInfo();


        ContextMenu menu;//列表右键菜单

        public PageTestInfo()
        {
            InitializeComponent();
        }

        //设置了人员信息，只显示人员信息
        public Model.TB_AthleteInfo AthleteInfo
        {
            get;
            set;
        }

        //设置测试项目
        public Model.TB_TestManager TestManager
        {
            get;
            set;
        }

        //导入数据
        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            ImportTestDataWindow itdWindow = new ImportTestDataWindow();
            itdWindow.Owner = Application.Current.MainWindow;
            itdWindow.SelectedTestItem = testManager.SelectedItem;
            if (itdWindow.ShowDialog() == true) {
                testManager.RefrenshList();
                //RefrenshDataGridSource();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (TestManager != null) {
                testManager.SelectedItem = TestManager;
            }

            Model.TB_Dict allDict = new Model.TB_Dict() { 
                Dict_Key="-1",
                Dict_Value="全部"
            };
            jointDictList = dictBLL.GetModelList("dict_groupid=2");
            jointDictList.Insert(0,allDict);
            jointsideDictList = dictBLL.GetModelList("dict_groupid=3");
            jointsideDictList.Insert(0, allDict);
            testmodeDictList = dictBLL.GetModelList("dict_groupid=1");
            testmodeDictList.Insert(0, allDict);

            Binding dictBind = new Binding() { Source=jointDictList};
            cbJoint.SetBinding(ComboBox.ItemsSourceProperty, dictBind);

            cbJointSide.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = jointsideDictList });

            cbTestMode.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = testmodeDictList });

            menu = new System.Windows.Controls.ContextMenu();

            MenuItem miShowChart = new MenuItem();
            miShowChart.Header = "查看图表";
            menu.Items.Add(miShowChart);
            miShowChart.Click += new RoutedEventHandler(miShowChart_Click);

            menu.Items.Add(new Separator());

            MenuItem miAddInStandard = new MenuItem();
            miAddInStandard.Header = "加入测试参考值";
            menu.Items.Add(miAddInStandard);
            miAddInStandard.Click += new RoutedEventHandler(miAddInStandard_Click);

            MenuItem miCompare = new MenuItem();
            miCompare.Header = "平均曲线对比";
            menu.Items.Add(miCompare);
            miCompare.Click += new RoutedEventHandler(miCompare_Click);

            menu.Items.Add(new Separator());
            MenuItem miDelete = new MenuItem();
            miDelete.Header = "删除";
            menu.Items.Add(miDelete);
            miDelete.Click += new RoutedEventHandler(miDelete_Click);
        }
        
        #region 右键菜单功能

        void miCompare_Click(object sender, RoutedEventArgs e)
        {
            Model.TestInfoModel model = dgTestInfo.SelectedValue as Model.TestInfoModel;
            AvgCurveCompareWindow chartWindow = new AvgCurveCompareWindow();
            chartWindow.TestInfoModelList = new List<Model.TestInfoModel>() { model};
            chartWindow.Owner = Application.Current.MainWindow;
            chartWindow.ShowInTaskbar = false;
            chartWindow.Owner = Application.Current.MainWindow;
            chartWindow.ShowDialog();
        }

        void miAddInStandard_Click(object sender, RoutedEventArgs e)
        {
            Model.TestInfoModel model = dgTestInfo.SelectedValue as Model.TestInfoModel;
            AddToStandardWindow addWindow = new AddToStandardWindow();
            addWindow.Owner = Application.Current.MainWindow;
            addWindow.TestInfoModelList = new List<Model.TestInfoModel>() { model };
            addWindow.Owner = Application.Current.MainWindow;
            addWindow.ShowDialog();
        }

        void miDelete_Click(object sender, RoutedEventArgs e)
        {
            Model.TestInfoModel model = dgTestInfo.SelectedValue as Model.TestInfoModel;
            if (MessageBox.Show("删除这些测试数据的同时也将删除测试参考值中的这些测试数据并且不能恢复。\r\n请确认要删除选择的测试数据吗？", "删除测试数据确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (testInfoBLL.Delete(model.TestID))
                {
                    string fileName = AppDomain.CurrentDomain.BaseDirectory + "\\AppData\\XmlData\\" + model.DataFileName;
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }
                    RefrenshDataGridSource();
                }
                else {
                    MessageBox.Show("删除记录时出错！","系统错误");
                }
            }
           
        }

        void miShowChart_Click(object sender, RoutedEventArgs e)
        {
            Model.TestInfoModel model = dgTestInfo.SelectedValue as Model.TestInfoModel;
            ShowChartWindow chartWindow = new ShowChartWindow();
            chartWindow.DataModel = model;
            chartWindow.Owner = Application.Current.MainWindow;
            chartWindow.ShowInTaskbar = false;
            chartWindow.IsOne = true;
            chartWindow.ShowDialog();
        }

        #endregion

        //刷新数据表
        private void RefrenshDataGridSource() {
            if (testManager.SelectedItem != null)
            {
                testInfoModelList = new List<Model.TestInfoModel>();
                string sql = "select ath.*,test.*,"
                    + "(select dict_value from tb_dict where dict_groupid=3 and dict_key=test.joint_side) as djointside,"
                    + "(select dict_value from tb_dict where dict_groupid=1 and dict_key=test.test_mode) as dtestmode,"
                    + "(select dict_value from tb_dict where dict_groupid=2 and dict_key=test.joint) as djoint,"
                    + "(select dict_value from tb_dict where dict_groupid=(select id from tb_dict where dict_groupid=2 and dict_key=test.joint) and dict_key=test.plane and instr(dict_groupid2,test.test_mode)>0) as dplane,"
                    + "(select dict_value from tb_dict where dict_groupid=4 and dict_key=test.InsuredSide) as dInsuredSide,"
                    + "(select dict_value from tb_dict where dict_groupid=5 and dict_key=test.Gravitycomp) as dGravitycomp "
                    + "from tb_athleteinfo as ath inner join tb_testinfo as test on ath.id=test.ath_id where 0=0 ";
                if (txtKeyWord.Text.Trim() != "")
                {
                    sql += "and ath.ath_name like '%" + txtKeyWord.Text.Trim() + "%'";//姓名关键字搜索
                }
                if (testManager.SelectedItem != null)
                {
                    sql += " and ath.ath_testid=" + testManager.SelectedItem.ID;//测试项目搜索
                }
                if (cbJoint.SelectedIndex > 0)
                {
                    sql += " and test.joint='" + jointDictList[cbJoint.SelectedIndex].Dict_Key + "'";//关节搜索
                }
                if (cbTestMode.SelectedIndex > 0)
                {
                    sql += " and test.test_mode='" + testmodeDictList[cbTestMode.SelectedIndex].Dict_Key + "'";//测试模式搜索
                }
                if (cbJointSide.SelectedIndex > 0)
                {
                    sql += " and test.joint_side='" + jointsideDictList[cbJointSide.SelectedIndex].Dict_Key + "'";//测试侧搜索
                }
                if (AthleteInfo != null)
                {
                    sql += " and ath.Ath_Code='" + AthleteInfo.Ath_Code + "'";//按人员编号搜索
                }
                sql += " order by test.id asc";
                DataSet ds = DbHelperOleDb.Query(sql);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    Model.TestInfoModel testInfoModel = new Model.TestInfoModel();
                    testInfoModel.Index = i + 1;
                    testInfoModel.DGravitycomp = dr["dGravitycomp"].ToString();
                    testInfoModel.DInsuredSide = dr["dInsuredSide"].ToString();
                    testInfoModel.DJoint = dr["djoint"].ToString();
                    testInfoModel.DJointSide = dr["djointside"].ToString();
                    testInfoModel.DPlane = dr["dplane"].ToString();
                    testInfoModel.DTestMode = dr["dtestmode"].ToString();
                    testInfoBLL.GetModelFromDataRow(dr, testInfoModel);
                    athleteInfoBLL.GetAthleteInfoFromDataRow(dr, testInfoModel);
                    testInfoModelList.Add(testInfoModel);
                }
                if (AthleteInfo != null)
                {
                    List<Model.TestInfoModel> currentDateTestInfoList = testInfoModelList.FindAll(x => x.TestDate.ToString("yyyyMMdd") == AthleteInfo.Ath_TestDate.ToString("yyyyMMdd"));
                    if (currentDateTestInfoList.Count > 0)
                    {
                        foreach (Model.TestInfoModel model in currentDateTestInfoList)
                        {
                            testInfoModelList.Remove(model);//先移除
                            testInfoModelList.Insert(0, model);//插入到第一个
                        }
                        for (int i = 0; i < testInfoModelList.Count; i++)
                        {
                            testInfoModelList[i].Index = i + 1;
                        }
                    }
                }
            }
            else {
                testInfoModelList = new List<Model.TestInfoModel>();
            }
            Binding b = new Binding() { Source = testInfoModelList };
            dgTestInfo.SetBinding(DataGrid.ItemsSourceProperty, b);
        }

        //测试管理器选择改变事件
        private void testManager_ItemSelectionChangedEvent(Model.TB_TestManager selectedItem)
        {
            RefrenshDataGridSource();
        }

        //显示图表
        private void btnShowChart_Click(object sender, RoutedEventArgs e)
        {
            List<Model.TestInfoModel> selectedTestInfoList = testInfoModelList.FindAll(x => x.IsChecked == true);
            if (selectedTestInfoList.Count == 0)
            {
                MessageBox.Show("请至少选择一条信息查看！", "系统信息");
                return;
            }
            foreach (Model.TestInfoModel model in selectedTestInfoList)
            {
                ShowChartWindow chartWindow = new ShowChartWindow();
                chartWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                chartWindow.DataModel = model;
                chartWindow.Owner = Application.Current.MainWindow;
                chartWindow.Show();
            }
        }

        //全选
        private void checkAll_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.IsChecked == true)
            {
                foreach (Model.TestInfoModel bi in testInfoModelList)
                {
                    bi.IsChecked = true;
                }
                dgTestInfo.SelectAll();
            }
            else
            {
                foreach (Model.TestInfoModel bi in testInfoModelList)
                {
                    bi.IsChecked = false;
                }
                dgTestInfo.UnselectAll();
            }
        }

        //删除
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            List<Model.TestInfoModel> selectedTestInfoList = testInfoModelList.FindAll(x => x.IsChecked == true);
            if (selectedTestInfoList.Count == 0)
            {
                MessageBox.Show("请至少选择一条信息删除！", "系统信息");
                return;
            }
            if (MessageBox.Show("删除这些测试数据的同时也将删除测试参考值中的这些测试数据并且不能恢复。\r\n请确认要删除选择的测试数据吗？", "删除测试数据确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                foreach (Model.TestInfoModel model in selectedTestInfoList)
                {
                    testInfoBLL.Delete(model.TestID);
                    string fileName = AppDomain.CurrentDomain.BaseDirectory + "\\AppData\\XmlData\\" + model.DataFileName;
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }
                }
                RefrenshDataGridSource();
            }
        }

        //添加到标准
        private void btnAddStandard_Click(object sender, RoutedEventArgs e)
        {
            List<Model.TestInfoModel> checkedModelList = testInfoModelList.FindAll(x => x.IsChecked == true);
            if (checkedModelList.Count > 0)
            {
                AddToStandardWindow addWindow = new AddToStandardWindow();
                addWindow.Owner = Application.Current.MainWindow;
                addWindow.TestInfoModelList = checkedModelList;
                addWindow.Owner = Application.Current.MainWindow;
                addWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("请至少选择一条数据！","系统信息");
            }
        }

        //列表单击
        private void dgTestInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckChanged();
        }

        private void dgTestInfo_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgTestInfo.SelectedIndex >= 0) {
                CheckChanged();
                btnShowChart_Click(btnShowChart, null);
            }
        }

        private void CheckChanged() {
            if (dgTestInfo.SelectedIndex >= 0)
            {
                foreach (Model.TestInfoModel model in testInfoModelList)
                {
                    model.IsChecked = false;
                }
                if (dgTestInfo.SelectedItems.Count > 1)
                {
                    foreach (var item in dgTestInfo.SelectedItems)
                    {
                        Model.TestInfoModel testModel = item as Model.TestInfoModel;
                        testModel.IsChecked = true;
                    }
                }
                else
                {
                    Model.TestInfoModel testModel = dgTestInfo.SelectedItem as Model.TestInfoModel;
                    testModel.IsChecked = true;
                }
            }
        }

        //姓名搜索
        private void txtKeyWord_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefrenshDataGridSource();
        }

        //曲线对比
        private void btnShowCompareChart_Click(object sender, RoutedEventArgs e)
        {
            if (testInfoModelList!=null&&testInfoModelList.Count>0)
            {
                List<Model.TestInfoModel> selectedTestInfoList = testInfoModelList.FindAll(x => x.IsChecked == true);
                if (selectedTestInfoList.Count == 0)
                {
                    MessageBox.Show("请至少选择一条信息！", "系统信息");
                    return;
                }
                AvgCurveCompareWindow chartWindow = new AvgCurveCompareWindow();
                chartWindow.TestInfoModelList = selectedTestInfoList;
                chartWindow.Show();
            }
            else {
                MessageBox.Show("请至少选择一条信息！", "系统信息");
            }
        }

        //搜索
        private void cbJoint_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefrenshDataGridSource();
        }

        //导出数据
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            List<Model.TestInfoModel> checkedModelList = testInfoModelList.FindAll(x => x.IsChecked == true);
            if (checkedModelList.Count > 0)
            {
                ExportProgressPage.TestInfoList = checkedModelList;
                ExportDataWindow exportWindow = new ExportDataWindow();
                exportWindow.Owner = Application.Current.MainWindow;
                exportWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("请至少选择一条数据！", "系统信息");
            }
        }

        //列表行加载时添加右键功能
        private void dgTestInfo_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.ToolTip = "右键查看图表";
            e.Row.MouseRightButtonDown += (s, a) =>
            {
                a.Handled = true;
                DataGridRow dgr = s as DataGridRow;
                (sender as DataGrid).SelectedIndex = dgr.GetIndex();
                dgr.Focus();
                menu.PlacementTarget = dgr;
                menu.IsOpen = true;
            };
        }

        //导出多个报告
        private void btnExportReport_Click(object sender, RoutedEventArgs e)
        {
            List<Model.TestInfoModel> selectedTestInfoList = testInfoModelList.FindAll(x => x.IsChecked == true);
            if (selectedTestInfoList.Count == 0)
            {
                MessageBox.Show("请选择一条信息！", "系统信息");
                return;
            }
            ExportReportWindow window = new ExportReportWindow();
            window.Owner = Application.Current.MainWindow;
            window.TestInfoList = selectedTestInfoList;
            window.ShowDialog();
        }

        //快速导入
        private void btnQuickImport_Click(object sender, RoutedEventArgs e)
        {
            QuickImportTestDataWindow qitw = new QuickImportTestDataWindow();
            qitw.Owner = Application.Current.MainWindow;
            if (qitw.ShowDialog() == true) {
                testManager.RefrenshList();
                RefrenshDataGridSource();
            }
        }
    }
}
