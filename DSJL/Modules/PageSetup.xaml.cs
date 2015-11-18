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
using SaltFish.Security;
using DSJL.Compoments;
using DSJL.Modules.Setup;

namespace DSJL.Modules
{
    /// <summary>
    /// PageSetup.xaml 的交互逻辑
    /// </summary>
    public partial class PageSetup : Page
    {
        //AppPwdSetupPage pwdPage;
        //HiddenSetupPage hiddenPage;
        ////MonitorSetupPage monitorPage;

        //List<MenuSource> menuSource;

        public PageSetup()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //pwdPage = new AppPwdSetupPage();
            //hiddenPage = new HiddenSetupPage();
            ////monitorPage = new MonitorSetupPage();

            //menuSource = new List<MenuSource>() {
            //    new MenuSource(){
            //        MenuName="密码修改",
            //        ActionPage=pwdPage
            //    },
            //    new MenuSource(){
            //        MenuName="隐藏设置",
            //        ActionPage=hiddenPage
            //    }
            //    //,
            //    //new MenuSource(){
            //    //    MenuName="监视文件夹设置",
            //    //    ActionPage=monitorPage
            //    //}
            //};
            //lbMenu.SetBinding(ListBox.ItemsSourceProperty, new Binding() { Source = menuSource });
            //lbMenu.SelectedIndex = 0;
            cbShowAll.IsChecked = BLL.TB_Setting.IsShowAllData;
        }

        //private void lbMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (lbMenu.SelectedIndex < 0) {
        //        lbMenu.SelectedIndex = 0;
        //    }
        //    frame.Navigate(menuSource[lbMenu.SelectedIndex].ActionPage);
        //}

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
