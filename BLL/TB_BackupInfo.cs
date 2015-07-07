using System;
using System.Data;
using System.Collections.Generic;
using DSJL.Model;
namespace DSJL.BLL
{
	/// <summary>
	/// TB_BackupInfo
	/// </summary>
	public partial class TB_BackupInfo
	{
		private readonly DSJL.DAL.TB_BackupInfo dal=new DSJL.DAL.TB_BackupInfo();
		public TB_BackupInfo()
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
		public void Add(DSJL.Model.TB_BackupInfo model)
		{
			dal.Add(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(DSJL.Model.TB_BackupInfo model)
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
		public DSJL.Model.TB_BackupInfo GetModel(int ID)
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
		public List<DSJL.Model.TB_BackupInfo> GetModelList(string strWhere)
		{
			DataSet ds = dal.GetList(strWhere);
			return DataTableToList(ds.Tables[0]);
		}
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<DSJL.Model.TB_BackupInfo> DataTableToList(DataTable dt)
		{
			List<DSJL.Model.TB_BackupInfo> modelList = new List<DSJL.Model.TB_BackupInfo>();
			int rowsCount = dt.Rows.Count;
			if (rowsCount > 0)
			{
				DSJL.Model.TB_BackupInfo model;
				for (int n = 0; n < rowsCount; n++)
				{
					model = new DSJL.Model.TB_BackupInfo();
                    model.Index = (n + 1).ToString();
					if(dt.Rows[n]["ID"].ToString()!="")
					{
						model.ID=int.Parse(dt.Rows[n]["ID"].ToString());
					}
					model.BackupName=dt.Rows[n]["BackupName"].ToString();
					model.BackupPath=dt.Rows[n]["BackupPath"].ToString();
					model.BackupDate=dt.Rows[n]["BackupDate"].ToString();
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

