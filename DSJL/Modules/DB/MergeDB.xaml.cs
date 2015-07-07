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
using DSJL.Model;
using CYLibrary.Archiver;
using System.Threading;
using DSJL.DBUtility;
using System.IO;

namespace DSJL.Modules.DB
{
    /// <summary>
    /// MergeDB.xaml 的交互逻辑
    /// </summary>
    public partial class MergeDB : Window
    {
        private delegate void RefrenshUIDelegate(double done, string state);
        private RefrenshUIDelegate rui;

        private string tempExtrctorPath = AppPath.RootPath + "\\appdatatemp\\";

        public MergeDB()
        {
            InitializeComponent();
            rui = new RefrenshUIDelegate(RefrenshProgress);
        }

        public string ArchiveFile
        {
            get;
            set;
        }

        private void archiver_ArchiveEvent(ArchiveEventArgs e)
        {
            this.Dispatcher.Invoke(rui, e.CurrentPercent, e.CurrentArchiveFile);
        }

        private void RefrenshProgress(double done, string state)
        {
            if (done > 0) {
                pbExtract.Value = done;
            }
           
            tbState.Text = state;
            switch (done.ToString())
            {
                case "-1":
                    Thread t = new Thread(new ThreadStart(Merge));
                    t.Start();
                    break;
                case "-2":
                    MessageBox.Show("数据合并完成！","系统信息");
                    this.Close();
                    break;
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (Directory.Exists(tempExtrctorPath)) {
                Directory.Delete(tempExtrctorPath,true);
            }
            Thread t = new Thread(new ThreadStart(Archive));
            t.Start();
        }

        private void Archive()
        {
            Archiver archiver = new Archiver();
            archiver.ArchivePwd = "CISS";
            archiver.ArchiveEvent += new Archiver.ArchiveEventHandle(archiver_ArchiveEvent);
            archiver.UnArchive(ArchiveFile, tempExtrctorPath);
        }

        private void Merge()
        {
            DbHelperOleDb.SetDBPath(tempExtrctorPath + "DSJLDB.mdb");

            DSJL.BLL.TB_AthleteInfo athBLL = new BLL.TB_AthleteInfo();
            DSJL.BLL.TB_StandardInfo standBLL = new BLL.TB_StandardInfo();
            DSJL.BLL.TB_StandTestRefe refeBLL = new BLL.TB_StandTestRefe();
            DSJL.BLL.TB_TestInfo testInfoBLL = new BLL.TB_TestInfo();
            DSJL.BLL.TB_TestManager testManagerBLL = new BLL.TB_TestManager();

            List<Model.TB_AthleteInfo> athList = athBLL.GetModelList("");
            List<Model.TB_StandardInfo> standList = standBLL.GetModelList("");
            List<Model.TB_StandTestRefe> refeList = refeBLL.GetModelList("");
            List<Model.TB_TestInfo> testInfoList = testInfoBLL.GetModelList("");
            List<Model.TB_TestManager> testManagerList = testManagerBLL.GetModelList("");

            DbHelperOleDb.SetDefaultDBPath();

            Dictionary<int, int> testManagerDict = new Dictionary<int, int>();
            Dictionary<int, int> testInfoDict = new Dictionary<int, int>();
            Dictionary<int, int> standDict = new Dictionary<int, int>();
            Dictionary<int, int> athDict = new Dictionary<int, int>();

            for (int i = 0; i < testManagerList.Count; i++)
            {
                Model.TB_TestManager managerModel = testManagerList[i];
                if (DbHelperOleDb.Exists("TB_TestManager", "TestName", managerModel.TestName))
                {
                    managerModel.TestName = managerModel.TestName + "(1)";
                }
                testManagerBLL.Add(managerModel);
                int newID = testManagerBLL.GetMaxId();
                testManagerDict.Add(managerModel.ID, newID);
                UpdateMergeState(Percent(i+1,testManagerList.Count), "正在导入测试项目信息...");
            }

            for (int i = 0; i < standList.Count; i++) {
                Model.TB_StandardInfo standModel = standList[i];
                if (DbHelperOleDb.Exists("Tb_StandardInfo", "Stand_Name", standModel.Stand_Name)) {
                    standModel.Stand_Name = standModel.Stand_Name + "(1)";
                }
                standBLL.Add(standModel);
                int newID = standBLL.GetMaxId();
                standDict.Add(standModel.ID, newID);
                UpdateMergeState(Percent(i + 1, standList.Count), "正在导入测试参考值信息...");
            }

            for (int i = 0; i < athList.Count; i++) {
                Model.TB_AthleteInfo athModel = athList[i];
                athModel.Ath_TestID = testManagerDict[athModel.Ath_TestID];
                string existID = "";
                
                int addResult = athBLL.Add(athModel,out existID);
                int newID=0;
                switch (addResult) { 
                    case BLL.TB_AthleteInfo.RepeatAdd:
                        newID = int.Parse(existID);
                        break;
                    case BLL.TB_AthleteInfo.Success:
                        newID = athBLL.GetMaxId();
                        break;
                }
                athDict.Add(athModel.ID, newID);
                UpdateMergeState(Percent(i + 1, athList.Count), "正在导入受测者信息...");
            }

            for (int i = 0; i < testInfoList.Count; i++) {
                Model.TB_TestInfo testInfoModel = testInfoList[i];
                testInfoModel.Ath_ID = athDict[testInfoModel.Ath_ID];
                testInfoBLL.Add(testInfoModel);
                int newID = testInfoBLL.GetMaxId();

                string dataFileFullName = AppPath.XmlDataDirPath + testInfoModel.DataFileName;
                string oldFileName = testInfoModel.DataFileName;
                if (File.Exists(dataFileFullName))
                {
                    testInfoModel.DataFileName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xml"; 
                }
                File.Copy(tempExtrctorPath + oldFileName, AppPath.XmlDataDirPath + testInfoModel.DataFileName);

                testInfoDict.Add(testInfoModel.ID, newID);
                UpdateMergeState(Percent(i + 1, testInfoList.Count), "正在导入测试信息...");
            }

            for (int i = 0; i < refeList.Count; i++) {
                Model.TB_StandTestRefe refeModel = refeList[i];
                refeModel.StandID = standDict[refeModel.StandID];
                refeModel.TestID = testInfoDict[refeModel.TestID];
                refeBLL.Add(refeModel);
                UpdateMergeState(Percent(i + 1, refeList.Count), "正在重设数据关系...");
            }

            Directory.Delete(tempExtrctorPath, true);

            UpdateMergeState(-2, "数据合并完成");
        }

        private double Percent(int currentCount, int allCount) {
            double p = 0;
            double c = currentCount;
            p = c / allCount * 10;
            return p;
        }

        private void UpdateMergeState(double percent, string state)
        {
            this.Dispatcher.Invoke(rui, percent, state);
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
