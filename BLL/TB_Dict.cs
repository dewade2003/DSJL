using System;
using System.Data;
using System.Collections.Generic;
using DSJL.Model;
namespace DSJL.BLL
{
	/// <summary>
	/// TB_Dict
	/// </summary>
	public partial class TB_Dict
	{
		private readonly DSJL.DAL.TB_Dict dal=new DSJL.DAL.TB_Dict();
		public TB_Dict()
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
		public void Add(DSJL.Model.TB_Dict model)
		{
			dal.Add(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(DSJL.Model.TB_Dict model)
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
		public DSJL.Model.TB_Dict GetModel(int ID)
		{
			
			return dal.GetModel(ID);
		}

        public DSJL.Model.TB_Dict GetModel(string joint, string plane,string testmode) {
            return GetModelList("dict_groupid=(select id from tb_dict where dict_key='" + joint + "' and dict_groupid=2) and dict_key='" + plane + "' and instr(dict_groupid2,"+testmode+")>0")[0];
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
		public List<DSJL.Model.TB_Dict> GetModelList(string strWhere)
		{
			DataSet ds = dal.GetList(strWhere);
			return DataTableToList(ds.Tables[0]);
		}
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<DSJL.Model.TB_Dict> DataTableToList(DataTable dt)
		{
			List<DSJL.Model.TB_Dict> modelList = new List<DSJL.Model.TB_Dict>();
			int rowsCount = dt.Rows.Count;
			if (rowsCount > 0)
			{
				DSJL.Model.TB_Dict model;
				for (int n = 0; n < rowsCount; n++)
				{
					model = new DSJL.Model.TB_Dict();
					if(dt.Rows[n]["ID"].ToString()!="")
					{
						model.ID=int.Parse(dt.Rows[n]["ID"].ToString());
					}
					if(dt.Rows[n]["Dict_GroupID"].ToString()!="")
					{
						model.Dict_GroupID=int.Parse(dt.Rows[n]["Dict_GroupID"].ToString());
					}
					model.Dict_Value=dt.Rows[n]["Dict_Value"].ToString();
					model.Dict_Key=dt.Rows[n]["Dict_Key"].ToString();
					model.actionone=dt.Rows[n]["actionone"].ToString();
					model.actiontwo=dt.Rows[n]["actiontwo"].ToString();
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

