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
using DSJL.Modules.Standard;
using System.Collections.ObjectModel;

namespace DSJL.Compoments
{
    /// <summary>
    /// TestStandardManager.xaml 的交互逻辑
    /// </summary>
    public partial class TestStandardManager : UserControl
    {
        private Model.StandInfoTreeDataModel selectedTreeItem;
        private BLL.TB_StandardInfo standardBLL;
        private List<Model.TB_StandardInfo> standInfoList;

        private ObservableCollection<Model.StandInfoTreeDataModel> treeModelCollection;

        public delegate void ItemSelectionChangedDelegate(Model.TB_StandardInfo selectedItem);
        /// <summary>
        /// 事项选择改变触发事件
        /// </summary>
        public event ItemSelectionChangedDelegate ItemSelectionChangedEvent;

        private Model.TB_StandardInfo selectedItem = null;

        public Model.TB_StandardInfo SelectedItem
        {
            get
            {
                return selectedItem;
            }
        }

        public TestStandardManager()
        {
            InitializeComponent();
            standardBLL = new BLL.TB_StandardInfo();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            treeModelCollection = new ObservableCollection<Model.StandInfoTreeDataModel>();
            Model.StandInfoTreeDataModel rootModel = new Model.StandInfoTreeDataModel();
            rootModel.IsExpanded = true;
            rootModel.DefaultIcon = "/DSJL;component/Assets/Images/folder.png";
            rootModel.OpenedIcon = "/DSJL;component/Assets/Images/folder_opened.png";
            rootModel.StandInfo = new Model.TB_StandardInfo() { Stand_Name = "参考值类别", Stand_Level = -1 };
            rootModel.IsChecked = true;
            treeModelCollection.Add(rootModel);

            tree.SetBinding(TreeView.ItemsSourceProperty, new Binding() { Source = treeModelCollection });

            tree.Focus();

            selectedItem = null;
            ItemSelectionChangedEvent(selectedItem);
        }

        private void imgDelete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Model.StandInfoTreeDataModel selectedModel = tree.SelectedItem as Model.StandInfoTreeDataModel;
            if (selectedModel != null)
            {
                Model.TB_StandardInfo info = selectedModel.StandInfo;
                if (info.Stand_Level != -1)
                { //不是选择的全部
                    if (MessageBox.Show("删除信息将不能恢复，确定要删除该参考值信息吗？", "删除参考值确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        if (standardBLL.Delete(info.ID))
                        {
                            if (selectedModel.ParentModel != null) {
                                selectedModel.ParentModel.RefrenshChildren();
                            }
                        }
                        else
                        {
                            MessageBox.Show("删除错误！", "系统错误");
                        }
                    }
                }
            }
        }

        private void imgEdit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Model.StandInfoTreeDataModel selectedModel = tree.SelectedItem as Model.StandInfoTreeDataModel;
            if (selectedModel != null)
            {
                Model.TB_StandardInfo info = selectedModel.StandInfo;
                if (info.Stand_Level != -1) {
                    EditStandardWindow editWindow = new EditStandardWindow();
                    editWindow.Owner = Application.Current.MainWindow;
                    editWindow.isAdd = false;
                    editWindow.StandardInfo = info;
                    editWindow.ShowDialog();
                    if (editWindow.DialogResult == true)
                    {
                        selectedModel.RefrenshChildren();
                    }
                }
            }
          
        }

        private void imgAdd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Model.StandInfoTreeDataModel selectedModel = tree.SelectedItem as Model.StandInfoTreeDataModel;
            if (selectedModel != null)
            {
                Model.TB_StandardInfo info = selectedModel.StandInfo;
                switch (info.Stand_Level)
                {
                    case 1:
                    case -1:
                        EditStandardWindow editWindow = new EditStandardWindow();
                        editWindow.Owner = Application.Current.MainWindow;
                        editWindow.AddSuccessEvent += new RoutedEventHandler(editWindow_AddSuccessEvent);
                        editWindow.isAdd = true;
                        editWindow.StandardInfo = info;
                        editWindow.ShowDialog();
                        editWindow.AddSuccessEvent -= new RoutedEventHandler(editWindow_AddSuccessEvent);
                        break;
                }
            }
        }

        void editWindow_AddSuccessEvent(object sender, RoutedEventArgs e)
        {
            selectedTreeItem.RefrenshChildren();
            selectedTreeItem.IsExpanded = true;
        }

        private void tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (tree.SelectedItem != null) {
                Model.StandInfoTreeDataModel selectedModel = e.NewValue as Model.StandInfoTreeDataModel;
                if (selectedModel != null) {
                    selectedTreeItem = selectedModel;
                    Model.TB_StandardInfo info = selectedModel.StandInfo;
                    if (info.Stand_Level == 2)//点击二级标准时才触发
                    {
                        selectedItem = info;
                        if (ItemSelectionChangedEvent != null)
                        {
                            ItemSelectionChangedEvent(selectedItem);
                        }
                    }
                }
                
            }
        }
    }
}
