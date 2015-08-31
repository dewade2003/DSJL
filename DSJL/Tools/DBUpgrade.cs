using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSJL.DBUtility;
using System.Collections;
using System.Windows;

namespace DSJL.Tools
{
   public  class DBUpgrade
    {
       public static bool Upgrade() {
           DbHelperOleDb dbUtil = new DbHelperOleDb();
           try
           {
               List<string> tables = dbUtil.GetTableNameList();
               if (!tables.Exists(x=>x.Equals("TB_DBVersion")))
               {
                   ArrayList sqlList = new ArrayList() {
                   "CREATE TABLE TB_DBVersion (ID AutoIncrement,versioncode varchar);",
                   "INSERT into TB_DBVersion (versioncode) VALUES (2);"
                   };
                   DBUtility.DbHelperOleDb.ExecuteSqlTran(sqlList);
               }
               if (!tables.Exists(x=>x.Equals("TB_StandardParams")))
               {
                   string createParamTableSql = "CREATE TABLE TB_StandardParams (ID AutoIncrement ,StandID int  NULL,StandName varchar  NULL,Ath_Sex varchar  NULL,Ath_AgeMinLimit int  NULL,Ath_AgeMaxLimit int  NULL,Ath_Project varchar  NULL,Ath_MainProject varchar  NULL,Joint_Side varchar  NULL,Test_Mode varchar  NULL,Joint varchar  NULL,Plane varchar  NULL,Speed1 varchar  NULL,Speed2 varchar  NULL);";

                   DBUtility.DbHelperOleDb.ExecuteSql(createParamTableSql);
               }
               return true;
           }
           catch (Exception ee)
           {
               MessageBox.Show("升级数据库出错！\r\n错误信息：" + ee.Message, "系统错误");
               return false;
           }
       }
    }
}
