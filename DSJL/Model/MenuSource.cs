using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace DSJL
{
    internal class MenuSource
    {

        public string IconSource
        {
            get;
            set;
        }

        public string MenuName
        {
            get;
            set;
        }

        public MenuAction Action
        {
            get;
            set;
        }

        public Page ActionPage
        {
            get;
            set;
        }
    }

    enum MenuAction {
        /// <summary>
        /// 人员管理
        /// </summary>
        AthleteManager,
        /// <summary>
        /// 测试结果
        /// </summary>
        TestInfo,
        /// <summary>
        /// 数据库管理
        /// </summary>
        DBManager,
        /// <summary>
        /// 设置
        /// </summary>
        Setup,
        /// <summary>
        /// 统计 
        /// </summary>
        Statics,
        /// <summary>
        /// 帮助
        /// </summary>
        Help
    }
}
