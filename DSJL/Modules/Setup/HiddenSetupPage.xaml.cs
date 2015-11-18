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
using DSJL.Compoments;
using SaltFish.Security;

namespace DSJL.Modules.Setup
{
    /// <summary>
    /// HiddenSetupPage.xaml 的交互逻辑
    /// </summary>
    public partial class HiddenSetupPage : Page
    {
        public HiddenSetupPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            cbShowAll.IsChecked = BLL.TB_Setting.IsShowAllData;
        }

        private void btnHiddenSave_Click(object sender, RoutedEventArgs e)
        {
            HashEncrypt he = new HashEncrypt(true, false);
            if (BLL.TB_Setting.OldPwd == "")
            {
                if (txtHiddenNewPwd.Password == "")
                {
                    MessageBox.Show("请输入新密码！");
                    return;
                }
                if (txtHiddenReNewPwd.Password == "")
                {
                    MessageBox.Show("请输入确认新密码！");
                    return;
                }
                if (txtHiddenNewPwd.Password.Trim() != txtHiddenReNewPwd.Password.Trim())
                {
                    MessageBox.Show("新密码和确认新密码输入不一致，\r\n请重新输入！");
                    return;
                }
            }
            else
            {
                if (txtHiddenOldPwd.Password == "")
                {
                    MessageBox.Show("请输入原始密码！");
                    return;
                }
                if (txtHiddenNewPwd.Password == "")
                {
                    MessageBox.Show("请输入新密码！");
                    return;
                }
                if (txtHiddenReNewPwd.Password == "")
                {
                    MessageBox.Show("请输入确认新密码！");
                    return;
                }
                if (txtHiddenNewPwd.Password.Trim() != txtHiddenReNewPwd.Password.Trim())
                {
                    MessageBox.Show("新密码和确认新密码输入不一致，\r\n请重新输入！");
                    return;
                }
                if (he.SHA1Encrypt(txtHiddenOldPwd.Password.Trim()) != BLL.TB_Setting.OldPwd)
                {
                    MessageBox.Show("原始密码输入错误！", "系统信息");
                    return;
                }
            }
            BLL.TB_Setting.UpdatePwd(he.SHA1Encrypt(txtHiddenNewPwd.Password.Trim()));
            txtHiddenOldPwd.Password = "";
            txtHiddenNewPwd.Password = "";
            txtHiddenReNewPwd.Password = "";
            MessageBox.Show("密码设置成功！");
        }

        private void cbShowAll_Checked(object sender, RoutedEventArgs e)
        {
            if (BLL.TB_Setting.OldPwd == "")
            {
                AddHiddenPwdWindow window = new AddHiddenPwdWindow();
                window.Owner = Application.Current.MainWindow;
                if (window.ShowDialog() == true)
                {
                    BLL.TB_Setting.ShowAllData(true);
                }
                else
                {
                    cbShowAll.IsChecked = false;
                }
            }
            else
            {
                CheckHiddenPwdWindow window = new CheckHiddenPwdWindow();
                window.Owner = Application.Current.MainWindow;
                if (window.ShowDialog() == true)
                {
                    BLL.TB_Setting.ShowAllData(true);
                }
                else
                {
                    cbShowAll.IsChecked = false;
                }
            }
        }

        private void cbShowAll_Unchecked(object sender, RoutedEventArgs e)
        {
            BLL.TB_Setting.ShowAllData(false);
        }

    }
}
