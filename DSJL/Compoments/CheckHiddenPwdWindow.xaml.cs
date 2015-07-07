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
using SaltFish.Security;

namespace DSJL.Compoments
{
    /// <summary>
    /// CheckHiddenPwdWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CheckHiddenPwdWindow : Window
    {
        HashEncrypt he = new HashEncrypt(true, false);
        public CheckHiddenPwdWindow()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (he.SHA1Encrypt(txtPwd.Password.Trim()) == BLL.TB_Setting.OldPwd)
            {
                this.DialogResult = true;
                this.Close();
            }
            else {
                MessageBox.Show("密码输入错误！","系统信息");
                txtPwd.Password = "";
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
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
