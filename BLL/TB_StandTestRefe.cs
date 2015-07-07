using System;
using System.Data;
using System.Collections.Generic;
using DSJL.Model;
namespace DSJL.BLL
{
	/// <summary>
	/// TB_StandTestRefe
	/// </summary>
	public partial class TB_StandTestRefe
	{
		private readonly DSJL.DAL.TB_StandTestRefe dal=new DSJL.DAL.TB_StandTestRefe();
        private TB_TestInfo testInfoBLL = new TB_TestInfo();
        private TB_AthleteInfo athleteInfoBLL = new TB_AthleteInfo();
		public TB_StandTestRefe()
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

        public bool Exists(int testID, int standID) {
            string sql = "select id from tb_standtestrefe where testid=" + testID + " and standid=" + standID;
            return dal.Exists(sql);
        }

		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(DSJL.Model.TB_StandTestRefe model)
		{
			dal.Add(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(DSJL.Model.TB_StandTestRefe model)
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

        public bool Delete(int testInfoID, int StandID) {
            return dal.Delete(testInfoID, StandID);
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
		public DSJL.Model.TB_StandTestRefe GetModel(int ID)
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
		public List<DSJL.Model.TB_StandTestRefe> GetModelList(string strWhere)
		{
			DataSet ds = dal.GetList(strWhere);
			return DataTableToList(ds.Tables[0]);
		}
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<DSJL.Model.TB_StandTestRefe> DataTableToList(DataTable dt)
		{
			List<DSJL.Model.TB_StandTestRefe> modelList = new List<DSJL.Model.TB_StandTestRefe>();
			int rowsCount = dt.Rows.Count;
			if (rowsCount > 0)
			{
				DSJL.Model.TB_StandTestRefe model;
				for (int n = 0; n < rowsCount; n++)
				{
					model = new DSJL.Model.TB_StandTestRefe();
					if(dt.Rows[n]["ID"].ToString()!="")
					{
						model.ID=int.Parse(dt.Rows[n]["ID"].ToString());
					}
					if(dt.Rows[n]["StandID"].ToString()!="")
					{
						model.StandID=int.Parse(dt.Rows[n]["StandID"].ToString());
					}
					if(dt.Rows[n]["TestID"].ToString()!="")
					{
						model.TestID=int.Parse(dt.Rows[n]["TestID"].ToString());
					}
					modelList.Add(model);
				}
			}
			return modelList;
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

        public List<Model.TestInfoModel> GetStandTestInfoModelList(int standID) {
            List<Model.TestInfoModel> modelList = new List<TestInfoModel>();
            DataSet ds = dal.GetStandTestInfo(standID);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];
                Model.TestInfoModel testInfoModel = new Model.TestInfoModel();
                testInfoModel.Index = i + 1;
                testInfoModel.DGravitycomp = dr["dGravitycomp"].ToString();
                testInfoModel.DInsuredSide = dr["dInsuredSide"].ToString();
                testInfoModel.DJoint = dr["djoint"].ToString();
                testInfoModel.DJointSide = dr["djointside"].ToString();
                testInfoModel.DPlane = dr["dplane"].ToString();
                testInfoModel.DTestMode = dr["dtestmode"].ToString();
                testInfoBLL.GetModelFromDataRow(dr, testInfoModel);
                athleteInfoBLL.GetAthleteInfoFromDataRow(dr, testInfoModel);
                modelList.Add(testInfoModel);
            }
            return modelList;
        }
	}
}

