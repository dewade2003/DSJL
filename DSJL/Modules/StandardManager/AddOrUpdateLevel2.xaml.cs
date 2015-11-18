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
    /// 添加和编辑参考值信息
    /// </summary>
    public partial class AddOrUpdateLevel2 : Window
    {
        public static readonly RoutedEvent AddSuccessRoutedEvent = EventManager.RegisterRoutedEvent(
   "AddSuccessRoutedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AddOrUpdateLevel2));

        public event RoutedEventHandler AddSuccessEvent
        {
            add { base.AddHandler(AddSuccessRoutedEvent, value); }

            remove { base.RemoveHandler(AddSuccessRoutedEvent, value); }

        }

        BLL.TB_StandardInfo standardInfoBLL;

        BLL.TB_StandardParams standardParamsBLL;
        Model.TB_StandardParams standardParam;

        BLL.TB_AthleteInfo athInfoBLL;

        BLL.TB_Dict dictBLL = new BLL.TB_Dict();
        List<Model.TB_Dict> jointDictList;//测试关节字典列表
        List<Model.TB_Dict> jointsideDictList;//测试侧字典列表
        List<Model.TB_Dict> testmodeDictList;//测试模式字典列表
        List<Model.TB_Dict> planeDictList;//运动方式字典列表

        public Model.TB_StandardInfo StandardInfo {
            set;
            get;
        }

        public bool isAdd
        {
            get;
            set;
        }

        public AddOrUpdateLevel2()
        {
            InitializeComponent();
            standardInfoBLL = new BLL.TB_StandardInfo();
            standardParamsBLL = new BLL.TB_StandardParams();
            athInfoBLL = new BLL.TB_AthleteInfo();

            Model.TB_Dict allDict = new Model.TB_Dict()
            {
                Dict_Key = "-1",
                Dict_Value = "不限"
            };
            jointDictList = dictBLL.GetModelListByGroupID(2);
            jointDictList.Insert(0, allDict);
            jointsideDictList = dictBLL.GetModelListByGroupID(3);
            jointsideDictList.Insert(0, allDict);
            testmodeDictList = dictBLL.GetModelListByGroupID(1);
            testmodeDictList.Insert(0, allDict);

        }

        private void refrenshPlaneDictList() {
            if (cbJoint.SelectedItem==null)
            {
                return;
            }
            planeDictList = new List<Model.TB_Dict>();
            Model.TB_Dict selectJoint = (Model.TB_Dict)cbJoint.SelectedItem;
            if (!selectJoint.Dict_Key.Equals("-1"))
            {
                planeDictList = dictBLL.GetModelList("dict_groupid=" + jointDictList[1].ID + " and dict_groupid2='1,2,3,4,5,7,8,9'");
            }
          
            Model.TB_Dict allDict = new Model.TB_Dict()
            {
                Dict_Key = "-1",
                Dict_Value = "不限"
            };
            planeDictList.Insert(0, allDict);
            cbPlane.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = planeDictList });
            cbPlane.SelectedIndex = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            init();
         
        }

        private void init() {
            Binding dictBind = new Binding() { Source = jointDictList };
            cbJoint.SetBinding(ComboBox.ItemsSourceProperty, dictBind);
            cbJointSide.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = jointsideDictList });
            cbTestMode.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = testmodeDictList });
            cbPlane.SetBinding(ComboBox.ItemsSourceProperty, new Binding() { Source = planeDictList });
            if (!isAdd)
            {
                tbTitle.Text = "编辑" + StandardInfo.Stand_Name;
                standardParam = standardParamsBLL.GetModelByStandID(StandardInfo.ID);
                if (standardParam==null)
                {
                    standardParam = new Model.TB_StandardParams();
                    standardParam.StandID = StandardInfo.ID;
                }
                txtName.Text = StandardInfo.Stand_Name;
            }
            else
            {
                standardParam = new Model.TB_StandardParams();
                if (StandardInfo.Stand_Level == -1)
                {
                    tbTitle.Text = "添加测试参考值类别";
                }
                else
                {
                    tbTitle.Text = "为" + StandardInfo.Stand_Name + "添加子参考值";
                }
                txtName.Text = "";
                
            }

            Binding standParamBind = new Binding() { Source = standardParam };
            grid.SetBinding(Grid.DataContextProperty, standParamBind);

            txtName.Focus();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text == "") {
                MessageBox.Show("名称不能为空！","系统信息");
                return;
            }
            Model.TB_StandardInfo info = new Model.TB_StandardInfo();
            bool result;
            if (!isAdd)
            {
                StandardInfo.Stand_Name = txtName.Text;
                result = standardInfoBLL.Update(StandardInfo);
                standardParamsBLL.Update(standardParam);
            }
            else {
            
                info.Stand_Name = txtName.Text;
                if (StandardInfo.Stand_Level == -1)
                {
                    info.Stand_Level = 1;
                    info.Stand_ParentID = 0;
                }
                else {
                    info.Stand_Level = 2;
                    info.Stand_ParentID = StandardInfo.ID;
                }
              
                result = standardInfoBLL.Add(info);
                int maxStandInfoId=  standardInfoBLL.GetMaxId();
                standardParam.StandID = maxStandInfoId;
                standardParamsBLL.Add(standardParam);
            }
            if (!result)
            {
                MessageBox.Show("保存到数据库出错，请稍后重试！", "系统错误");
            }
            else {
                if (isAdd)
                {
                    RaiseEvent(new RoutedEventArgs(AddOrUpdateLevel2.AddSuccessRoutedEvent, this));
                    string desc = "添加成功！\r\n";
                    if (StandardInfo.Stand_Level == -1)
                    {
                        desc += "是否继续添加参考值类别？";
                    }
                    else {
                        desc += "是否继续为" + StandardInfo.Stand_Name + "添加子参考值？";
                    }
                    if (MessageBox.Show(desc, "系统信息", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        standardParam = new Model.TB_StandardParams();
                        txtName.Text = "";
                        init();
                    }
                    else
                    {
                        this.DialogResult = true;
                        this.Close();
                    }
                }
                else {
                    this.DialogResult = true;
                    this.Close();
                }
            
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

        private void cbJoint_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            refrenshPlaneDictList();

            //Model.TB_Dict selectedJoint = (Model.TB_Dict)cbJoint.SelectedItem;
            //planeDictList.RemoveAll(x => x.Dict_Key != "-1");
            //if (selectedJoint.Dict_Key == "-1")
            //{
            //    planeDictList.AddRange(dictBLL.GetModelList("dict_groupid=" + jointDictList[1].ID));
            //}
        }
    }
}
