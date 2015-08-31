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

namespace DSJL.Compoments
{
    /// <summary>
    /// TestManager.xaml 的交互逻辑
    /// </summary>
    public partial class TestManager : UserControl
    {
        private static int selectedTestItemIndex = -1;

        BLL.TB_TestManager testManagerBLL;
        BLL.TB_TestInfo testInfoBLL;
        static List<Model.TB_TestManager> testList;

        public delegate void ItemSelectionChangedDelegate(Model.TB_TestManager selectedItem);
        /// <summary>
        /// 事项选择改变触发事件
        /// </summary>
        public event ItemSelectionChangedDelegate ItemSelectionChangedEvent;

        private Model.TB_TestManager testItem=null;

        public Model.TB_TestManager SelectedItem {
            get {
                return testItem;
            }
            set {
                testItem = value;
            }
        }

        public TestManager()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            RefrenshList();
        }

        public void RefrenshList()
        {
            testManagerBLL = new BLL.TB_TestManager();
            testItmesListBox.ItemsSource = testList = testManagerBLL.GetModelList("");

            //如果设置了测试项目，则选中设置的测试项目
            int index = -1;

            if (testItem != null)
            {
                index = testList.FindIndex(x => x.ID == testItem.ID);
            }
            else if (testList.Count>0)
            {
                index = 0;
            }

            if (selectedTestItemIndex >= 0 && testItem == null)
            {
                testItmesListBox.SelectedIndex = selectedTestItemIndex;
            }
            else
            {
                testItmesListBox.SelectedIndex = index;
            }
        }

        //添加
        private void imgAdd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AddTestItemWindow addWindow = new AddTestItemWindow();
            addWindow.Owner = Application.Current.MainWindow;
            if (addWindow.ShowDialog() == true) {
                selectedTestItemIndex = 0;
                RefrenshList();
            }
        }

        //修改
        private void imgEdit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (testItmesListBox.SelectedIndex >= 0)
            {
                AddTestItemWindow addWindow = new AddTestItemWindow();
                addWindow.Owner = Application.Current.MainWindow;
                addWindow.TestItem = testList[testItmesListBox.SelectedIndex];
                if (addWindow.ShowDialog() == true)
                {
                    RefrenshList();
                }
            }
            else
            {
                MessageBox.Show("请选择一项编辑！");
            }
        }

        //删除
        private void imgDelete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (testItmesListBox.SelectedIndex >= 0)
            {
                if (MessageBox.Show("删除测试项目将会删除该项目下所有受试者信息和测试数据，改变应用该项目中测试数据建立的测试参考值。\r\n删除的信息将不能恢复，请确认要删除选择的测试项目吗？", "删除测试项目确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    testInfoBLL = new BLL.TB_TestInfo();
                    List<Model.TB_TestInfo> testInfoList = testInfoBLL.GetModelList("ath_id in (select id from tb_athleteinfo where ath_testid=" + testList[testItmesListBox.SelectedIndex].ID + ")");

                    foreach (Model.TB_TestInfo testInfo in testInfoList)
                    {
                        string dataFileName = Model.AppPath.XmlDataDirPath + testInfo.DataFileName;
                        if (File.Exists(dataFileName))
                        {
                            File.Delete(dataFileName);
                        }
                    }
                    int selectTestID=testList[testItmesListBox.SelectedIndex].ID;
                    testManagerBLL.Delete(selectTestID);

                    //删除测试信息缓存
                    DSJL.Caches.TestInfoModelCache.DeleteCache(selectTestID);

                    RefrenshList();
                }
            }
            else {
                MessageBox.Show("请选择一项删除！");
            }
        }

        private void testItmesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            if (testItmesListBox.SelectedIndex >= 0)
            {
                testItem = testList[testItmesListBox.SelectedIndex];
                selectedTestItemIndex = testItmesListBox.SelectedIndex;
            }
            else {
                testItem = null;
            }

            if (ItemSelectionChangedEvent != null)
            {
                ItemSelectionChangedEvent(testItem);
            }
        }
    }
}
