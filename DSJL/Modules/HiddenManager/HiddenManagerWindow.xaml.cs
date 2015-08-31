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
    /// HiddenManagerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HiddenManagerWindow : Window
    {
        List<Model.TB_AthleteInfo> athleteList = new List<Model.TB_AthleteInfo>();
        BLL.TB_AthleteInfo athleteBLL;
        public HiddenManagerWindow()
        {
            InitializeComponent();
            athleteBLL = new BLL.TB_AthleteInfo();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnVisibly_Click(object sender, RoutedEventArgs e)
        {
            string ids = GetSelectedID();
            if (ids == "")
            {
                MessageBox.Show("请至少选择一条信息！", "系统信息");
                return;
            }
            try
            {
                athleteBLL.HiddenData(ids, false);
                RefrenshAthleteList();
            }
            catch (Exception ee) {
                MessageBox.Show("保存到数据库出错！\r\n"+ee.Message, "系统错误");
            }
        }

        private string GetSelectedID()
        {
            string ids = "";
            List<Model.TB_AthleteInfo> selectedAthList = athleteList.FindAll(x => x.IsChecked == true);
            if (selectedAthList.Count != 0)
            {
                foreach (Model.TB_AthleteInfo ath in selectedAthList)
                {
                    ids += ath.ID.ToString();
                    if (ath != selectedAthList.Last())
                    {
                        ids += ",";
                    }
                }
            }
            return ids;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefrenshAthleteList();
        }

        private void RefrenshAthleteList()
        {
            athleteList = athleteBLL.GetAllModelList("hidden=1");
            dgAthlete.ItemsSource = athleteList;
        }

        //全选
        private void checkAll_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.IsChecked == true)
            {
                foreach (Model.TB_AthleteInfo bi in athleteList)
                {
                    bi.IsChecked = true;
                }
                dgAthlete.SelectAll();
            }
            else
            {
                foreach (Model.TB_AthleteInfo bi in athleteList)
                {
                    bi.IsChecked = false;
                }
                dgAthlete.UnselectAll();
            }
        }

        private void dgAthlete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgAthlete.SelectedIndex >= 0)
            {
                foreach (Model.TB_AthleteInfo bi in athleteList)
                {
                    bi.IsChecked = false;
                }
                if (dgAthlete.SelectedItems.Count > 1)
                {
                    foreach (var selectedItem in dgAthlete.SelectedItems)
                    {
                        Model.TB_AthleteInfo selectedAth = selectedItem as Model.TB_AthleteInfo;
                        selectedAth.IsChecked = true;
                    }
                }
                else
                {
                    Model.TB_AthleteInfo selectedAth = dgAthlete.SelectedItem as Model.TB_AthleteInfo;
                    selectedAth.IsChecked = true;
                }
            }
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
