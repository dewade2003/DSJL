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

namespace DSJL.Compoments
{
    /// <summary>
    /// StandardManager.xaml 的交互逻辑
    /// </summary>
    public partial class StandardManager : UserControl
    {
        BLL.TB_StandardInfo standardInfoBLL;
        List<Model.TB_StandardInfo> standInfoList;

        public delegate void ItemSelectionChangedDelegate(Model.TB_StandardInfo selectedItem);
        /// <summary>
        /// 事项选择改变触发事件
        /// </summary>
        public event ItemSelectionChangedDelegate ItemSelectionChangedEvent;

        private Model.TB_StandardInfo standItem=null;

        public Model.TB_StandardInfo SelectedItem
        {
            get
            {
                return standItem;
            }
        }

        public StandardManager()
        {
            InitializeComponent();
            standardInfoBLL = new BLL.TB_StandardInfo();
            standInfoList = new List<Model.TB_StandardInfo>();
        }

        public void SelectNull() {
            standItmesListBox.SelectedIndex = -1;
        }

        private void standItmesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (standItmesListBox.SelectedIndex >= 0)
            {
                standItem = standInfoList[standItmesListBox.SelectedIndex];
                if (ItemSelectionChangedEvent != null)
                {
                    ItemSelectionChangedEvent(standItem);
                }
            }
            else {
                standItem = null;
            }
        }

        private void imgAdd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AddOrUpdateStandInfoWindow addOrUpdateWindow = new AddOrUpdateStandInfoWindow();
            addOrUpdateWindow.Owner = Application.Current.MainWindow;
            addOrUpdateWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (addOrUpdateWindow.ShowDialog() == true) {
                RefrenshList();
            }
        }

        private void imgEdit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (standItmesListBox.SelectedIndex >= 0)
            {
                AddOrUpdateStandInfoWindow updateWindow = new AddOrUpdateStandInfoWindow();
                updateWindow.StandardInfo = standInfoList[standItmesListBox.SelectedIndex];
                updateWindow.Owner = Application.Current.MainWindow;
                updateWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                if (updateWindow.ShowDialog() == true)
                {
                    RefrenshList();
                }
            }
            else {
                MessageBox.Show("请选择一条数据编辑！", "系统信息");
            }
        }

        private void imgDelete_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (standItmesListBox.SelectedIndex >= 0)
            {
                if (MessageBox.Show("删除信息将不能恢复，确定删除选择的信息吗？", "系统信息", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    standardInfoBLL.Delete(standInfoList[standItmesListBox.SelectedIndex].ID);
                    RefrenshList();
                }
            }
            else
            {
                MessageBox.Show("请选择一条数据删除！", "系统信息");
            }
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            RefrenshList();
        }

        private void RefrenshList() {
            standInfoList = standardInfoBLL.GetModelList("");
            standItmesListBox.ItemsSource = standInfoList;
            standItmesListBox.SelectedIndex = 0;
        }
    }
}
