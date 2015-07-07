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

namespace DSJL.Compoments
{
    /// <summary>
    /// AddOrUpdateStandInfoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddOrUpdateStandInfoWindow : Window
    {
        private bool isEdit = false;
        private Model.TB_StandardInfo standardInfo = new Model.TB_StandardInfo();

        private BLL.TB_StandardInfo standardInfoBLL;

        public AddOrUpdateStandInfoWindow()
        {
            InitializeComponent();
            standardInfoBLL = new BLL.TB_StandardInfo();
        }

        public Model.TB_StandardInfo StandardInfo
        {
            set {
                standardInfo = value;
                isEdit = true;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (isEdit) {
                tbTitle.Text = "编辑 " + standardInfo.Stand_Name;
            }
            Binding b = new Binding() { Source = standardInfo };
            grid.SetBinding(Grid.DataContextProperty, b);
            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (isEdit)
            {
                standardInfoBLL.Update(standardInfo);
            }
            else {
                standardInfoBLL.Add(standardInfo);
            }
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
