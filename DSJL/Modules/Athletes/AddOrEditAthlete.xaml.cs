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
using NPinyin;

namespace DSJL.Modules.Athletes
{
    /// <summary>
    /// AddOrEditAthlete.xaml 的交互逻辑
    /// </summary>
    public partial class AddOrEditAthlete : Window
    {
        private bool isEdit = false;
        private Model.TB_AthleteInfo athlete=new Model.TB_AthleteInfo();
        private Model.TB_AthleteInfo oldAthInfo = new Model.TB_AthleteInfo();
        private BLL.TB_AthleteInfo athleteBLL;

        BLL.TB_TestManager testManagerBLL;
        List<Model.TB_TestManager> testList;

        //设置的测试项目，用于初始化时定位
        private Model.TB_TestManager testItem;

        public Model.TB_TestManager TestItem
        {
            set
            {
                testItem = value;
            }
        }

        private string[] fileLines=null;//导入数据时为找到测试人员时，传入数据文件信息，把信息里的人员信息赋值到 athlete

        public string[] FileLines {
            set {
                fileLines = value;
            }
        }

        private Model.TB_AthleteInfo currentAthInfo = null;//导入数据时为找到测试人员时
        public Model.TB_AthleteInfo CurrentAthInfo {
            set {
                currentAthInfo = value;
            }
        }

        public AddOrEditAthlete()
        {
            InitializeComponent();
            athleteBLL = new BLL.TB_AthleteInfo();
            testManagerBLL = new BLL.TB_TestManager();
            txtName.Focus();
        }

        public Model.TB_AthleteInfo Athlete {
            set {
                athlete = value;
                oldAthInfo.Ath_Name = value.Ath_Name;
                oldAthInfo.Ath_Sex = value.Ath_Sex;
                oldAthInfo.Ath_Birthday = value.Ath_Birthday;
                oldAthInfo.Ath_TestDate = value.Ath_TestDate;
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
          
            testList = testManagerBLL.GetModelList("");
            Binding b = new Binding() { Source = testList };
            cbTestItems.SetBinding(ComboBox.ItemsSourceProperty, b);
            List<string> addresslist = athleteBLL.GetColumnDistinctList("Ath_TestAddress");
            txtTestAddress.ItemsSource = addresslist;
            List<string> teststatelist = athleteBLL.GetColumnDistinctList("Ath_TestState");
            txtTestState.ItemsSource = teststatelist;

           
            if (isEdit)
            {
                tbTitle.Text = "编辑" + athlete.Ath_Name + "信息";
            }
            else {
                athlete.Ath_TestMachine = "Isomed2000";
                if (addresslist.Count > 0) {
                    athlete.Ath_TestAddress = addresslist[0];
                }
                if (teststatelist.Count > 0) {
                    athlete.Ath_TestState = teststatelist[0];
                }
            }
            if (currentAthInfo != null) {
                athlete.Ath_Name = currentAthInfo.Ath_Name;
                athlete.Ath_PinYin = currentAthInfo.Ath_PinYin;
                athlete.Ath_Height = currentAthInfo.Ath_Height;
                athlete.Ath_Weight = currentAthInfo.Ath_Weight;
                athlete.Ath_Birthday = currentAthInfo.Ath_Birthday == null ? DateTime.Now : currentAthInfo.Ath_Birthday;
                athlete.Ath_TestDate = currentAthInfo.Ath_TestDate;
            }

            SetBind();
        }

        //获取数据文件前32行每行的值
        private string GetLineValue(string line)
        {
            string value = line.Substring(line.IndexOf(':') + 1).Trim();
            return value;
        }

        private void SetBind() {
            if (!isEdit&&testItem!=null) {
                athlete.Ath_TestID = testItem.ID;
            }
            Binding b = new Binding() { Source = athlete };
            formGrid.SetBinding(Grid.DataContextProperty, b);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text.Trim() == "") {
                MessageBox.Show("姓名不能为空！","系统信息");
                return;
            }
            if (cbTestItems.SelectedIndex < 0) {
                MessageBox.Show("请选择测试项目！", "系统信息");
                return;
            }
            if (isEdit)
            {
                try {
                    int updateResult = -1;

                    athlete.Ath_PinYin = DataUtil.PersonNamePinYinUtil.GetNamePinyin(athlete.Ath_Name);
                    if (oldAthInfo.Ath_Name != athlete.Ath_Name || oldAthInfo.Ath_Sex != athlete.Ath_Sex || ((DateTime)oldAthInfo.Ath_Birthday).ToString("yyyy-MM-dd") != ((DateTime)athlete.Ath_Birthday).ToString("yyyy-MM-dd")||((DateTime)oldAthInfo.Ath_TestDate).ToString("yyyy-MM-dd")!=((DateTime)athlete.Ath_TestDate).ToString("yyyy-MM-dd"))
                    {
                        updateResult= athleteBLL.Update(athlete, true);
                    }
                    else {
                        updateResult= athleteBLL.Update(athlete);
                    }
                    switch (updateResult)
                    {
                        case BLL.TB_AthleteInfo.Success:
                             this.DialogResult = true;
                             this.Close();
                            break;
                        case BLL.TB_AthleteInfo.RepeatAdd:
                            MessageBox.Show("因已存在相同的人员信息，编辑失败，请确认！", "系统错误");
                            break;
                        case BLL.TB_AthleteInfo.Error:
                            MessageBox.Show("修改信息时出错，请重试!\r\n如果问题一直存在，或联系软件制作者。", "系统错误");
                            break;
                    }
              
                }
                catch (Exception ee) {
                    MessageBox.Show("编辑失败\r\n" + ee.Message, "系统错误");
                }
            }
            else {
                try
                {
                    athlete.Ath_PinYin = DataUtil.PersonNamePinYinUtil.GetNamePinyin(athlete.Ath_Name);
                    string existID = "";
                    int addResult = athleteBLL.Add(athlete,out existID);
                    switch (addResult) { 
                        case BLL.TB_AthleteInfo.Success:
                             athlete = new Model.TB_AthleteInfo();
                             SetBind();
                             this.DialogResult = true;
                             break;
                        case BLL.TB_AthleteInfo.RepeatAdd:
                             MessageBox.Show("已存在相同的人员信息，请确认！", "系统错误");
                             break;
                    }
                }
                catch (Exception ee) {
                    MessageBox.Show("添加失败\r\n" + ee.Message, "系统错误");
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
