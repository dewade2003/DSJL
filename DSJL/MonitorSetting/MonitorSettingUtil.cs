using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace DSJL.MonitorSetting
{
    class MonitorSettingUtil
    {
        static readonly string settingFilePath = "MonitorSetting\\MonitorSettings.xml";
        private string settingFileFullPath = "";

        XDocument settingDoc;
        bool isOpenMonitor = true;
        List<MonitorDirModel> modelList=new List<MonitorDirModel>();

        public MonitorSettingUtil() {
            settingFileFullPath = AppDomain.CurrentDomain.BaseDirectory + settingFilePath;
            if (File.Exists(settingFileFullPath))
            {
                try
                {
                    LoadSetting();
                }
                catch
                {
                    isOpenMonitor = false;
                    throw new Exception("配置文件加载失败！");
                }
            }
        }

        private void LoadSetting() {
            settingDoc = XDocument.Load(settingFileFullPath);
            XElement settingEle = settingDoc.Root.Element("MonitorDirSetting");
            isOpenMonitor = bool.Parse(settingEle.Attribute("IsOpenMonitor").Value);

            modelList.Clear();
            XElement dirsEle = settingDoc.Root.Element("dirs");
            IEnumerable<XElement> dirListEle = dirsEle.Elements();
            int count=dirListEle.Count();
            for (int i = 0; i < count; i++)
            {
                XElement ele = dirListEle.ElementAt(i);
                MonitorDirModel dirModel = new MonitorDirModel();
                dirModel.Index = i + 1;
                dirModel.IsChecked = false;

                dirModel.DirAddDate = ele.Attribute("diradddate").Value;
                dirModel.DirName = ele.Attribute("dirname").Value;
                dirModel.DirPath = ele.Attribute("dirpath").Value;

                modelList.Add(dirModel);
            }
        }

        public bool IsOpenMonitor {
            get {
                return isOpenMonitor;
            }
            set {
                isOpenMonitor = value;

                XElement settingEle = settingDoc.Root.Element("MonitorDirSetting");
                settingEle.SetAttributeValue("IsOpenMonitor", value);
                settingDoc.Save(settingFileFullPath);
            }
        }

        public List<MonitorDirModel> DirModelList {
            get {
                return modelList;
            }
        }

        public bool AddDir(string path) {
            bool addResult = true;
            try
            {
                string dirName = path.Substring(path.LastIndexOf("\\") + 1);

                XElement ele = new XElement("dir");
                ele.SetAttributeValue("dirname", dirName);
                ele.SetAttributeValue("dirpath", path);
                ele.SetAttributeValue("diradddate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                XElement dirsEle = settingDoc.Root.Element("dirs");
                dirsEle.Add(ele);
                settingDoc.Save(settingFileFullPath);
                LoadSetting();
            }
            catch {
                addResult = false;
            }
            return addResult;
        }

        public bool DeleteDir(int index) {
            bool deleteResult = true;
            try
            {
                XElement dirsEle = settingDoc.Root.Element("dirs");
                XElement ele = dirsEle.Elements().ElementAt(index - 1);
                ele.Remove();

                settingDoc.Save(settingFileFullPath);
                LoadSetting();
            }
            catch {
                deleteResult = false;
            }
            return deleteResult;
        }

        public bool DeleteCheckedDir(int[] indexs) {
            bool deleteResult = true;
            try
            {
                XElement dirsEle = settingDoc.Root.Element("dirs");
                List<XElement> eleList = new List<XElement>();
                foreach (int index in indexs)
                {
                    XElement ele = dirsEle.Elements().ElementAt(index - 1);
                    eleList.Add(ele);
                }
                foreach (XElement ele in eleList) {
                    ele.Remove();
                }
                settingDoc.Save(settingFileFullPath);
                LoadSetting();
            }
            catch {
                deleteResult = false;
            }
            return deleteResult;
        }

        public bool DeleteAllDir() {
            bool deleteResult = true;

            try
            {
                XElement dirsEle = settingDoc.Root.Element("dirs");
                while (dirsEle.Elements().Count() != 0) {
                    dirsEle.Elements().ElementAt(0).Remove();
                }
                settingDoc.Save(settingFileFullPath);
                LoadSetting();
            }
            catch {
                deleteResult = false;
            }

          
            return deleteResult;
        }
    }
}
