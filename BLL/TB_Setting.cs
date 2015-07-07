using System;
using System.Data;
using System.Collections.Generic;
using DSJL.Model;
namespace DSJL.BLL
{
	/// <summary>
	/// TB_Setting
	/// </summary>
	public partial class TB_Setting
	{
		private static readonly DSJL.DAL.TB_Setting dal=new DSJL.DAL.TB_Setting();
		public TB_Setting()
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
		public void Add(DSJL.Model.TB_Setting model)
		{
			dal.Add(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(DSJL.Model.TB_Setting model)
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
		public DSJL.Model.TB_Setting GetModel(int ID)
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
		public List<DSJL.Model.TB_Setting> GetModelList(string strWhere)
		{
			DataSet ds = dal.GetList(strWhere);
			return DataTableToList(ds.Tables[0]);
		}
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public static List<DSJL.Model.TB_Setting> DataTableToList(DataTable dt)
		{
			List<DSJL.Model.TB_Setting> modelList = new List<DSJL.Model.TB_Setting>();
			int rowsCount = dt.Rows.Count;
			if (rowsCount > 0)
			{
				DSJL.Model.TB_Setting model;
				for (int n = 0; n < rowsCount; n++)
				{
					model = new DSJL.Model.TB_Setting();
					if(dt.Rows[n]["ID"].ToString()!="")
					{
						model.ID=int.Parse(dt.Rows[n]["ID"].ToString());
					}
					model.SetName=dt.Rows[n]["SetName"].ToString();
					model.SetValue=dt.Rows[n]["SetValue"].ToString();
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

        #region 扩展方法

        private static List<Model.TB_Setting> settingList = new List<Model.TB_Setting>();

        #region 隐藏设置
        private static bool isShowAllData = false;
        public static bool IsShowAllData
        {
            get {
                settingList = DataTableToList(dal.GetList("").Tables[0]);
                Model.TB_Setting hiddenSet = settingList.Find(x => x.SetName.ToLower() == "showall");
                isShowAllData = bool.Parse(hiddenSet.SetValue);
                return isShowAllData;
            }
        }

        public static string OldPwd {
            get {
                settingList = DataTableToList(dal.GetList("").Tables[0]);
                Model.TB_Setting hiddenSet = settingList.Find(x => x.SetName.ToLower() == "pwd");
                return hiddenSet.SetValue;
            }
        }

        public static bool ShowAllData(bool isShowAll) {
            settingList = DataTableToList(dal.GetList("").Tables[0]);
            Model.TB_Setting hiddenSet = settingList.Find(x => x.SetName.ToLower() == "showall");

            hiddenSet.SetValue = isShowAll.ToString();

            return dal.Update(hiddenSet);
        }

        public static bool UpdatePwd(string pwd) {
            settingList = DataTableToList(dal.GetList("").Tables[0]);
            Model.TB_Setting hiddenSet = settingList.Find(x => x.SetName.ToLower() == "pwd");

            hiddenSet.SetValue = pwd;

            return dal.Update(hiddenSet);
        }
        #endregion

        #endregion
    }
}

