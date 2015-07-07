﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace DSJL.ExcelDao
{
    public class ExcelUtil
    {
        private static ApplicationClass excelApp;
        private static Worksheet mySheet1;//工作簿1
        private static object missing = Missing.Value;

        public static List<List<string>> Read(string fileName,int columnCount) {
            List<List<string>> textList = new List<List<string>>();
            excelApp = new ApplicationClass();
            Workbooks myWorkBooks = excelApp.Workbooks;
            myWorkBooks.Open(fileName, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing);
            Sheets sheets = excelApp.Sheets;
            mySheet1 = (Worksheet)sheets[1];
            int rowCount = mySheet1.UsedRange.Rows.Count;
            if (rowCount != 0)
            {
                for (int i = 3; i <= rowCount; i++)
                {
                    string name = ((Range)mySheet1.Cells[i, 2]).Text.ToString();
                 
                    if (name != "") {
                        List<string> list = new List<string>();
                        for (int j = 0; j < columnCount; j++)
                        {
                            list.Add(((Range)mySheet1.Cells[i, j + 1]).Text.ToString());
                        }
                        textList.Add(list);
                    }
                }
            }
            myWorkBooks.Close();
            excelApp.Quit();
            excelApp = null;
            return textList;
        }

        /// <summary>
        /// 创建excel文件并写入内容
        /// <param name="contents">内容</param>
        /// </summary>
        //public static bool CreateExcel(string[,] contents)
        //{
        //    bool result = true;

        //    excelApp = new ApplicationClass();
        //    excelApp.Application.Workbooks.Add(true);
        //    Workbook book = (Microsoft.Office.Interop.Excel.Workbook)excelApp.ActiveWorkbook;
        //    mySheet1 = (Microsoft.Office.Interop.Excel.Worksheet)book.ActiveSheet;

        //    return result;
        //}
      
    }
}
