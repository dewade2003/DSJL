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
    /// ShowTestInfoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ShowTestInfoWindow : Window
    {
        public ShowTestInfoWindow()
        {
            InitializeComponent();
        }

        public Model.TB_AthleteInfo AthleteInfo
        {
            get;
            set;
        }

        public Model.TB_TestManager TestManager
        {
            get;
            set;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbTitle.Text = AthleteInfo.Ath_Name + "测试信息";

            DSJL.Modules.PageTestInfo pageTestInfo = new PageTestInfo();
            pageTestInfo.AthleteInfo = AthleteInfo;
            pageTestInfo.TestManager = TestManager;
            this.frame.Navigate(pageTestInfo);
        }

        //拖动
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

        //关闭
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
