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
		public void Add(DSJL.Model.TB_TestManager model)
		{
			dal.Add(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(DSJL.Model.TB_TestManager model)
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
		public DSJL.Model.TB_TestManager GetModel(int ID)
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
		public List<DSJL.Model.TB_TestManager> GetModelList(string strWhere)
		{
			DataSet ds = dal.GetList(strWhere);
            testItemList=DataTableToList(ds.Tables[0]);
            return testItemList;
		}
		/// <summary>
		/// 获得数据列表
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

