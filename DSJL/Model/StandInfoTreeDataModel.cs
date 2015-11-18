using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace DSJL.Model
{
    class StandInfoTreeDataModel : BaseModel
    {
        private string defaultIcon;
        private string openedIcon;
        private string icon;
        private bool isExpanded;

        private Model.TB_StandardInfo standInfo;

        private BLL.TB_StandardInfo standInfoBLL = new BLL.TB_StandardInfo();

        private ObservableCollection<StandInfoTreeDataModel> children = new ObservableCollection<StandInfoTreeDataModel>();

        /// <summary>
        /// 默认图标
        /// </summary>
        public string DefaultIcon { set { defaultIcon = Icon = value; } }
        /// <summary>
        /// 打开后的图表
        /// </summary>
        public string OpenedIcon { set { openedIcon = value; } }

        public string Icon
        {
            get { return icon; }
            set { icon = value;
                //NotifyPropertyChanged("Icon");
            }
        }

        /// <summary>
        /// 是否打开
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }
            set
            {
                isExpanded = value;
                if (isExpanded)
                {
                    Icon = openedIcon;
                }
                else {
                    Icon = defaultIcon;
                }
                //NotifyPropertyChanged("IsExpended");
            }
        }

        private StandInfoTreeDataModel parentModel;//注释
        /// <summary>
        /// 注释
        /// </summary>
        public StandInfoTreeDataModel ParentModel
        {
            get { return parentModel; }
            set { parentModel = value;
                //NotifyPropertyChanged("ParentModel");
            }
        }
        

        /// <summary>
        /// 
        /// 参考值Model
        /// </summary>
        public Model.TB_StandardInfo StandInfo
        {
            get { return standInfo; }
            set { standInfo = value;
                //NotifyPropertyChanged("StandInfo");
            }
        }

        public ObservableCollection<StandInfoTreeDataModel> Children {
            get {
                //RefrenshChildren();
                return children;
            }
            set {
                children = value;
                NotifyPropertyChanged("Children");
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void RefrenshChildren()
        {
            children.Clear();
            List<Model.TB_StandardInfo> standInfoModelList;
            switch (standInfo.Stand_Level)
            {
                case -1:
                    standInfoModelList = standInfoBLL.GetModelList("stand_level=1");
                    foreach (Model.TB_StandardInfo standInfoModel in standInfoModelList)
                    {
                        StandInfoTreeDataModel treeModel = new StandInfoTreeDataModel();
                        treeModel.parentModel = this;
                        treeModel.StandInfo = standInfoModel;
                        treeModel.DefaultIcon = "/DSJL;component/Assets/Images/folder.png";
                        treeModel.OpenedIcon = "/DSJL;component/Assets/Images/folder_opened.png";
                        children.Add(treeModel);
                    }
                    break;
                case 1:
                    standInfoModelList = standInfoBLL.GetModelList("stand_level=2 and Stand_ParentID=" + standInfo.ID);
                    foreach (Model.TB_StandardInfo standInfoModel in standInfoModelList)
                    {
                        StandInfoTreeDataModel treeModel = new StandInfoTreeDataModel();
                        treeModel.parentModel = this;
                        treeModel.StandInfo = standInfoModel;
                        treeModel.DefaultIcon = "/DSJL;component/Assets/Images/file.png";
                        treeModel.OpenedIcon = "/DSJL;component/Assets/Images/file.png";
                        children.Add(treeModel);
                    }
                    break;
            }
        }
      
    }
}
