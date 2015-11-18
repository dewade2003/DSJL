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
using DSJL.DBUtility;

namespace DSJL.Modules.Setup
{
    /// <summary>
    /// AppPwdSetupPage.xaml 的交互逻辑
    /// </summary>
    public partial class AppPwdSetupPage : Page
    {
        public AppPwdSetupPage()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtOldPwd.Password == "")
            {
                MessageBox.Show("请输入原始密码！");
                return;
            }
            if (txtNewPwd.Password == "")
            {
                MessageBox.Show("请输入新密码！");
                return;
            }
            if (txtReNewPwd.Password == "")
            {
                MessageBox.Show("请输入确认新密码！");
                return;
            }
            if (txtNewPwd.Password.Trim() != txtReNewPwd.Password.Trim())
            {
                MessageBox.Show("新密码和确认新密码输入不一致，\r\n请重新输入！");
                return;
            }
            string sql = "update tb_admininfo set adminpwd='" + txtNewPwd.Password.Trim() + "' where adminpwd='" + txtOldPwd.Password.Trim() + "' and adminname='admin'";
            try
            {
                int excuteCount = DbHelperOleDb.ExecuteSql(sql);
                if (excuteCount > 0)
                {
                    MessageBox.Show("密码修改成功！");
                    txtOldPwd.Password = "";
                    txtNewPwd.Password = "";
                    txtReNewPwd.Password = "";
                }
                else
                {
                    MessageBox.Show("原始密码输入错误，请重新输入！");
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("修改错误！\r\n" + ee.Message);
            }

        }
    }
}
