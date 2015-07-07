using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSJL.Model;

namespace DSJL.MonitorSetting
{
    class MonitorDirModel:BaseModel
    {
        public string DirName
        {
            get;
            set;
        }

        public string DirPath
        {
            get;
            set;
        }

        public string DirAddDate
        {
            get;
            set;
        }
    }
}
