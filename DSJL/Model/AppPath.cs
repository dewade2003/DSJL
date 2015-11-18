using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DSJL.Model
{
    class AppPath
    {
        public static readonly string DBName = "DSJLDB.mdb";
        public static readonly string EmptyDBName = "EmptyDSJLDB.mdb";
        public static readonly string RootPath = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// AppData
        /// </summary>
        public static readonly string DataPath = RootPath + "AppData\\";
        public static readonly string ImportStandFilePath = RootPath + "AppData\\ImportStand\\";

        public static readonly string XmlDataDirPath = RootPath + "AppData\\XmlData\\";
        public static readonly string TempDirPath = RootPath + "temp\\";

        public static string CacheRootPath {
            get {
                string cachesPath = RootPath + "Caches\\";
                CreateDirIfNotExists(cachesPath);
                return cachesPath;
            }
        }

        public static string StandardChartCachePath
        {
            get
            {
                string cachesPath = RootPath + "Caches\\StandardChart\\";
                CreateDirIfNotExists(cachesPath);
                return cachesPath;
            }
        }

        private static void CreateDirIfNotExists(string path) {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
