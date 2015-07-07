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
		public void Add(DSJL.Model.TB_Setting model)
		{
			dal.Add(model);
		}

		/// <summary>
		/// ����һ������
		/// </summary>
		public bool Update(DSJL.Model.TB_Setting model)
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
		public DSJL.Model.TB_Setting GetModel(int ID)
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
		public List<DSJL.Model.TB_Setting> GetModelList(string strWhere)
		{
			DataSet ds = dal.GetList(strWhere);
			return DataTableToList(ds.Tables[0]);
		}
		/// <summary>
		/// ��������б�
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

        #region ��չ����

        private static List<Model.TB_Setting> settingList = new List<Model.TB_Setting>();

        #region ��������
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

