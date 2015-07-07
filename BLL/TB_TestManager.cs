using System;
using System.Data;
using System.Collections.Generic;
using DSJL.Model;
namespace DSJL.BLL
{
	/// <summary>
	/// TB_TestManager
	/// </summary>
	public partial class TB_TestManager
	{
		private readonly DSJL.DAL.TB_TestManager dal=new DSJL.DAL.TB_TestManager();

        private static List<Model.TB_TestManager> testItemList = new List<Model.TB_TestManager>();

        public static List<Model.TB_TestManager> TestItemList {
            get {
                return testItemList;
            }
        }

		public TB_TestManager()
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

		/// <summary>
		/// ����һ������
		/// </summary>
		public void Add(DSJL.Model.TB_TestManager model)
		{
			dal.Add(model);
		}

		/// <summary>
		/// ����һ������
		/// </summary>
		public bool Update(DSJL.Model.TB_TestManager model)
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
		public DSJL.Model.TB_TestManager GetModel(int ID)
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
		public List<DSJL.Model.TB_TestManager> GetModelList(string strWhere)
		{
			DataSet ds = dal.GetList(strWhere);
            testItemList=DataTableToList(ds.Tables[0]);
            return testItemList;
		}
		/// <summary>
		/// ��������б�
		/// </summary>
		public List<DSJL.Model.TB_TestManager> DataTableToList(DataTable dt)
		{
			List<DSJL.Model.TB_TestManager> modelList = new List<DSJL.Model.TB_TestManager>();
			int rowsCount = dt.Rows.Count;
			if (rowsCount > 0)
			{
				DSJL.Model.TB_TestManager model;
				for (int n = 0; n < rowsCount; n++)
				{
					model = new DSJL.Model.TB_TestManager();
					if(dt.Rows[n]["ID"].ToString()!="")
					{
						model.ID=int.Parse(dt.Rows[n]["ID"].ToString());
					}
					model.TestName=dt.Rows[n]["TestName"].ToString();
					model.TestStartDate=dt.Rows[n]["TestStartDate"].ToString();
					model.TestEndDate=dt.Rows[n]["TestEndDate"].ToString();
					model.Remark=dt.Rows[n]["Remark"].ToString();
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
	}
}

