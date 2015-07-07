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
using System.Windows.Navigation;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Threading;

namespace DSJL.Modules.Test.Export
{
    /// <summary>
    /// CheckParamsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ExportDataWindow : Window
    {
        public ExportDataWindow()
        {
            InitializeComponent();
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region 页切换效果
        private bool _allowDirectNavigation = false;
        private NavigatingCancelEventArgs _navArgs = null;
        private void Frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (frame.Content != null && !_allowDirectNavigation)
            {
                e.Cancel = true;
                _navArgs = e;
                this.IsHitTestVisible = false;
                DoubleAnimation da = new DoubleAnimation(0.3d, new Duration(TimeSpan.FromMilliseconds(300)));
                da.Completed += FadeOutCompleted;
                frame.BeginAnimation(OpacityProperty, da);
            }
            _allowDirectNavigation = false;
        }

        private void FadeOutCompleted(object sender, EventArgs e)
        {
            (sender as AnimationClock).Completed -= FadeOutCompleted;

            this.IsHitTestVisible = true;

            _allowDirectNavigation = true;
            switch (_navArgs.NavigationMode)
            {
                case NavigationMode.New:
                    if (_navArgs.Uri == null)
                    {
                        frame.Navigate(_navArgs.Content);
                    }
                    else
                    {
                        frame.Navigate(_navArgs.Uri);
                    }
                    break;
                case NavigationMode.Back:
                    frame.GoBack();
                    break;

                case NavigationMode.Forward:
                    frame.GoForward();
                    break;
                case NavigationMode.Refresh:
                    frame.Refresh();
                    break;
            }

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
                (ThreadStart)delegate()
            {
                DoubleAnimation da = new DoubleAnimation(1.0d, new Duration(TimeSpan.FromMilliseconds(200)));
                frame.BeginAnimation(OpacityProperty, da);
            });
        }
        #endregion
    }
}
