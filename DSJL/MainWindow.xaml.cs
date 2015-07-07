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
using System.IO;
using DSJL.Model;
using Visifire.Charts;
using WPFHelper.Window;
using DSJL.Modules;
using Microsoft.Win32;
using DSJL.Export;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Threading;

namespace DSJL
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static List<MenuSource> listMenuData = null;//菜单数据

        PageAthleteManager pageAthleteManager;
        PageDBManager pageDBManager;
        PageSetup pageSetup;
        PageTestInfo pageTestInfo;
        PageStandard pageStandard;

        public MainWindow()
        {
            InitializeComponent();
            
            WindowHelper.RepairWindowBehavior(this);
            pageAthleteManager = new PageAthleteManager();
            pageDBManager = new PageDBManager();
            pageSetup = new PageSetup();
            pageTestInfo = new PageTestInfo();
            pageStandard = new PageStandard();

            listMenuData = new List<MenuSource>(){
                new MenuSource(){
                    MenuName="人员管理",
                    IconSource="Assets/Images/athlete.png",
                    Action=MenuAction.AthleteManager,
                    ActionPage=pageAthleteManager
                },
                new MenuSource(){
                    MenuName="测试信息",
                    IconSource="Assets/Images/test.png",
                    Action=MenuAction.TestInfo,
                    ActionPage=pageTestInfo
                },
                new MenuSource(){
                    MenuName="测试参考值管理",
                    IconSource="Assets/Images/statics.png",
                    Action=MenuAction.Statics,
                    ActionPage=pageStandard
                },
                new MenuSource(){
                    MenuName="数据库管理",
                    IconSource="Assets/Images/db.png",
                    Action=MenuAction.DBManager,
                    ActionPage=pageDBManager
                },
                new MenuSource(){
                    MenuName="设置",
                    IconSource="Assets/Images/setup.png",
                    Action=MenuAction.Setup,
                    ActionPage=pageSetup
                },
                new MenuSource(){
                    MenuName="关于软件",
                    IconSource="Assets/Images/help.png",
                    Action=MenuAction.Help,
                    ActionPage=null
                }
            };
            listMenu.ItemsSource = listMenuData;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            listMenu.SelectedIndex = 0;
        }
        //菜单改变
        private void listMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listMenuData[listMenu.SelectedIndex].Action != MenuAction.Help)
            {
                this.frame.Navigate(listMenuData[listMenu.SelectedIndex].ActionPage);
            }
            else {
                HelpWindow help = new HelpWindow();
                help.Owner = this;
                help.ShowDialog();
            }
        }

        #region 最大化 最小化 关闭
        //最小化
        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        //最大化
        private void btnMax_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                double screenHeight = SystemParameters.PrimaryScreenHeight;
                double screenWidth = SystemParameters.PrimaryScreenWidth;

                this.Left = (screenWidth - this.Width) / 2;
                this.Top = (screenHeight - this.Height) / 2;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        //退出
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion
        //拖动窗口
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized) {
                try
                {
                    Point mousePoint = e.GetPosition(this);

                    if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed && e.ChangedButton == System.Windows.Input.MouseButton.Left && mousePoint.Y <= 100)
                    {

                        this.DragMove();
                    }
                }
                catch{}
            }
        }

        private void btnDownloadTemplate_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "请选择保存文件的路径";
            sfd.DefaultExt = "xls";
            sfd.FileName = "受测者名单";
            sfd.OverwritePrompt = true;
            sfd.AddExtension = true;
            sfd.Filter = "Excel文件(*.xls)|*.xls";
            if (sfd.ShowDialog() == true)
            {
                string path = sfd.FileName;
                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\AppTemplate\\listofnames.xls", path, true);
                MessageBox.Show("保存成功！", "系统信息");
            }
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + "help.pdf");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("确定退出软件吗？", "系统消息", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
            else {
                e.Cancel = true;
            }
        }
    }
}
