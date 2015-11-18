using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DSJL.Modules.Standard;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DSJL.Model;

namespace DSJL.Compoments
{
    /// <summary>
    /// TestStandardManager.xaml 的交互逻辑
    /// </summary>
    public partial class TestStandardManager : UserControl
    {
        private Model.StandInfoTreeDataModel selectedTreeItem;
        private BLL.TB_StandardInfo standardBLL;

        private ObservableCollection<Model.StandInfoTreeDataModel> treeModelCollection=new ObservableCollection<StandInfoTreeDataModel>();

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
          
        }
        Model.StandInfoTreeDataModel rootModel = new Model.StandInfoTreeDataModel();
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            standardBLL = new BLL.TB_StandardInfo();
            if (treeModelCollection.Count==0)
            {
                rootModel.IsExpanded = true;
                rootModel.DefaultIcon = "/DSJL;component/Assets/Images/folder.png";
                rootModel.OpenedIcon = "/DSJL;component/Assets/Images/folder_opened.png";
                rootModel.StandInfo = new Model.TB_StandardInfo() { Stand_Name = "参考值类别", Stand_Level = -1 };
                rootModel.IsChecked = true;
                LoadStandInfo();
            }
            tree.Focus();

            selectedItem = null;
            ItemSelectionChangedEvent(selectedItem);
        }

        public void LoadStandInfo() {
            rootModel.Children.Clear();
            treeModelCollection.Clear();

            List<Model.TB_StandardInfo> standInfoList = standardBLL.GetModelList("");

            Task task = new Task(() =>
            {
                //加载导入的测试参考值
                List<Model.TB_StandardInfo> importedlevel1List = Stand.StandConfig.GetParentStandList();
                if (importedlevel1List!=null)
                {
                    foreach (var item in importedlevel1List)
                    {
                        StandInfoTreeDataModel treeModel = new StandInfoTreeDataModel();
                        treeModel.ParentModel = rootModel;
                        treeModel.StandInfo = item;
                        treeModel.DefaultIcon = "/DSJL;component/Assets/Images/folder.png";
                        treeModel.OpenedIcon = "/DSJL;component/Assets/Images/folder_opened.png";

                        List<Model.TB_StandardInfo> importedChildList = Stand.StandConfig.GetChildStandInfo(item.Stand_Name);
                        foreach (var item2 in importedChildList)
                        {
                            StandInfoTreeDataModel childTreeModel = new StandInfoTreeDataModel();
                            childTreeModel.ParentModel = treeModel;
                            childTreeModel.StandInfo = item2;
                            childTreeModel.DefaultIcon = "/DSJL;component/Assets/Images/file.png";
                            childTreeModel.OpenedIcon = "/DSJL;component/Assets/Images/file.png";

                            treeModel.Children.Add(childTreeModel);
                        }
                        item.Stand_Name += "(导入)";
                        rootModel.Children.Add(treeModel);
                    }
                }

                var level1List = from items in standInfoList where items.Stand_Level ==1 orderby items.ID select items;
                var level2List= from items in standInfoList where items.Stand_Level == 2 orderby items.ID select items;
                foreach (var item in level1List)
                {
                    StandInfoTreeDataModel treeModel = new StandInfoTreeDataModel();
                    treeModel.ParentModel = rootModel;
                    treeModel.StandInfo = item;
                    treeModel.DefaultIcon = "/DSJL;component/Assets/Images/folder.png";
                    treeModel.OpenedIcon = "/DSJL;component/Assets/Images/folder_opened.png";

                    var childList = from standItem in level2List where standItem.Stand_ParentID == item.ID select standItem;
               
                    foreach (var item2 in childList)
                    {
                        StandInfoTreeDataModel childTreeModel= new StandInfoTreeDataModel();
                        childTreeModel.ParentModel = treeModel;
                        childTreeModel.StandInfo = item2;
                        childTreeModel.DefaultIcon = "/DSJL;component/Assets/Images/file.png";
                        childTreeModel.OpenedIcon = "/DSJL;component/Assets/Images/file.png";

                        treeModel.Children.Add(childTreeModel);
                    }
                    rootModel.Children.Add(treeModel);
                     
                }
                this.Dispatcher.Invoke(new Action(() =>
                {
                    treeModelCollection.Add(rootModel);
                    tree.ItemsSource = treeModelCollection;
                }));
            });
            task.Start();
        }

        private void imgDelete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Model.StandInfoTreeDataModel selectedModel = tree.SelectedItem as Model.StandInfoTreeDataModel;
            
            
            if (selectedModel != null)
            {
                if (selectedTreeItem?.StandInfo.Tag == -1)//import stand
                {
                    if (DSJL.Tools.MessageBoxTool.ShowAskMsgBox("确定删除导入的参考值吗？") == MessageBoxResult.Yes)
                    {
                        try
                        {
                            DSJL.Stand.StandConfig.DeleteStand(selectedModel.StandInfo);
                            selectedModel.ParentModel.Children.Remove(selectedModel);
                            selectedModel = null;
                        }
                        catch (Exception ee)
                        {

                            DSJL.Tools.MessageBoxTool.ShowConfirmMsgBox("删除参考值出错！\r\n"+ee.Message);
                        }
                       
                    }
                    return;
                }

                Model.TB_StandardInfo info = selectedModel.StandInfo;
                if (info.Stand_Level != -1)
                { //不是选择的全部
                    if (MessageBox.Show("删除信息将不能恢复，确定要删除该参考值信息吗？", "删除参考值确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        if (standardBLL.Delete(info.ID))
                        {
                            selectedModel.ParentModel.Children.Remove(selectedModel);
                            selectedModel = null;
                       
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
            if (SelectedItemIsImported())
            {
                return;
            }

            Model.StandInfoTreeDataModel selectedModel = tree.SelectedItem as Model.StandInfoTreeDataModel;
            if (selectedModel != null)
            {
                Model.TB_StandardInfo info = selectedModel.StandInfo;
                switch (info.Stand_Level)
                {
                    case 1:
                        AddOrUpdateLevel1 editWindow1 = new AddOrUpdateLevel1();
                        editWindow1.Owner = Application.Current.MainWindow;
                        editWindow1.isAdd = false;
                        editWindow1.StandardInfo = info;
                        editWindow1.ShowDialog();
                        break;
                    case 2:
                        AddOrUpdateLevel2 editWindow2 = new AddOrUpdateLevel2();
                        editWindow2.Owner = Application.Current.MainWindow;
                        editWindow2.isAdd = false;
                        editWindow2.StandardInfo = info;
                        editWindow2.ShowDialog();
                        break;
                }
            }
          
        }

        private void imgAdd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectedItemIsImported())
            {
                return;
            }

            Model.StandInfoTreeDataModel selectedModel = tree.SelectedItem as Model.StandInfoTreeDataModel;
            if (selectedModel != null)
            {
                Model.TB_StandardInfo info = selectedModel.StandInfo;
                switch (info.Stand_Level)
                {
                    case 1:
                          AddOrUpdateLevel2 editWindow2 = new AddOrUpdateLevel2();
                          editWindow2.Owner = Application.Current.MainWindow;
                          editWindow2.AddSuccessEvent += new RoutedEventHandler(editWindow_AddSuccessEvent);
                          editWindow2.isAdd = true;
                          editWindow2.StandardInfo = info;
                          editWindow2.ShowDialog();
                          editWindow2.AddSuccessEvent -= new RoutedEventHandler(editWindow_AddSuccessEvent);
                          break;
                    case -1:
                        AddOrUpdateLevel1 editWindow1 = new AddOrUpdateLevel1();
                        editWindow1.Owner = Application.Current.MainWindow;
                        editWindow1.AddSuccessEvent += new RoutedEventHandler(editWindow_AddSuccessEvent);
                        editWindow1.isAdd = true;
                        editWindow1.StandardInfo = info;
                        editWindow1.ShowDialog();
                        editWindow1.AddSuccessEvent -= new RoutedEventHandler(editWindow_AddSuccessEvent);
                        break;
                }
            }
        }

        private bool SelectedItemIsImported() {
            bool result = false;
            if (selectedTreeItem?.StandInfo.Tag==-1)
            {
                Tools.MessageBoxTool.ShowConfirmMsgBox("导入的测试参考值不可添加子参考值，编辑或删除！");
                result = true;
            }
            return result;
        }

        void editWindow_AddSuccessEvent(object sender, RoutedEventArgs e)
        {
            List<Model.TB_StandardInfo> standInfoList = standardBLL.GetModelList(string.Format("stand_parentid={0}", selectedItem.Stand_Level == -1 ? 0 : selectedItem.ID));
            if (standInfoList?.Count>0)
            {
                StandInfoTreeDataModel treeModel = new StandInfoTreeDataModel();
                treeModel.ParentModel = selectedTreeItem;
                treeModel.StandInfo = standInfoList.Last();
                if (selectedItem.Stand_Level == -1)
                {
                    treeModel.DefaultIcon = "/DSJL;component/Assets/Images/folder.png";
                    treeModel.OpenedIcon = "/DSJL;component/Assets/Images/folder_opened.png";
                }
                else if (selectedItem.Stand_Level == 1)
                {
                    treeModel.DefaultIcon = "/DSJL;component/Assets/Images/file.png";
                    treeModel.OpenedIcon = "/DSJL;component/Assets/Images/file.png";
                }
                selectedTreeItem.Children.Add(treeModel);
                selectedTreeItem.IsExpanded = true;
            }
        }

        private void tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (tree.SelectedItem != null) {
                Model.StandInfoTreeDataModel selectedModel = e.NewValue as Model.StandInfoTreeDataModel;
                if (selectedModel != null) {
                    selectedTreeItem = selectedModel;
                    Model.TB_StandardInfo info = selectedModel.StandInfo;
                    //if (info.Stand_Level >= 1)//点击一、二级标准时才触发
                    //{
                        selectedItem = info;
                        if (ItemSelectionChangedEvent != null)
                        {
                            ItemSelectionChangedEvent(selectedItem);
                        }
                    //}
                }
                
            }
        }
    }
}
