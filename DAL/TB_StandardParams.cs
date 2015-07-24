using System;
using System.Data;
using System.Text;
using System.Data.OleDb;
using DSJL.DBUtility;//Please add references
namespace DSJL.DAL
{
	/// <summary>
	/// 数据访问类:TB_StandardParams
	/// </summary>
	public partial class TB_StandardParams
	{
		public TB_StandardParams()
		{}
		#region  BasicMethod

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
		return DbHelperOleDb.GetMaxID("ID", "TB_StandardParams"); 
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int ID)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from TB_StandardParams");
			strSql.Append(" where ID=@ID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ID", OleDbType.Integer,4)
			};
			parameters[0].Value = ID;

			return DbHelperOleDb.Exists(strSql.ToString(),parameters);
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public bool Add(DSJL.Model.TB_StandardParams model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into TB_StandardParams(");
			strSql.Append("StandID,StandName,Ath_Sex,Ath_AgeMinLimit,Ath_AgeMaxLimit,Ath_Project,Ath_MainProject,Joint_Side,Test_Mode,Joint,Plane,Speed1,Speed2)");
			strSql.Append(" values (");
			strSql.Append("@StandID,@StandName,@Ath_Sex,@Ath_AgeMinLimit,@Ath_AgeMaxLimit,@Ath_Project,@Ath_MainProject,@Joint_Side,@Test_Mode,@Joint,@Plane,@Speed1,@Speed2)");
			OleDbParameter[] parameters = {
					new OleDbParameter("@StandID", OleDbType.Integer,4),
					new OleDbParameter("@StandName", OleDbType.VarChar,255),
					new OleDbParameter("@Ath_Sex", OleDbType.VarChar,255),
					new OleDbParameter("@Ath_AgeMinLimit", OleDbType.Integer,4),
					new OleDbParameter("@Ath_AgeMaxLimit", OleDbType.Integer,4),
					new OleDbParameter("@Ath_Project", OleDbType.VarChar,255),
					new OleDbParameter("@Ath_MainProject", OleDbType.VarChar,255),
					new OleDbParameter("@Joint_Side", OleDbType.VarChar,255),
					new OleDbParameter("@Test_Mode", OleDbType.VarChar,255),
					new OleDbParameter("@Joint", OleDbType.VarChar,255),
					new OleDbParameter("@Plane", OleDbType.VarChar,255),
					new OleDbParameter("@Speed1", OleDbType.VarChar,255),
					new OleDbParameter("@Speed2", OleDbType.VarChar,255)};
			parameters[0].Value = model.StandID;
			parameters[1].Value = model.StandName;
			parameters[2].Value = model.Ath_Sex;
			parameters[3].Value = model.Ath_AgeMinLimit;
			parameters[4].Value = model.Ath_AgeMaxLimit;
			parameters[5].Value = model.Ath_Project;
			parameters[6].Value = model.Ath_MainProject;
			parameters[7].Value = model.Joint_Side;
			parameters[8].Value = model.Test_Mode;
			parameters[9].Value = model.Joint;
			parameters[10].Value = model.Plane;
			parameters[11].Value = model.Speed1;
			parameters[12].Value = model.Speed2;

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
		/// 更新一条数据
		/// </summary>
		public bool Update(DSJL.Model.TB_StandardParams model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update TB_StandardParams set ");
			strSql.Append("StandID=@StandID,");
			strSql.Append("StandName=@StandName,");
			strSql.Append("Ath_Sex=@Ath_Sex,");
			strSql.Append("Ath_AgeMinLimit=@Ath_AgeMinLimit,");
			strSql.Append("Ath_AgeMaxLimit=@Ath_AgeMaxLimit,");
			strSql.Append("Ath_Project=@Ath_Project,");
			strSql.Append("Ath_MainProject=@Ath_MainProject,");
			strSql.Append("Joint_Side=@Joint_Side,");
			strSql.Append("Test_Mode=@Test_Mode,");
			strSql.Append("Joint=@Joint,");
			strSql.Append("Plane=@Plane,");
			strSql.Append("Speed1=@Speed1,");
			strSql.Append("Speed2=@Speed2");
			strSql.Append(" where ID=@ID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@StandID", OleDbType.Integer,4),
					new OleDbParameter("@StandName", OleDbType.VarChar,255),
					new OleDbParameter("@Ath_Sex", OleDbType.VarChar,255),
					new OleDbParameter("@Ath_AgeMinLimit", OleDbType.Integer,4),
					new OleDbParameter("@Ath_AgeMaxLimit", OleDbType.Integer,4),
					new OleDbParameter("@Ath_Project", OleDbType.VarChar,255),
					new OleDbParameter("@Ath_MainProject", OleDbType.VarChar,255),
					new OleDbParameter("@Joint_Side", OleDbType.VarChar,255),
					new OleDbParameter("@Test_Mode", OleDbType.VarChar,255),
					new OleDbParameter("@Joint", OleDbType.VarChar,255),
					new OleDbParameter("@Plane", OleDbType.VarChar,255),
					new OleDbParameter("@Speed1", OleDbType.VarChar,255),
					new OleDbParameter("@Speed2", OleDbType.VarChar,255),
					new OleDbParameter("@ID", OleDbType.Integer,4)};
			parameters[0].Value = model.StandID;
			parameters[1].Value = model.StandName;
			parameters[2].Value = model.Ath_Sex;
			parameters[3].Value = model.Ath_AgeMinLimit;
			parameters[4].Value = model.Ath_AgeMaxLimit;
			parameters[5].Value = model.Ath_Project;
			parameters[6].Value = model.Ath_MainProject;
			parameters[7].Value = model.Joint_Side;
			parameters[8].Value = model.Test_Mode;
			parameters[9].Value = model.Joint;
			parameters[10].Value = model.Plane;
			parameters[11].Value = model.Speed1;
			parameters[12].Value = model.Speed2;
			parameters[13].Value = model.ID;

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
			strSql.Append("delete from TB_StandardParams ");
			strSql.Append(" where ID=@ID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ID", OleDbType.Integer,4)
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
		/// 批量删除数据
		/// </summary>
		public bool DeleteList(string IDlist )
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from TB_StandardParams ");
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


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public DSJL.Model.TB_StandardParams GetModel(int ID)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select ID,StandID,StandName,Ath_Sex,Ath_AgeMinLimit,Ath_AgeMaxLimit,Ath_Project,Ath_MainProject,Joint_Side,Test_Mode,Joint,Plane,Speed1,Speed2 from TB_StandardParams ");
			strSql.Append(" where ID=@ID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ID", OleDbType.Integer,4)
			};
			parameters[0].Value = ID;

			DSJL.Model.TB_StandardParams model=new DSJL.Model.TB_StandardParams();
			DataSet ds=DbHelperOleDb.Query(strSql.ToString(),parameters);
			if(ds.Tables[0].Rows.Count>0)
			{
				return DataRowToModel(ds.Tables[0].Rows[0]);
			}
			else
			{
				return null;
			}
		}


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public DSJL.Model.TB_StandardParams GetModelByStandID(int ID)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,StandID,StandName,Ath_Sex,Ath_AgeMinLimit,Ath_AgeMaxLimit,Ath_Project,Ath_MainProject,Joint_Side,Test_Mode,Joint,Plane,Speed1,Speed2 from TB_StandardParams ");
            strSql.Append(" where StandID=@ID");
            OleDbParameter[] parameters = {
					new OleDbParameter("@ID", OleDbType.Integer,4)
			};
            parameters[0].Value = ID;

            DSJL.Model.TB_StandardParams model = new DSJL.Model.TB_StandardParams();
            DataSet ds = DbHelperOleDb.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public DSJL.Model.TB_StandardParams DataRowToModel(DataRow row)
		{
			DSJL.Model.TB_StandardParams model=new DSJL.Model.TB_StandardParams();
			if (row != null)
			{
				if(row["ID"]!=null && row["ID"].ToString()!="")
				{
					model.ID=int.Parse(row["ID"].ToString());
				}
				if(row["StandID"]!=null && row["StandID"].ToString()!="")
				{
					model.StandID=int.Parse(row["StandID"].ToString());
				}
				if(row["StandName"]!=null)
				{
					model.StandName=row["StandName"].ToString();
				}
				if(row["Ath_Sex"]!=null)
				{
					model.Ath_Sex=row["Ath_Sex"].ToString();
				}
				if(row["Ath_AgeMinLimit"]!=null && row["Ath_AgeMinLimit"].ToString()!="")
				{
					model.Ath_AgeMinLimit=int.Parse(row["Ath_AgeMinLimit"].ToString());
				}
				if(row["Ath_AgeMaxLimit"]!=null && row["Ath_AgeMaxLimit"].ToString()!="")
				{
					model.Ath_AgeMaxLimit=int.Parse(row["Ath_AgeMaxLimit"].ToString());
				}
				if(row["Ath_Project"]!=null)
				{
					model.Ath_Project=row["Ath_Project"].ToString();
				}
				if(row["Ath_MainProject"]!=null)
				{
					model.Ath_MainProject=row["Ath_MainProject"].ToString();
				}
				if(row["Joint_Side"]!=null)
				{
					model.Joint_Side=row["Joint_Side"].ToString();
				}
				if(row["Test_Mode"]!=null)
				{
					model.Test_Mode=row["Test_Mode"].ToString();
				}
				if(row["Joint"]!=null)
				{
					model.Joint=row["Joint"].ToString();
				}
				if(row["Plane"]!=null)
				{
					model.Plane=row["Plane"].ToString();
				}
				if(row["Speed1"]!=null)
				{
					model.Speed1=row["Speed1"].ToString();
				}
				if(row["Speed2"]!=null)
				{
					model.Speed2=row["Speed2"].ToString();
				}
			}
			return model;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetList(string strWhere)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select ID,StandID,StandName,Ath_Sex,Ath_AgeMinLimit,Ath_AgeMaxLimit,Ath_Project,Ath_MainProject,Joint_Side,Test_Mode,Joint,Plane,Speed1,Speed2 ");
			strSql.Append(" FROM TB_StandardParams ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			return DbHelperOleDb.Query(strSql.ToString());
		}

		/// <summary>
		/// 获取记录总数
		/// </summary>
		public int GetRecordCount(string strWhere)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) FROM TB_StandardParams ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
            object obj = DbHelperOleDb.GetSingle(strSql.ToString());
			if (obj == null)
			{
				return 0;
			}
			else
			{
				return Convert.ToInt32(obj);
			}
		}
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("SELECT * FROM ( ");
			strSql.Append(" SELECT ROW_NUMBER() OVER (");
			if (!string.IsNullOrEmpty(orderby.Trim()))
			{
				strSql.Append("order by T." + orderby );
			}
			else
			{
				strSql.Append("order by T.ID desc");
			}
			strSql.Append(")AS Row, T.*  from TB_StandardParams T ");
			if (!string.IsNullOrEmpty(strWhere.Trim()))
			{
				strSql.Append(" WHERE " + strWhere);
			}
			strSql.Append(" ) TT");
			strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
			return DbHelperOleDb.Query(strSql.ToString());
		}

		/*
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public DataSet GetList(int PageSize,int PageIndex,string strWhere)
		{
			OleDbParameter[] parameters = {
					new OleDbParameter("@tblName", OleDbType.VarChar, 255),
					new OleDbParameter("@fldName", OleDbType.VarChar, 255),
					new OleDbParameter("@PageSize", OleDbType.Integer),
					new OleDbParameter("@PageIndex", OleDbType.Integer),
					new OleDbParameter("@IsReCount", OleDbType.Boolean),
					new OleDbParameter("@OrderType", OleDbType.Boolean),
					new OleDbParameter("@strWhere", OleDbType.VarChar,1000),
					};
			parameters[0].Value = "TB_StandardParams";
			parameters[1].Value = "ID";
			parameters[2].Value = PageSize;
			parameters[3].Value = PageIndex;
			parameters[4].Value = 0;
			parameters[5].Value = 0;
			parameters[6].Value = strWhere;	
			return DbHelperOleDb.RunProcedure("UP_GetRecordByPage",parameters,"ds");
		}*/

		#endregion  BasicMethod
		#region  ExtensionMethod

		#endregion  ExtensionMethod
	}
}

