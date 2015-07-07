using System;
using System.Data;
using System.Collections.Generic;
using DSJL.Model;
namespace DSJL.BLL
{
	/// <summary>
	/// TB_AthleteInfo
	/// </summary>
	public partial class TB_AthleteInfo : AbsBLL
	{
		private readonly DSJL.DAL.TB_AthleteInfo dal=new DSJL.DAL.TB_AthleteInfo();
		public TB_AthleteInfo()
		{}
		#region  Method

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
			return dal.GetMaxId();
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int ID)
		{
			return dal.Exists(ID);
		}

		/// <summary>
		/// 增加一条数据
		/// </summary>
		public int Add(DSJL.Model.TB_AthleteInfo model,out string existID)
		{
            existID = "";
            //1.验证是否是重复导入，字段 姓名，性别，出生日期，测试日期
            DataSet ds = dal.GetList("ath_name='" + model.Ath_Name + "' and ath_sex='" + model.Ath_Sex + "' and DateDiff('d',ath_birthday,#" + ((DateTime)model.Ath_Birthday).ToString("yyyy-MM-dd") + "#)=0 and DateDiff('d',ath_testdate,#" + ((DateTime)model.Ath_TestDate).ToString("yyyy-MM-dd") + "#)=0");
            if (ds.Tables[0].Rows.Count > 0) {
                existID = ds.Tables[0].Rows[0]["ID"].ToString();
                return RepeatAdd;
            }
            //2.验证是否有此人信息，字段：姓名，性别，出生日期
            ds = dal.GetList("ath_name='" + model.Ath_Name + "' and ath_sex='" + model.Ath_Sex + "' and DateDiff('d',ath_birthday,#" + ((DateTime)model.Ath_Birthday).ToString("yyyy-MM-dd") + "#)=0");
            if (ds.Tables[0].Rows.Count > 0)
            {
                model.Ath_Code = ds.Tables[0].Rows[0]["ath_code"].ToString();
            }
            else {
                ds = dal.GetList("ath_testid=" + model.Ath_TestID,"");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    string maxCode = ds.Tables[0].Rows[0]["ath_code"].ToString();
                    int code = int.Parse(maxCode.Substring(maxCode.Length - 5));
                    model.Ath_Code = DateTime.Now.ToString("yyyyMM") + model.Ath_TestID.ToString("00") + (code + 1).ToString("00000");
                }
                else {
                    model.Ath_Code = DateTime.Now.ToString("yyyyMM") + model.Ath_TestID.ToString("00") + "00001";
                }
            }
            try
            {
                dal.Add(model);
            }
            catch(Exception ee) {
                throw ee;
            }
            return Success;
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public int Update(DSJL.Model.TB_AthleteInfo model)
        {
            bool updateResult=dal.Update(model);
            if (updateResult)
            {
                return Success;
            }
            else {
                return Error;
            }
		}

        public int Update(DSJL.Model.TB_AthleteInfo model, bool isChangeKeyInformation) {
            if (!isChangeKeyInformation)
            {
                return Update(model);
            }
            else {
                DataSet ds = dal.GetList("ath_name='" + model.Ath_Name + "' and ath_sex='" + model.Ath_Sex + "' and DateDiff('d',ath_birthday,#" + ((DateTime)model.Ath_Birthday).ToString("yyyy-MM-dd") + "#)=0 and DateDiff('d',ath_testdate,#" + ((DateTime)model.Ath_TestDate).ToString("yyyy-MM-dd") + "#)=0");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    return RepeatAdd;
                }

                //2.验证是否有此人信息，字段：姓名，性别，出生日期
                ds = dal.GetList("ath_name='" + model.Ath_Name + "' and ath_sex='" + model.Ath_Sex + "' and DateDiff('d',ath_birthday,#" + ((DateTime)model.Ath_Birthday).ToString("yyyy-MM-dd") + "#)=0");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    model.Ath_Code = ds.Tables[0].Rows[0]["ath_code"].ToString();
                }
                else
                {
                    ds = dal.GetList("ath_testid=" + model.Ath_TestID);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        string maxCode = ds.Tables[0].Rows[0]["ath_code"].ToString();
                        int code = int.Parse(maxCode.Substring(maxCode.Length - 5));
                        model.Ath_Code = DateTime.Now.ToString("yyyyMM") + model.Ath_TestID.ToString("00") + (code + 1).ToString("00000");
                    }
                    else
                    {
                        model.Ath_Code = DateTime.Now.ToString("yyyyMM") + model.Ath_TestID.ToString("00") + "00001";
                    }
                }
                bool updateResult = dal.Update(model);
                if (updateResult)
                {
                    return Success;
                }
                else
                {
                    return Error;
                }
            }
        }
		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(int ID)
		{
			
			return dal.Delete(ID);
		}
		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool DeleteList(string IDlist )
		{
			return dal.DeleteList(IDlist );
		}

        public bool HiddenData(string ids,bool isHidden) {
            if (isHidden)
            {
                return dal.HiddenData(ids, 1);
            }
            else {
                return dal.HiddenData(ids, 0);
            }
        }

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public DSJL.Model.TB_AthleteInfo GetModel(int ID)
		{
			
			return dal.GetModel(ID);
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetList(string strWhere)
		{
			return dal.GetList(strWhere);
		}
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<DSJL.Model.TB_AthleteInfo> GetModelList(string strWhere)
		{
			DataSet ds = dal.GetList(strWhere);
			return DataTableToList(ds.Tables[0],false);
		}

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<DSJL.Model.TB_AthleteInfo> GetModelList(string strWhere,string orderby)
        {
            DataSet ds = dal.GetList(strWhere,orderby);
            return DataTableToList(ds.Tables[0], false);
        }

        /// <summary>
        /// 获得所有数据列表
        /// </summary>
        public List<DSJL.Model.TB_AthleteInfo> GetAllModelList(string strWhere)
        {
            DataSet ds = dal.GetList(strWhere);
            return DataTableToList(ds.Tables[0],true);
        }

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<DSJL.Model.TB_AthleteInfo> DataTableToList(DataTable dt,bool isShowAll)
		{
			List<DSJL.Model.TB_AthleteInfo> modelList = new List<DSJL.Model.TB_AthleteInfo>();
			int rowsCount = dt.Rows.Count;
			if (rowsCount > 0)
			{
				DSJL.Model.TB_AthleteInfo model;
				for (int n = 0; n < rowsCount; n++)
				{
					model = new DSJL.Model.TB_AthleteInfo();
                    model.Index = n + 1;
					if(dt.Rows[n]["ID"].ToString()!="")
					{
						model.ID=int.Parse(dt.Rows[n]["ID"].ToString());
					}
					model.Ath_Code=dt.Rows[n]["Ath_Code"].ToString();
					model.Ath_Name=dt.Rows[n]["Ath_Name"].ToString();
					model.Ath_PinYin=dt.Rows[n]["Ath_PinYin"].ToString();
					model.Ath_Sex=dt.Rows[n]["Ath_Sex"].ToString();
					if(dt.Rows[n]["Ath_Birthday"].ToString()!="")
					{
						model.Ath_Birthday=DateTime.Parse(dt.Rows[n]["Ath_Birthday"].ToString());
					}
					model.Ath_Height=dt.Rows[n]["Ath_Height"].ToString();
					model.Ath_Weight=dt.Rows[n]["Ath_Weight"].ToString();
					model.Ath_Project=dt.Rows[n]["Ath_Project"].ToString();
					model.Ath_MainProject=dt.Rows[n]["Ath_MainProject"].ToString();
					model.Ath_TrainYears=dt.Rows[n]["Ath_TrainYears"].ToString();
					model.Ath_Level=dt.Rows[n]["Ath_Level"].ToString();
					model.Ath_Team=dt.Rows[n]["Ath_Team"].ToString();
					if(dt.Rows[n]["Ath_TestDate"].ToString()!="")
					{
						model.Ath_TestDate=DateTime.Parse(dt.Rows[n]["Ath_TestDate"].ToString());
					}
					model.Ath_TestAddress=dt.Rows[n]["Ath_TestAddress"].ToString();
					model.Ath_TestMachine=dt.Rows[n]["Ath_TestMachine"].ToString();
					model.Ath_TestState=dt.Rows[n]["Ath_TestState"].ToString();
					model.Ath_Remark=dt.Rows[n]["Ath_Remark"].ToString();
					if(dt.Rows[n]["Ath_TestID"].ToString()!="")
					{
						model.Ath_TestID=int.Parse(dt.Rows[n]["Ath_TestID"].ToString());
					}
                    
                    if (!isShowAll) {
                        model.Hidden = dt.Rows[n]["hidden"].ToString();

                        if (model.Hidden != "0" && !TB_Setting.IsShowAllData)
                        {
                            model.Ath_Name = "";
                            model.Ath_Birthday = null;
                            model.Ath_Weight = "";
                            model.Ath_Height = "";
                        }
                    }
                   
					modelList.Add(model);
				}
			}
			return modelList;
		}

        public void GetAthleteInfoFromDataRow(DataRow dr,Model.TestInfoModel model) {
            if (dr["ath.ID"].ToString() != "")
            {
                model.AthID = int.Parse(dr["ath.ID"].ToString());
            }
            model.Ath_Code = dr["Ath_Code"].ToString();
            model.Ath_Name = dr["Ath_Name"].ToString();
            model.Ath_PinYin = dr["Ath_PinYin"].ToString();
            model.Ath_Sex = dr["Ath_Sex"].ToString();
            if (dr["Ath_Birthday"].ToString() != "")
            {
                model.Ath_Birthday = DateTime.Parse(dr["Ath_Birthday"].ToString());
            }
            model.Ath_Height = dr["Ath_Height"].ToString();
            model.Ath_Weight = dr["Ath_Weight"].ToString();
            model.Ath_Project = dr["Ath_Project"].ToString();
            model.Ath_MainProject = dr["Ath_MainProject"].ToString();
            model.Ath_TrainYears = dr["Ath_TrainYears"].ToString();
            model.Ath_Level = dr["Ath_Level"].ToString();
            model.Ath_Team = dr["Ath_Team"].ToString();
            if (dr["Ath_TestDate"].ToString() != "")
            {
                model.Ath_TestDate = DateTime.Parse(dr["Ath_TestDate"].ToString());
            }
            model.Ath_TestAddress = dr["Ath_TestAddress"].ToString();
            model.Ath_TestMachine = dr["Ath_TestMachine"].ToString();
            model.Ath_TestState = dr["Ath_TestState"].ToString();
            model.Ath_Remark = dr["Ath_Remark"].ToString();
            if (dr["Ath_TestID"].ToString() != "")
            {
                model.Ath_TestID = int.Parse(dr["Ath_TestID"].ToString());
            }
            if (dr["hidden"].ToString() != "0" && !TB_Setting.IsShowAllData) {
                model.Ath_Name = "";
            }
           
        }

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetAllList()
		{
			return GetList("");
		}

		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		//public DataSet GetList(int PageSize,int PageIndex,string strWhere)
		//{
			//return dal.GetList(PageSize,PageIndex,strWhere);
		//}

        public List<string> GetColumnDistinctList(string columnName) {
            List<string> list=new List<string>();
            DataTable dt=dal.GetColumnDistinctList(columnName).Tables[0];
            int rowCount = dt.Rows.Count;
            for (int i = 0; i < rowCount; i++) {
                DataRow dr = dt.Rows[i];
                string value = dr[columnName].ToString();
                if (value != "") {
                    list.Add(value);
                }
            }
            return list;
        }

		#endregion  Method
    }
}

