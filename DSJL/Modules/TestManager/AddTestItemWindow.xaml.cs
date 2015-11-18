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

namespace DSJL
{
    /// <summary>
    /// AddTestItemWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddTestItemWindow : Window
    {
        private bool isEdit = false;

        private Model.TB_TestManager testItem = new Model.TB_TestManager();
        private BLL.TB_TestManager testManagerBLL;
        public AddTestItemWindow()
        {
            InitializeComponent();
            txtItemName.Focus();
        }

        public Model.TB_TestManager TestItem
        {
            set {
                testItem = value;
                isEdit = true;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (isEdit)
            {
                tbTitle.Text = "编辑" + testItem.TestName;
            }
            else {
                testItem.TestStartDate = testItem.TestEndDate = DateTime.Now.ToString("yyyy/MM/dd");
            }
            Binding b = new Binding() { Source = testItem };
            grid.SetBinding(Grid.DataContextProperty, b);
            testManagerBLL = new BLL.TB_TestManager();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtItemName.Text.Equals(string.Empty))
            {
                MessageBox.Show("测试项目名称不能为空！", "系统信息");
                return;
            }
            try
            {
                if (isEdit)
                {
                    testManagerBLL.Update(testItem);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    testManagerBLL.Add(testItem);
                    if (MessageBox.Show("添加成功，是否继续添加测试项目？", "系统信息", MessageBoxButton.YesNo) == MessageBoxResult.No) {
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        txtItemName.Text = "";
                        txtItemName.Focus();
                    }
                }
              
            }
            catch (Exception ee) {
                MessageBox.Show("保存数据出错！\r\n" + ee.Message);
            }

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
    }
}
