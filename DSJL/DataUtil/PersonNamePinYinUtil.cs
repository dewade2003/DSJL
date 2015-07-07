using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPinyin;

namespace DSJL.DataUtil
{
    class PersonNamePinYinUtil
    {
        public static string GetNamePinyin(string name) {
            string pinyin = Pinyin.GetPinyin(name).ToUpper();
            string[] pinyinarr = pinyin.Split(' ');
            if (pinyinarr.Length > 2)//如果名字为三个字以上，去掉空格间隔
            {
                pinyin = "";
                for (int i = 0; i < pinyinarr.Length; i++)
                {
                    pinyin += pinyinarr[i];
                    if (i == 0)
                    {
                        pinyin += " ";
                    }
                }
            }
            return pinyin;
        }
    }
}
