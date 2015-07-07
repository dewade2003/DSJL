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
using DSJL.Modules.Athletes;

namespace DSJL.Modules.Test
{
    /// <summary>
    /// CheckAthWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CheckAthWindow : Window
    {
        List<Model.TB_AthleteInfo> athleteList = new List<Model.TB_AthleteInfo>();
        BLL.TB_AthleteInfo athleteBLL;

        private Model.TB_AthleteInfo currentAthInfo;
        private Model.TB_AthleteInfo checkedAth;

        private Model.TB_TestManager selectedTestItem;

        public Model.TB_TestManager SelectedTestItem {
            set {
                selectedTestItem = value;
                testManager.SelectedItem = selectedTestItem;
            }
        }

        public Model.TB_AthleteInfo CurrentAthInfo {
            set {
                currentAthInfo = value;
            }
        }

        public Model.TB_AthleteInfo CheckedAth
        {
            get {
                return checkedAth;
            }
        }

        public CheckAthWindow()
        {
            InitializeComponent();
            athleteBLL = new BLL.TB_AthleteInfo();
            testManager.ItemSelectionChangedEvent += new Compoments.TestManager.ItemSelectionChangedDelegate(testManager_ItemSelectionChangedEvent);
        }

        void testManager_ItemSelectionChangedEvent(Model.TB_TestManager selectedItem)
        {
            RefrenshAthleteList();
        }

        private void RefrenshAthleteList()
        {
            if (testManager.SelectedItem != null)
            {
                athleteList = athleteBLL.GetModelList("ath_testid=" + testManager.SelectedItem.ID);
                dgAthlete.ItemsSource = athleteList;
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void dgAthlete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgAthlete.SelectedIndex >= 0)
            {
                foreach (Model.TB_AthleteInfo athInfo in athleteList) {
                    athInfo.IsChecked = false;
                }
                Model.TB_AthleteInfo selectedAth = dgAthlete.SelectedItem as Model.TB_AthleteInfo;
                selectedAth.IsChecked = true;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            List<Model.TB_AthleteInfo> selectedAthleteList = athleteList.FindAll(model => model.IsChecked == true);
            if (selectedAthleteList.Count > 0)
            {
                checkedAth = selectedAthleteList[0];
                this.DialogResult = true;
                this.Close();
            }
            else {
                this.DialogResult = false;
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbFileContent.Text += "姓名拼音：" + currentAthInfo.Ath_Name+"\r\n";
            tbFileContent.Text += "性别：" + currentAthInfo.Ath_Sex + "\r\n";
            tbFileContent.Text += "生日：" + currentAthInfo.Ath_Birthday + "\r\n";
            tbFileContent.Text += "身高：" + currentAthInfo.Ath_Height + "\r\n";
            tbFileContent.Text += "体重：" + currentAthInfo.Ath_Weight + "\r\n";
            tbFileContent.Text += "测试日期：" + currentAthInfo.Ath_TestDate;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddOrEditAthlete window = new AddOrEditAthlete();
            window.TestItem = testManager.SelectedItem;
            window.CurrentAthInfo = currentAthInfo;
            window.Owner = this;
            if (window.ShowDialog() == true)
            {
                RefrenshAthleteList();
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            ImportAthlete window = new ImportAthlete();
            window.TestItem = testManager.SelectedItem;
            window.Owner = this;
            if (window.ShowDialog() == true)
            {
                RefrenshAthleteList();
            }
        }
    }
}
