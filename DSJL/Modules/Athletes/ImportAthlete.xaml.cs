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
using Microsoft.Win32;
using DSJL.ExcelDao;
using System.Data.OleDb;
using System.Data;
using NPinyin;

namespace DSJL.Modules.Athletes
{
    /// <summary>
    /// ImportAthlete.xaml 的交互逻辑
    /// </summary>
    public partial class ImportAthlete : Window
    {
        BLL.TB_TestManager testManagerBLL;
        BLL.TB_AthleteInfo athInfoBLL;
        List<Model.TB_TestManager> testList;
        List<List<string>> excelContentList;

        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);

        private Model.TB_TestManager testItem;

        public Model.TB_TestManager TestItem {
            set {
                testItem = value;
            }
        }

        public ImportAthlete()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            testManagerBLL = new BLL.TB_TestManager();
            athInfoBLL = new BLL.TB_AthleteInfo();
            testList = testManagerBLL.GetModelList("");
            Binding b = new Binding() { Source = testList };
            cbTestItems.SetBinding(ComboBox.ItemsSourceProperty, b);
            if (testItem == null)
            {
                cbTestItems.SelectedIndex = 0;
            }
            else {
                cbTestItems.SelectedIndex = testList.FindIndex(x => x.ID == testItem.ID);
            }
        }

        private void btnChooseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel文件(*.xls,*.xlsx)|*.xls;*.xlsx";
            ofd.Title = "选择测试者信息文件";
            if (ofd.ShowDialog() == true) {
                try
                {
                    excelContentList = ExcelUtil.Read(ofd.FileName, 15);
                    if (excelContentList.Count == 0)
                    {
                        MessageBox.Show("名单文件中没有信息，请重新选择！", "系统信息");
                    }
                    else {
                        tbFilePath.Text = ofd.FileName;
                        importProgress.Maximum = excelContentList.Count;
                        importProgress.Minimum = 0;
                        importProgress.Value = 0;

                        tbProgress.Text = "0/" + excelContentList.Count;
                    }
                }
                catch (Exception ee) {
                    MessageBox.Show("读取Excel文件出错，请稍候再试！\r\n" + ee.Message,"系统错误");
                }
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            dgAthlete.ItemsSource = null;
            tbAllCount.Text = "";
            tbSuccessCount.Text = "";
            tbFaildCount.Text = "";

            if (tbFilePath.Text == ""||excelContentList.Count==0) {
                MessageBox.Show("请选择受测者名单！", "系统信息");
                return;
            }
            if (cbTestItems.SelectedIndex < 0) {
                MessageBox.Show("请选择测试项目！", "系统信息");
                return;
            }
          
            double value = 0;
            UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(importProgress.SetValue);

            List<Model.TB_AthleteInfo> existsAthNameList = new List<Model.TB_AthleteInfo>();
            int errorCount = 0;
            int successCount = 0;
            bool cancleImport = false;
            for (int i = 0; i < excelContentList.Count; i++) {
                 List<string> columnList = excelContentList[i];
                
                    tbProgress.Text = (i + 1) + "/" + excelContentList.Count;

                    try
                    {
                        Model.TB_AthleteInfo newAthInfo = new Model.TB_AthleteInfo();
                        newAthInfo.Ath_TestDate = columnList[0] == "" ? DateTime.Now : DateTime.Parse(columnList[0]);
                        newAthInfo.Ath_Name = columnList[1];
                        newAthInfo.Ath_PinYin = DataUtil.PersonNamePinYinUtil.GetNamePinyin(newAthInfo.Ath_Name);
                        newAthInfo.Ath_Sex = DataUtil.AthleteSexUtil.GetSex(columnList[2]);
                        newAthInfo.Ath_Birthday = columnList[3] == "" ? DateTime.Now : DateTime.Parse(columnList[3]);
                        newAthInfo.Ath_Height = columnList[4];
                        newAthInfo.Ath_Weight = columnList[5];
                        newAthInfo.Ath_Project = columnList[6];
                        newAthInfo.Ath_MainProject = columnList[7];
                        newAthInfo.Ath_TrainYears = columnList[8];
                        newAthInfo.Ath_Level = columnList[9];
                        newAthInfo.Ath_Team = columnList[10];
                        newAthInfo.Ath_TestAddress = columnList[11];
                        newAthInfo.Ath_TestMachine = columnList[12];
                        newAthInfo.Ath_TestState = columnList[13];
                        newAthInfo.Ath_Remark = columnList[14];
                        newAthInfo.Ath_TestID = testList[cbTestItems.SelectedIndex].ID;
                        string existID = "";
                        int result = athInfoBLL.Add(newAthInfo, out existID);
                        if (result == BLL.TB_AthleteInfo.RepeatAdd)
                        {
                            existsAthNameList.Add(newAthInfo);
                        }
                        else {
                            successCount++;
                        }
                    }
                    catch
                    {
                        errorCount++;
                        if (MessageBox.Show("第" + (i + 2) + "行 " + columnList[0] + "的信息导入错误，请检查！\r\n是否继续导入？", "系统信息", MessageBoxButton.YesNo) == MessageBoxResult.No)
                        {
                            value = 0;
                            Dispatcher.Invoke(updatePbDelegate,
                     System.Windows.Threading.DispatcherPriority.Background,
                     new object[] { ProgressBar.ValueProperty, value });
                            cancleImport = true;
                            break;
                        }
                    }
                    value += 1;
                    Dispatcher.Invoke(updatePbDelegate,
                        System.Windows.Threading.DispatcherPriority.Background,
                        new object[] { ProgressBar.ValueProperty, value });
               
            }

            for (int i = 0; i < existsAthNameList.Count; i++) {
                existsAthNameList[i].Index = i + 1;
            }

            dgAthlete.ItemsSource = existsAthNameList;
            tbAllCount.Text = "总共" + excelContentList.Count + "条信息";
            tbSuccessCount.Text = "成功导入" + successCount + "条信息";
            tbFaildCount.Text = "未导入" + (existsAthNameList.Count + errorCount) + "条信息";
            if (!cancleImport)
            {
                MessageBox.Show("导入完成！", "系统信息");
            }
         
            tbFilePath.Text = "";
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

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddTestItemWindow addWindow = new AddTestItemWindow();
            addWindow.Owner = this;
            if (addWindow.ShowDialog() == true) {
                testList = testManagerBLL.GetModelList("");
                Binding b = new Binding() { Source = testList };
                cbTestItems.SetBinding(ComboBox.ItemsSourceProperty, b);
                cbTestItems.SelectedIndex = 0;
            }
        }
    }
}
