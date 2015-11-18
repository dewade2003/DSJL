using DSJL.Caches.Dict;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DSJL.Caches.Util
{
    class AthTestInfoModelUtil
    {
        public static List<Model.TestInfoModel> AthTestUtil(DataSet ds) {
            List<Model.TestInfoModel> testInfoModelList = new List<Model.TestInfoModel>();
             BLL.TB_TestInfo testInfoBLL = new BLL.TB_TestInfo();
             BLL.TB_AthleteInfo athleteInfoBLL = new BLL.TB_AthleteInfo();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];
                Model.TestInfoModel testInfoModel = new Model.TestInfoModel();
                testInfoModel.Index = i + 1;

                testInfoBLL.GetModelFromDataRow(dr, testInfoModel);
                athleteInfoBLL.GetAthleteInfoFromDataRow(dr, testInfoModel);

                testInfoModel.DGravitycomp = DictCache.GetDictValue(DictCache.Gravitycomp, testInfoModel.Gravitycomp);
                testInfoModel.DInsuredSide = DictCache.GetDictValue(DictCache.InsuredSide, testInfoModel.InsuredSide);
                testInfoModel.DJoint = DictCache.GetDictValue(DictCache.Joint, testInfoModel.Joint);
                testInfoModel.DJointSide = DictCache.GetDictValue(DictCache.JointSide, testInfoModel.Joint_Side);
                testInfoModel.DTestMode = DictCache.GetDictValue(DictCache.TestMode, testInfoModel.Test_Mode);

                testInfoModel.DPlane = DictCache.GetDict(testInfoModel.Joint, testInfoModel.Plane, testInfoModel.Test_Mode).Dict_Value;

                testInfoModelList.Add(testInfoModel);
            }
            return testInfoModelList;
        }
    }
}
