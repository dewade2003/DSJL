using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSJL.DataUtil
{
    /// <summary>
    /// 运动员性别处理
    /// </summary>
    class AthleteSexUtil
    {

        /// <summary>
        /// 根据输入的字符获取性别
        /// </summary>
        public static string GetSex(string str)
        {
            string sex = "男";
            switch (str.ToLower()) { 
                case "female":
                case "f":
                case "nv":
                case "n":
                case "woman":
                case "女":
                    sex = "女";
                    break;
            }
            return sex;
        }
      
    }
}
