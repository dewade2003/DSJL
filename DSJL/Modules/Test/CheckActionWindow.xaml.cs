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

namespace DSJL.Modules.Test
{
    /// <summary>
    /// CheckActionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CheckActionWindow : Window
    {
        Model.TB_Dict actionModel;
        BLL.TB_Dict dictBLl = new BLL.TB_Dict();

        public CheckActionWindow()
        {
            InitializeComponent();
        }

        public string Joint
        {
            get;
            set;
        }

        public string Plane
        {
            get;
            set;
        }

        public string TestModeAction
        {
            get;
            set;
        }

        public string ActionID
        {
            get;
            set;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //actionModel = dictBLl.GetModel(Joint, Plane);
                cbActionOne.Content = actionModel.actionone;
                cbActionTwo.Content = actionModel.actiontwo;

                cbActionOne.IsChecked = true;
            }
            catch {
                MessageBox.Show("未找到测试动作信息！","系统信息");
                this.Close();
            }
        }

        private void cbActionOne_Checked(object sender, RoutedEventArgs e)
        {
            TestModeAction = actionModel.actionone;
            ActionID = "1";
            cbActionTwo.IsChecked = false;
        }

        private void cbActionTwo_Checked(object sender, RoutedEventArgs e)
        {
            TestModeAction = actionModel.actiontwo;
            ActionID = "2";
            cbActionOne.IsChecked = false;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
