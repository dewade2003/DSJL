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
		/// �õ����ID
		/// </summary>
		public int GetMaxId()
		{
			return dal.GetMaxId();
		}

		/// <summary>
		/// �Ƿ���ڸü�¼
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
		/// ����һ������
		/// </summary>
		public void Add(DSJL.Model.TB_StandTestRefe model)
		{
			dal.Add(model);
		}

		/// <summary>
		/// ����һ������
		/// </summary>
		public bool Update(DSJL.Model.TB_StandTestRefe model)
		{
			return dal.Update(model);
		}

		/// <summary>
		/// ɾ��һ������
		/// </summary>
		public bool Delete(int ID)
		{
			
			return dal.Delete(ID);
		}

        public bool Delete(int testInfoID, int StandID) {
            return dal.Delete(testInfoID, StandID);
        }
		/// <summary>
		/// ɾ��һ������
		/// </summary>
		public bool DeleteList(string IDlist )
		{
			return dal.DeleteList(IDlist );
		}

		/// <summary>
		/// �õ�һ������ʵ��
		/// </summary>
		public DSJL.Model.TB_StandTestRefe GetModel(int ID)
		{
			
			return dal.GetModel(ID);
		}

		/// <summary>
		/// ��������б�
		/// </summary>
		public DataSet GetList(string strWhere)
		{
			return dal.GetList(strWhere);
		}
		/// <summary>
		/// ��������б�
		/// </summary>
		public List<DSJL.Model.TB_StandTestRefe> GetModelList(string strWhere)
		{
			DataSet ds = dal.GetList(strWhere);
			return DataTableToList(ds.Tables[0]);
		}
		/// <summary>
		/// ��������б�
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
		/// ��������б�
		/// </summary>
		public DataSet GetAllList()
		{
			return GetList("");
		}

		/// <summary>
		/// ��ҳ��ȡ�����б�
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

