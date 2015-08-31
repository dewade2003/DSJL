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
    /// AddDataWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddDataWindow : Window
    {
        BLL.TB_StandardParams standPramsBLL;
        BLL.TB_AthleteInfo athleteBLL;
        BLL.TB_TestInfo testInfoBLL;
        BLL.TB_StandTestRefe standTestRefeBLL;

        Dictionary<int, List<Model.TB_AthleteInfo>> athInfoListDict = new Dictionary<int, List<Model.TB_AthleteInfo>>();
        List<Model.TB_AthleteInfo> athleteList = new List<Model.TB_AthleteInfo>();
        Model.TB_StandardParams standParam,parentStandParam;
        public Model.TB_StandardInfo StandInfo
        {
            get;
            set;
        }
        public AddDataWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            standTestRefeBLL = new BLL.TB_StandTestRefe();
            testInfoBLL = new BLL.TB_TestInfo();
            athleteBLL = new BLL.TB_AthleteInfo();
            standPramsBLL = new BLL.TB_StandardParams();
            standParam = standPramsBLL.GetModelByStandID(StandInfo.ID);
            parentStandParam = standPramsBLL.GetModelByStandID((int)StandInfo.Stand_ParentID);
            tbTitle.Text +="  当前测试参考值为：" +StandInfo.Stand_Name;
        }

        private void RefrenshAthleteList()
        {
            if (testManager.SelectedItem != null)
            {
                if (athInfoListDict.ContainsKey(testManager.SelectedItem.ID))
                {
                    athleteList = athInfoListDict[testManager.SelectedItem.ID];
                }
                else {
                    StringBuilder searchCaseBuilder = new StringBuilder();
                    searchCaseBuilder.Append("ath_testid=" + testManager.SelectedItem.ID);
                    if (parentStandParam!=null)
                    {
                        if (!parentStandParam.Ath_Sex.Equals("不限"))
                        {
                            searchCaseBuilder.Append(" and ath_sex='" + standParam.Ath_Sex + "'");
                        }
                        if (!parentStandParam.Ath_MainProject.Equals("不限"))
                        {
                            searchCaseBuilder.Append(" and ath_mainproject='" + standParam.Ath_MainProject + "'");
                        }
                        if (!parentStandParam.Ath_Project.Equals("不限"))
                        {
                            searchCaseBuilder.Append(" and ath_project='" + standParam.Ath_Project + "'");
                        }
                    }
                    athleteList = new List<Model.TB_AthleteInfo>();
                    List<Model.TB_AthleteInfo> athList = athleteBLL.GetModelList(searchCaseBuilder.ToString());
                    if (parentStandParam.Ath_AgeMinLimit != -1 && parentStandParam.Ath_AgeMaxLimit != -1)
                    {
                        foreach (var item in athList)
                        {
                            if (item.Age >= parentStandParam.Ath_AgeMinLimit && item.Age <= parentStandParam.Ath_AgeMaxLimit)
                            {
                                athleteList.Add(item);
                            }
                        }
                    }
                    else {
                        athleteList = athList;
                    }

                    athInfoListDict.Add(testManager.SelectedItem.ID, athleteList);
                }
                dgAthlete.SetBinding(DataGrid.ItemsSourceProperty, new Binding() { Source = athleteList });
            }
        }

        //测试事项选择改变事件
        private void testManager_ItemSelectionChangedEvent(Model.TB_TestManager selectedItem)
        {
            RefrenshAthleteList();
        }

        //列表单击
        private void dgAthlete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgAthlete.SelectedIndex >= 0)
            {
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
                else
                {
                    Model.TB_AthleteInfo selectedAth = dgAthlete.SelectedItem as Model.TB_AthleteInfo;
                    selectedAth.IsChecked = true;
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            List<int> athIDs = new List<int>();
            foreach (var item in athInfoListDict.Values)
            {
                foreach (var item1 in item)
                {
                    if (item1.IsChecked)
                    {
                        athIDs.Add(item1.ID);
                    }
                }
            }
            StringBuilder caseStringBuilder = new StringBuilder();
            caseStringBuilder.Append("ath_id in(" + string.Join(",", athIDs.ToArray()) + ")");
            if (!standParam.Joint.Equals("-1"))
            {
                caseStringBuilder.Append(" and joint='" + standParam.Joint+"'");
            }
            if (!standParam.Joint_Side.Equals("-1"))
            {
                caseStringBuilder.Append(" and joint_side='" + standParam.Joint_Side+"'");
            }
            if (!standParam.Plane.Equals("-1"))
            {
                caseStringBuilder.Append(" and plane='" + standParam.Plane+"'");
            }
            if (!standParam.Test_Mode.Equals("-1"))
            {
                caseStringBuilder.Append(" and test_mode='" + standParam.Test_Mode+"'");
            }
            if (standParam.Speed1 != "-1" && standParam.Speed2 != "-1")
            {
                caseStringBuilder.Append(" and CInt(speed1)>="+standParam.Speed1+" and CInt(speed2)<="+standParam.Speed2);
            }
            List<Model.TB_TestInfo> testInfoList = testInfoBLL.GetModelList(caseStringBuilder.ToString());
            //List<Model.TB_TestInfo> willAddedTestInfoList = new List<Model.TB_TestInfo>();
            int count = 0;
            foreach (var item in testInfoList)
            {
                if (!standTestRefeBLL.Exists(item.ID,StandInfo.ID))
                {
                    Model.TB_StandTestRefe refe = new Model.TB_StandTestRefe();
                    refe.StandID = StandInfo.ID;
                    refe.TestID = item.ID;
                    standTestRefeBLL.Add(refe);
                    count++;
                }
            }
            if (count==0)
            {
                MessageBox.Show("没有找到测试数据！", "系统信息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else 
            {
                DialogResult = true;
                MessageBox.Show("添加了" + count + "条测试数据！", "系统信息", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
         
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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

      
    }
}
