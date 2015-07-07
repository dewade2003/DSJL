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
    /// AddHiddenPwdWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddHiddenPwdWindow : Window
    {
        public AddHiddenPwdWindow()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            HashEncrypt he = new HashEncrypt(true, false);
            BLL.TB_Setting.UpdatePwd(he.SHA1Encrypt(txtPwd.Password.Trim()));
            this.DialogResult = true;
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
