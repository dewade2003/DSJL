using System;
using System.Data;
using System.Collections.Generic;
using DSJL.Model;
namespace DSJL.BLL
{
	/// <summary>
	/// TB_TestInfo
	/// </summary>
	public partial class TB_TestInfo
	{
		private readonly DSJL.DAL.TB_TestInfo dal=new DSJL.DAL.TB_TestInfo();
		public TB_TestInfo()
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
		public void Add(DSJL.Model.TB_TestInfo model)
		{
			dal.Add(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(DSJL.Model.TB_TestInfo model)
		{
			return dal.Update(model);
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

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public DSJL.Model.TB_TestInfo GetModel(int ID)
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
		public List<DSJL.Model.TB_TestInfo> GetModelList(string strWhere)
		{
			DataSet ds = dal.GetList(strWhere);
			return DataTableToList(ds.Tables[0]);
		}
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<DSJL.Model.TB_TestInfo> DataTableToList(DataTable dt)
		{
			List<DSJL.Model.TB_TestInfo> modelList = new List<DSJL.Model.TB_TestInfo>();
			int rowsCount = dt.Rows.Count;
			if (rowsCount > 0)
			{
				DSJL.Model.TB_TestInfo model;
				for (int n = 0; n < rowsCount; n++)
				{
					model = new DSJL.Model.TB_TestInfo();
					if(dt.Rows[n]["ID"].ToString()!="")
					{
						model.ID=int.Parse(dt.Rows[n]["ID"].ToString());
					}
					if(dt.Rows[n]["Ath_ID"].ToString()!="")
					{
						model.Ath_ID=int.Parse(dt.Rows[n]["Ath_ID"].ToString());
					}
					if(dt.Rows[n]["TestDate"].ToString()!="")
					{
						model.TestDate=DateTime.Parse(dt.Rows[n]["TestDate"].ToString());
					}
					if(dt.Rows[n]["TestTime"].ToString()!="")
					{
						model.TestTime=DateTime.Parse(dt.Rows[n]["TestTime"].ToString());
					}
					model.Joint_Side=dt.Rows[n]["Joint_Side"].ToString();
					model.Test_Mode=dt.Rows[n]["Test_Mode"].ToString();
					model.Joint=dt.Rows[n]["Joint"].ToString();
					model.Plane=dt.Rows[n]["Plane"].ToString();
					model.Motion_Start=dt.Rows[n]["Motion_Start"].ToString();
					model.Motion_End=dt.Rows[n]["Motion_End"].ToString();
					model.Speed1=dt.Rows[n]["Speed1"].ToString();
					model.Speed2=dt.Rows[n]["Speed2"].ToString();
					model.Acceleration1=dt.Rows[n]["Acceleration1"].ToString();
					model.Acceleration2=dt.Rows[n]["Acceleration2"].ToString();
					model.Break=dt.Rows[n]["Break"].ToString();
					model.NOOfSets=dt.Rows[n]["NOOfSets"].ToString();
					model.NOOfRepetitions=dt.Rows[n]["NOOfRepetitions"].ToString();
                    string insuredSide= dt.Rows[n]["InsuredSide"].ToString();
                    model.InsuredSide = insuredSide.Equals("") ? "2" : insuredSide;
                    string gracomp= dt.Rows[n]["Gravitycomp"].ToString();
                    model.Gravitycomp = gracomp.Equals("") ? "0" : gracomp;
					model.Therapist=dt.Rows[n]["Therapist"].ToString();
					model.BaseFileName=dt.Rows[n]["BaseFileName"].ToString();
					model.DataFileName=dt.Rows[n]["DataFileName"].ToString();
                    model.Remark = dt.Rows[n]["remark"].ToString();
                    model.ActionID = dt.Rows[n]["actionid"].ToString();
					modelList.Add(model);
				}
			}
			return modelList;
		}

        public void GetModelFromDataRow(DataRow dr,Model.TestInfoModel model) {
            if (dr["test.ID"].ToString() != "")
            {
                model.TestID = int.Parse(dr["test.ID"].ToString());
            }
            if (dr["Ath_ID"].ToString() != "")
            {
                model.Ath_ID = int.Parse(dr["Ath_ID"].ToString());
            }
            if (dr["TestDate"].ToString() != "")
            {
                model.TestDate = DateTime.Parse(dr["TestDate"].ToString());
            }
            if (dr["TestTime"].ToString() != "")
            {
                model.TestTime = DateTime.Parse(dr["TestTime"].ToString());
            }
            model.Joint_Side = dr["Joint_Side"].ToString();
            model.Test_Mode = dr["Test_Mode"].ToString();
            model.Joint = dr["Joint"].ToString();
            model.Plane = dr["Plane"].ToString();
            model.Motion_Start = dr["Motion_Start"].ToString();
            model.Motion_End = dr["Motion_End"].ToString();
            model.Speed1 = dr["Speed1"].ToString();
            model.Speed2 = dr["Speed2"].ToString();
            model.Acceleration1 = dr["Acceleration1"].ToString();
            model.Acceleration2 = dr["Acceleration2"].ToString();
            model.Break = dr["Break"].ToString();
            model.NOOfSets = dr["NOOfSets"].ToString();
            model.NOOfRepetitions = dr["NOOfRepetitions"].ToString();
            string insuredSide = dr["InsuredSide"].ToString();
            model.InsuredSide = insuredSide.Equals("") ? "2" : insuredSide;
            string gracomp = dr["Gravitycomp"].ToString();
            model.Gravitycomp = gracomp.Equals("") ? "0" : gracomp;
            model.Therapist = dr["Therapist"].ToString();
            model.BaseFileName = dr["BaseFileName"].ToString();
            model.DataFileName = dr["DataFileName"].ToString();

            model.Remark = dr["remark"].ToString();
            model.ActionID = dr["actionid"].ToString();
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

		#endregion  Method
	}
}

