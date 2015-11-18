using DSJL.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DSJL.Caches.Dict;

namespace DSJL.Caches
{
   static  class TestInfoModelCache
    {
       static  Dictionary<int, List<Model.TestInfoModel>> testInfoModelListDict = new Dictionary<int, List<Model.TestInfoModel>>();

        public static List<Model.TestInfoModel> GetTestInfoModelListByTestID(int testID,TestInfoModelParams param) {
            List<Model.TestInfoModel> testInfoModelList = new List<Model.TestInfoModel>();
            if (testInfoModelListDict.ContainsKey(testID))
            {
                testInfoModelList=testInfoModelListDict[testID];
            }
            else {
                testInfoModelList = QueryTestInfo(testID);
                testInfoModelListDict.Add(testID, testInfoModelList);
            }
            if (param!=null)
            {
                List<Model.TestInfoModel> checkedList=testInfoModelList;
                if (param.athName!=null)
                {
                    checkedList = checkedList.FindAll(x => x.Ath_Name.Contains(param.athName.ToLower()) || x.Ath_Name.Contains(param.athName.ToUpper()));
                }
                if (param.athCode!=null)
                {
                    checkedList = checkedList.FindAll(x => x.Ath_Code == param.athCode);
                }
                if (param.testJoint!=null)
                {
                    checkedList = checkedList.FindAll(x => x.Joint == param.testJoint);
                }
                if (param.testJointSide!=null)
                {
                    checkedList = checkedList.FindAll(x => x.Joint_Side == param.testJointSide);
                }
                if (param.testMode!=null)
                {
                    checkedList = checkedList.FindAll(x => x.Test_Mode == param.testMode);
                }
                testInfoModelList =(List<Model.TestInfoModel>) checkedList;
            }

            return testInfoModelList;
        }

        public static void Refrensh(int testID) {
            if (testInfoModelListDict.ContainsKey(testID))
            {
                testInfoModelListDict[testID] = QueryTestInfo(testID);
            }
            else {
                testInfoModelListDict.Add(testID, QueryTestInfo(testID));
            }
        }

        public static List<Model.TestInfoModel> GetCheckedModelList() {
            List<Model.TestInfoModel> checkedModelList = new List<Model.TestInfoModel>();
            foreach (var item in testInfoModelListDict.Values)
            {
                checkedModelList.AddRange(item.Where(x => x.IsChecked == true));
            }
            return checkedModelList;
        }

        public static void SetUncheck() {
            List<Model.TestInfoModel> checkedModelList = GetCheckedModelList();
            foreach (var item in checkedModelList)
            {
                item.IsChecked = false;
            }
        }

        public static void DeleteCache(int testID) {
            if (testInfoModelListDict.ContainsKey(testID))
            {
                testInfoModelListDict.Remove(testID);
            }
         
        }

        private static List<Model.TestInfoModel> QueryTestInfo(int testID) {
            List<Model.TestInfoModel> testInfoModelList = new List<Model.TestInfoModel>();
            //string sql = "select ath.*,test.*,"
            //    + "(select dict_value from tb_dict where dict_groupid=3 and dict_key=test.joint_side) as djointside,"
            //    + "(select dict_value from tb_dict where dict_groupid=1 and dict_key=test.test_mode) as dtestmode,"
            //    + "(select dict_value from tb_dict where dict_groupid=2 and dict_key=test.joint) as djoint,"
            //    + "(select dict_value from tb_dict where dict_groupid=(select id from tb_dict where dict_groupid=2 and dict_key=test.joint) and dict_key=test.plane and instr(dict_groupid2,test.test_mode)>0) as dplane,"
            //    + "(select dict_value from tb_dict where dict_groupid=4 and dict_key=test.InsuredSide) as dInsuredSide,"
            //    + "(select dict_value from tb_dict where dict_groupid=5 and dict_key=test.Gravitycomp) as dGravitycomp "
            //    + "from tb_athleteinfo as ath inner join tb_testinfo as test on ath.id=test.ath_id "
            //    +" where ath.ath_testid="+testID.ToString();

           string  sql = "select ath.*,test.* from tb_athleteinfo as ath inner join tb_testinfo as test on ath.id=test.ath_id where ath.ath_testid="+testID.ToString();
            DataSet ds = DbHelperOleDb.Query(sql);

            return Util.AthTestInfoModelUtil.AthTestUtil(ds);
        } 
    }
}
