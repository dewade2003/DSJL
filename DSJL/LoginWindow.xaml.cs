using DSJL.Tools;
using System.Windows;
using System.Windows.Input;

namespace DSJL
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        MainWindow mainWindow;
        BLL.TB_AdminInfo adminInfoBLL;
        public LoginWindow()
        {
            InitializeComponent();
            txtName.Focus();
            //测试代码
            //BaseDataUtil util = new BaseDataUtil();
            //util.Test();
        }

        //拖动
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        //登录
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text == "") {
                MessageBoxTool.ShowConfirmMsgBox("用户名不能为空！");
                return;
            }
            if (txtPwd.Password == "") {
                MessageBoxTool.ShowConfirmMsgBox("密码不能为空！");
               
                return;
            }
            if (adminInfoBLL.GetModelList("AdminName='" + txtName.Text.Trim() + "' and AdminPwd='" + txtPwd.Password.Trim() + "'").Count > 0)
            {
                mainWindow = new MainWindow();
                Application.Current.MainWindow = mainWindow;
                this.Close();
                mainWindow.Show();
            }
            else {
                MessageBoxTool.ShowConfirmMsgBox("用户名或密码输入错误，请重新输入！！");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            adminInfoBLL = new BLL.TB_AdminInfo();
          
        }

        //关闭
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("确定退出软件吗？", "系统消息", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                Application.Current.Shutdown();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin_Click(btnLogin, System.EventArgs.Empty as RoutedEventArgs);
            }
        }
    }
}
