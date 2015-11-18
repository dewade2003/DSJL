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
using System.Windows.Shapes;

namespace DSJL.Modules.Standard
{
    /// <summary>
    /// AddToStandardWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddToStandardWindow : Window
    {
        BLL.TB_StandardInfo standBLL;
        BLL.TB_StandTestRefe refeBLL;

        List<Model.TB_StandardInfo> standList;

        List<Model.TestInfoModel> testInfoModelList;

        static Model.TB_StandardInfo selectStandInfo1, selectStandInfo2;
        public List<Model.TestInfoModel> TestInfoModelList
        {
            set
            {
                testInfoModelList = value;
            }
        }

        public AddToStandardWindow()
        {
            InitializeComponent();
            testInfoModelList = new List<Model.TestInfoModel>();
            standBLL = new BLL.TB_StandardInfo();
            refeBLL = new BLL.TB_StandTestRefe();
            standList = new List<Model.TB_StandardInfo>();
        }

     

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            standList = standBLL.GetModelList("");
            List<Model.TB_StandardInfo> standListLevel1 = standList.FindAll(x => x.Stand_Level == 1);
            lbLevel1.ItemsSource = standListLevel1;

            //lbLevel1.SelectedIndex = 0;
            lbLevel1.SelectedIndex = -1;
            if (selectStandInfo1 != null) {
                for (int i = 0; i < standListLevel1.Count; i++)
                {
                    if (standListLevel1[i].ID == selectStandInfo1.ID)
                    {
                        lbLevel1.SelectedIndex = i;
                        break;
                    }
                }
            }
           
            //lbLevel1.SelectedValue = selectStandInfo1;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var selectedStandList = standList.FindAll(x => x.IsChecked == true);
            if (selectedStandList.Count > 0)
            {
                foreach (Model.TB_StandardInfo standInfo in selectedStandList) {
                    foreach (Model.TestInfoModel testInfoModel in testInfoModelList) {
                        if (!refeBLL.Exists(testInfoModel.TestID, standInfo.ID)) {
                            Model.TB_StandTestRefe refeModel = new Model.TB_StandTestRefe();
                            refeModel.StandID = standInfo.ID;
                            refeModel.TestID = testInfoModel.TestID;
                            refeBLL.Add(refeModel);
                        }
                    }
                }

               

                MessageBox.Show("添加成功！", "系统信息");
                this.Close();
            }
            else {
                MessageBox.Show("添加失败，请至少选择一个测试参考值！", "系统信息");
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Point mousePoint = e.GetPosition(this);

                if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed && e.ChangedButton == System.Windows.Input.MouseButton.Left && mousePoint.Y <= 30)
                {

                    this.DragMove();
                }
            }
            catch { }
        }

        private void lbLevel1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Model.TB_StandardInfo selectedLevel1= lbLevel1.SelectedValue as Model.TB_StandardInfo;
            List<Model.TB_StandardInfo> standInfoList2 = standList.FindAll(x => x.Stand_ParentID == selectedLevel1.ID);
            if (standInfoList2.Count > 0 && selectStandInfo2 != null) {
                Model.TB_StandardInfo findedStandInfo = standInfoList2.Find(x => x.ID == selectStandInfo2.ID);
                if (findedStandInfo != null) {
                    findedStandInfo.IsChecked = true;
                }

            }
            Binding b = new Binding() { Source = standInfoList2 };
            lbLevel2.SetBinding(ListBox.ItemsSourceProperty, b);

            selectStandInfo1 = selectedLevel1;
        }

        private void lbLevel2_MouseDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void lbLevel2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Model.TB_StandardInfo selectedInfo = (lbLevel2.SelectedValue as Model.TB_StandardInfo);
            standList.Find(x => x.ID == selectedInfo.ID).IsChecked = !selectedInfo.IsChecked;

            selectStandInfo2 = selectedInfo;
        }
    }
}
