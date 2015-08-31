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
using DSJL.Caches;

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

        Dictionary<int, List<Model.TestInfoModel>> testInfoModelListDict = new Dictionary<int, List<Model.TestInfoModel>>();

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
                //if (testInfoModelListDict.ContainsKey(testManager.SelectedItem.ID))
                //{
                //    testInfoModelListDict.Remove(testManager.SelectedItem.ID);
                //}
                //testManager.RefrenshList();
                TestInfoModelCache.Refrensh(testManager.SelectedItem.ID);
                RefrenshDataGridSource();
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
            miShowChart.Click += new RoutedEventHandler(btnShowChart_Click);

            menu.Items.Add(new Separator());

            MenuItem miAddInStandard = new MenuItem();
            miAddInStandard.Header = "加入测试参考值";
            menu.Items.Add(miAddInStandard);
            miAddInStandard.Click += new RoutedEventHandler(btnAddStandard_Click);

            MenuItem miCompare = new MenuItem();
            miCompare.Header = "数据对比";
            menu.Items.Add(miCompare);
            miCompare.Click += new RoutedEventHandler(btnShowCompareChart_Click);

            menu.Items.Add(new Separator());
            MenuItem miDelete = new MenuItem();
            miDelete.Header = "删除";
            menu.Items.Add(miDelete);
            miDelete.Click += new RoutedEventHandler(btnDelete_Click);
        }

        //刷新数据表
        private void RefrenshDataGridSource() {
            if (testManager.SelectedItem != null)
            {
                TestInfoModelParams param = new TestInfoModelParams();
                if (txtKeyWord.Text.Trim() != "")
                {
                    param.athName = txtKeyWord.Text.Trim();
                }
                if (cbJoint.SelectedIndex > 0)
                {
                    param.testJoint = jointDictList[cbJoint.SelectedIndex].Dict_Key;
                }
                if (cbTestMode.SelectedIndex > 0)
                {
                    param.testMode = testmodeDictList[cbTestMode.SelectedIndex].Dict_Key;
                }
                if (cbJointSide.SelectedIndex > 0)
                {
                    param.testJointSide = jointsideDictList[cbJointSide.SelectedIndex].Dict_Key;
                }
                if (AthleteInfo != null)
                {
                    param.athCode = AthleteInfo.Ath_Code;
                }
                testInfoModelList = TestInfoModelCache.GetTestInfoModelListByTestID(testManager.SelectedItem.ID,param);
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
                TestInfoModelCache.Refrensh(testManager.SelectedItem.ID);
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
                List<Model.TestInfoModel> selectedTestInfoList = TestInfoModelCache.GetCheckedModelList();
                //List<Model.TestInfoModel> selectedTestInfoList = testInfoModelList.FindAll(x => x.IsChecked == true);
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
                ExportProgressPage.FileNamePreExt = "";
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
            //e.Row.ToolTip = "右键查看图表";
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
              
                //RefrenshDataGridSource();
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            TestInfoModelCache.SetUncheck();
        }

        private void dgTestInfo_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            List<Model.TestInfoModel> checkedModelList = testInfoModelList.FindAll(x=>x.IsChecked==true);
            if (checkedModelList.Count>0)
            {
                menu.IsOpen = true;
            }
        
        }

        private void StackPanel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            StackPanel target=(StackPanel)sender;
            int index=(int)target.Tag;
            Model.TestInfoModel selectedModel = testInfoModelList.Find(x => x.Index == index);
            selectedModel.IsChecked =! selectedModel.IsChecked;
        }
    }
}
