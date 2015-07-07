using System;
using System.Data;
using System.Text;
using System.Data.OleDb;
using DSJL.DBUtility;//Please add references
namespace DSJL.DAL
{
	/// <summary>
	/// 数据访问类:TB_AthleteInfo
	/// </summary>
	public partial class TB_AthleteInfo
	{
		public TB_AthleteInfo()
		{}
		#region  Method

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
		return DbHelperOleDb.GetMaxID("ID", "TB_AthleteInfo"); 
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int ID)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from TB_AthleteInfo");
			strSql.Append(" where ID=OLEDBID ");
			OleDbParameter[] parameters = {
					new OleDbParameter("OLEDBID", OleDbType.Integer,4)};
			parameters[0].Value = ID;

			return DbHelperOleDb.Exists(strSql.ToString(),parameters);
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(DSJL.Model.TB_AthleteInfo model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into TB_AthleteInfo(");
			strSql.Append("Ath_Code,Ath_Name,Ath_PinYin,Ath_Sex,Ath_Birthday,Ath_Height,Ath_Weight,Ath_Project,Ath_MainProject,Ath_TrainYears,Ath_Level,Ath_Team,Ath_TestDate,Ath_TestAddress,Ath_TestMachine,Ath_TestState,Ath_Remark,Ath_TestID)");
			strSql.Append(" values (");
			strSql.Append("OLEDBAth_Code,OLEDBAth_Name,OLEDBAth_PinYin,OLEDBAth_Sex,OLEDBAth_Birthday,OLEDBAth_Height,OLEDBAth_Weight,OLEDBAth_Project,OLEDBAth_MainProject,OLEDBAth_TrainYears,OLEDBAth_Level,OLEDBAth_Team,OLEDBAth_TestDate,OLEDBAth_TestAddress,OLEDBAth_TestMachine,OLEDBAth_TestState,OLEDBAth_Remark,OLEDBAth_TestID)");
			OleDbParameter[] parameters = {
					new OleDbParameter("OLEDBAth_Code", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_Name", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_PinYin", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_Sex", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_Birthday", OleDbType.Date),
					new OleDbParameter("OLEDBAth_Height", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_Weight", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_Project", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_MainProject", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_TrainYears", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_Level", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_Team", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_TestDate", OleDbType.Date),
					new OleDbParameter("OLEDBAth_TestAddress", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_TestMachine", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_TestState", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_Remark", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_TestID", OleDbType.Integer,4)};
			parameters[0].Value = model.Ath_Code;
			parameters[1].Value = model.Ath_Name;
			parameters[2].Value = model.Ath_PinYin;
			parameters[3].Value = model.Ath_Sex;
			parameters[4].Value = model.Ath_Birthday;
			parameters[5].Value = model.Ath_Height;
			parameters[6].Value = model.Ath_Weight;
			parameters[7].Value = model.Ath_Project;
			parameters[8].Value = model.Ath_MainProject;
			parameters[9].Value = model.Ath_TrainYears;
			parameters[10].Value = model.Ath_Level;
			parameters[11].Value = model.Ath_Team;
			parameters[12].Value = model.Ath_TestDate;
			parameters[13].Value = model.Ath_TestAddress;
			parameters[14].Value = model.Ath_TestMachine;
			parameters[15].Value = model.Ath_TestState;
			parameters[16].Value = model.Ath_Remark;
			parameters[17].Value = model.Ath_TestID;

			DbHelperOleDb.ExecuteSql(strSql.ToString(),parameters);
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(DSJL.Model.TB_AthleteInfo model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update TB_AthleteInfo set ");
			strSql.Append("Ath_Code=OLEDBAth_Code,");
			strSql.Append("Ath_Name=OLEDBAth_Name,");
			strSql.Append("Ath_PinYin=OLEDBAth_PinYin,");
			strSql.Append("Ath_Sex=OLEDBAth_Sex,");
			strSql.Append("Ath_Birthday=OLEDBAth_Birthday,");
			strSql.Append("Ath_Height=OLEDBAth_Height,");
			strSql.Append("Ath_Weight=OLEDBAth_Weight,");
			strSql.Append("Ath_Project=OLEDBAth_Project,");
			strSql.Append("Ath_MainProject=OLEDBAth_MainProject,");
			strSql.Append("Ath_TrainYears=OLEDBAth_TrainYears,");
			strSql.Append("Ath_Level=OLEDBAth_Level,");
			strSql.Append("Ath_Team=OLEDBAth_Team,");
			strSql.Append("Ath_TestDate=OLEDBAth_TestDate,");
			strSql.Append("Ath_TestAddress=OLEDBAth_TestAddress,");
			strSql.Append("Ath_TestMachine=OLEDBAth_TestMachine,");
			strSql.Append("Ath_TestState=OLEDBAth_TestState,");
			strSql.Append("Ath_Remark=OLEDBAth_Remark,");
			strSql.Append("Ath_TestID=OLEDBAth_TestID");
			strSql.Append(" where ID=OLEDBID");
			OleDbParameter[] parameters = {
					new OleDbParameter("OLEDBAth_Code", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_Name", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_PinYin", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_Sex", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_Birthday", OleDbType.Date),
					new OleDbParameter("OLEDBAth_Height", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_Weight", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_Project", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_MainProject", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_TrainYears", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_Level", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_Team", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_TestDate", OleDbType.Date),
					new OleDbParameter("OLEDBAth_TestAddress", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_TestMachine", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_TestState", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_Remark", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAth_TestID", OleDbType.Integer,4),
					new OleDbParameter("OLEDBID", OleDbType.Integer,4)};
			parameters[0].Value = model.Ath_Code;
			parameters[1].Value = model.Ath_Name;
			parameters[2].Value = model.Ath_PinYin;
			parameters[3].Value = model.Ath_Sex;
			parameters[4].Value = model.Ath_Birthday;
			parameters[5].Value = model.Ath_Height;
			parameters[6].Value = model.Ath_Weight;
			parameters[7].Value = model.Ath_Project;
			parameters[8].Value = model.Ath_MainProject;
			parameters[9].Value = model.Ath_TrainYears;
			parameters[10].Value = model.Ath_Level;
			parameters[11].Value = model.Ath_Team;
			parameters[12].Value = model.Ath_TestDate;
			parameters[13].Value = model.Ath_TestAddress;
			parameters[14].Value = model.Ath_TestMachine;
			parameters[15].Value = model.Ath_TestState;
			parameters[16].Value = model.Ath_Remark;
			parameters[17].Value = model.Ath_TestID;
			parameters[18].Value = model.ID;

			int rows=DbHelperOleDb.ExecuteSql(strSql.ToString(),parameters);
			if (rows > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(int ID)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from TB_AthleteInfo ");
			strSql.Append(" where ID=OLEDBID");
			OleDbParameter[] parameters = {
					new OleDbParameter("OLEDBID", OleDbType.Integer,4)
};
			parameters[0].Value = ID;

			int rows=DbHelperOleDb.ExecuteSql(strSql.ToString(),parameters);
			if (rows > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool DeleteList(string IDlist )
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from TB_AthleteInfo ");
			strSql.Append(" where ID in ("+IDlist + ")  ");
			int rows=DbHelperOleDb.ExecuteSql(strSql.ToString());
			if (rows > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}


        public bool HiddenData(string ids,int value) {
            string sql = "update tb_athleteinfo set hidden=" + value + " where id in (" + ids + ")";
            int rows = DbHelperOleDb.ExecuteSql(sql);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public DSJL.Model.TB_AthleteInfo GetModel(int ID)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select ID,Ath_Code,Ath_Name,Ath_PinYin,Ath_Sex,Ath_Birthday,Ath_Height,Ath_Weight,Ath_Project,Ath_MainProject,Ath_TrainYears,Ath_Level,Ath_Team,Ath_TestDate,Ath_TestAddress,Ath_TestMachine,Ath_TestState,Ath_Remark,Ath_TestID from TB_AthleteInfo ");
			strSql.Append(" where ID=OLEDBID");
			OleDbParameter[] parameters = {
					new OleDbParameter("OLEDBID", OleDbType.Integer,4)
};
			parameters[0].Value = ID;

			DSJL.Model.TB_AthleteInfo model=new DSJL.Model.TB_AthleteInfo();
			DataSet ds=DbHelperOleDb.Query(strSql.ToString(),parameters);
			if(ds.Tables[0].Rows.Count>0)
			{
				if(ds.Tables[0].Rows[0]["ID"].ToString()!="")
				{
					model.ID=int.Parse(ds.Tables[0].Rows[0]["ID"].ToString());
				}
				model.Ath_Code=ds.Tables[0].Rows[0]["Ath_Code"].ToString();
				model.Ath_Name=ds.Tables[0].Rows[0]["Ath_Name"].ToString();
				model.Ath_PinYin=ds.Tables[0].Rows[0]["Ath_PinYin"].ToString();
				model.Ath_Sex=ds.Tables[0].Rows[0]["Ath_Sex"].ToString();
				if(ds.Tables[0].Rows[0]["Ath_Birthday"].ToString()!="")
				{
					model.Ath_Birthday=DateTime.Parse(ds.Tables[0].Rows[0]["Ath_Birthday"].ToString());
				}
				model.Ath_Height=ds.Tables[0].Rows[0]["Ath_Height"].ToString();
				model.Ath_Weight=ds.Tables[0].Rows[0]["Ath_Weight"].ToString();
				model.Ath_Project=ds.Tables[0].Rows[0]["Ath_Project"].ToString();
				model.Ath_MainProject=ds.Tables[0].Rows[0]["Ath_MainProject"].ToString();
				model.Ath_TrainYears=ds.Tables[0].Rows[0]["Ath_TrainYears"].ToString();
				model.Ath_Level=ds.Tables[0].Rows[0]["Ath_Level"].ToString();
				model.Ath_Team=ds.Tables[0].Rows[0]["Ath_Team"].ToString();
				if(ds.Tables[0].Rows[0]["Ath_TestDate"].ToString()!="")
				{
					model.Ath_TestDate=DateTime.Parse(ds.Tables[0].Rows[0]["Ath_TestDate"].ToString());
				}
				model.Ath_TestAddress=ds.Tables[0].Rows[0]["Ath_TestAddress"].ToString();
				model.Ath_TestMachine=ds.Tables[0].Rows[0]["Ath_TestMachine"].ToString();
				model.Ath_TestState=ds.Tables[0].Rows[0]["Ath_TestState"].ToString();
				model.Ath_Remark=ds.Tables[0].Rows[0]["Ath_Remark"].ToString();
				if(ds.Tables[0].Rows[0]["Ath_TestID"].ToString()!="")
				{
					model.Ath_TestID=int.Parse(ds.Tables[0].Rows[0]["Ath_TestID"].ToString());
				}
				return model;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetList(string strWhere)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select ID,Ath_Code,Ath_Name,Ath_PinYin,Ath_Sex,Ath_Birthday,Ath_Height,Ath_Weight,Ath_Project,Ath_MainProject,Ath_TrainYears,Ath_Level,Ath_Team,Ath_TestDate,Ath_TestAddress,Ath_TestMachine,Ath_TestState,Ath_Remark,Ath_TestID,hidden ");
			strSql.Append(" FROM TB_AthleteInfo ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
            strSql.Append(" order by id asc");
			return DbHelperOleDb.Query(strSql.ToString());
		}

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere,string orderBy)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,Ath_Code,Ath_Name,Ath_PinYin,Ath_Sex,Ath_Birthday,Ath_Height,Ath_Weight,Ath_Project,Ath_MainProject,Ath_TrainYears,Ath_Level,Ath_Team,Ath_TestDate,Ath_TestAddress,Ath_TestMachine,Ath_TestState,Ath_Remark,Ath_TestID,hidden ");
            strSql.Append(" FROM TB_AthleteInfo ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            if (orderBy.Trim() != "")
            {
                strSql.Append(" order by "+orderBy);
            }
            else {
                strSql.Append(" order by id desc");
            }
            
            return DbHelperOleDb.Query(strSql.ToString());
        }

        public DataSet GetColumnDistinctList(string columnName) {
            string sql = "select distinct " + columnName + " from tb_athleteinfo";
            return DbHelperOleDb.Query(sql);
        }
		/*
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public DataSet GetList(int PageSize,int PageIndex,string strWhere)
		{
			OleDbParameter[] parameters = {
					new OleDbParameter("OLEDBtblName", OleDbType.VarChar, 255),
					new OleDbParameter("OLEDBfldName", OleDbType.VarChar, 255),
					new OleDbParameter("OLEDBPageSize", OleDbType.Integer),
					new OleDbParameter("OLEDBPageIndex", OleDbType.Integer),
					new OleDbParameter("OLEDBIsReCount", OleDbType.Boolean),
					new OleDbParameter("OLEDBOrderType", OleDbType.Boolean),
					new OleDbParameter("OLEDBstrWhere", OleDbType.VarChar,1000),
					};
			parameters[0].Value = "TB_AthleteInfo";
			parameters[1].Value = "ID";
			parameters[2].Value = PageSize;
			parameters[3].Value = PageIndex;
			parameters[4].Value = 0;
			parameters[5].Value = 0;
			parameters[6].Value = strWhere;	
			return DbHelperOleDb.RunProcedure("UP_GetRecordByPage",parameters,"ds");
		}*/

		#endregion  Method
	}
}

