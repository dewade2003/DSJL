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
    public partial class EditStandardWindow : Window
    {
        public static readonly RoutedEvent AddSuccessRoutedEvent = EventManager.RegisterRoutedEvent(
   "AddSuccessRoutedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EditStandardWindow));

        public event RoutedEventHandler AddSuccessEvent
        {
            add { base.AddHandler(AddSuccessRoutedEvent, value); }

            remove { base.RemoveHandler(AddSuccessRoutedEvent, value); }

        }

        BLL.TB_StandardInfo standardInfoBLL;
        private Model.TB_StandardInfo standardInfo;

        public Model.TB_StandardInfo StandardInfo {
            set {
                standardInfo = value;
            }
            get {
                return standardInfo;
            }
        }

        public bool isAdd
        {
            get;
            set;
        }

        public EditStandardWindow()
        {
            InitializeComponent();
            standardInfoBLL = new BLL.TB_StandardInfo();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isAdd)
            {
                tbTitle.Text = "编辑" + standardInfo.Stand_Name;
                txtName.Text = standardInfo.Stand_Name;
            }
            else
            {
                if (standardInfo.Stand_Level == -1)
                {
                    tbTitle.Text = "添加测试参考值类别";
                }
                else {
                    tbTitle.Text = "为" + standardInfo.Stand_Name + "添加子参考值";
                }
            }
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
          
            bool result;
            if (!isAdd)
            {
                standardInfo.Stand_Name = txtName.Text;
                result = standardInfoBLL.Update(standardInfo);
            }
            else {
                Model.TB_StandardInfo info = new Model.TB_StandardInfo();
                info.Stand_Name = txtName.Text;
                if (standardInfo.Stand_Level == -1)
                {
                    info.Stand_Level = 1;
                    info.Stand_ParentID = 0;
                }
                else {
                    info.Stand_Level = 2;
                    info.Stand_ParentID = standardInfo.ID;
                }
              
                result = standardInfoBLL.Add(info);
            }
            if (!result)
            {
                MessageBox.Show("保存到数据库出错，请稍后重试！", "系统错误");
            }
            else {
                if (isAdd)
                {
                    RaiseEvent(new RoutedEventArgs(EditStandardWindow.AddSuccessRoutedEvent, this));
                    string desc = "添加成功！\r\n";
                    if (standardInfo.Stand_Level == -1)
                    {
                        desc += "是否继续添加参考值类别？";
                    }
                    else {
                        desc += "是否继续为" + standardInfo.Stand_Name + "添加子参考值？";
                    }
                    if (MessageBox.Show(desc, "系统信息", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        txtName.Text = "";
                        txtName.Focus();
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
    }
}
