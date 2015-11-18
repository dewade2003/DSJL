using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using DSJL.Dog;

namespace DSJL
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application,ISingleInstanceApp
    {
        private const string Unique = "DSJL";

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                var application = new App();
                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            string errStr = "";
            //if (!Dog.Dog.CheckDog(out errStr))
            //{
            //    MessageBox.Show("与加密狗通讯出错！\r\n错误信息：" + errStr, "系统信息");
            //    Application.Current.Shutdown();
            //    return;
            //}

            //////////测试代码
            //DateTime dt1 = DateTime.Now;
            //DateTime dt2 = new DateTime(2015, 11, 30);
            //int result = DateTime.Compare(dt1, dt2);
            //Console.WriteLine("result is:{0}",result);
            /////////测试代码结束


            Caches.Dict.DictCache.LoadDict();

            if (DSJL.Tools.DBUpgrade.Upgrade())
            {
                LoginWindow login = new LoginWindow();
                Application.Current.MainWindow = login;
                login.Show();

                //MainWindow mainWindow = new MainWindow();
                //Application.Current.MainWindow = mainWindow;
                //mainWindow.Show();
            }
            else {
                Application.Current.Shutdown();
            }
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            Application.Current.MainWindow.Activate();
            Application.Current.MainWindow.Topmost = true;
            Application.Current.MainWindow.Topmost = false;
            return true;
        }
    }
}
