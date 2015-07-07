using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSJL.Model
{
    class AppPath
    {

        public static readonly string RootPath = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// AppData
        /// </summary>
        public static readonly string DataPath = RootPath + "\\AppData\\";

        public static readonly string XmlDataDirPath = RootPath + "\\AppData\\XmlData\\";
    }
}
