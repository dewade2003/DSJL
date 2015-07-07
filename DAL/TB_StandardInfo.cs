using System;
using System.Data;
using System.Text;
using System.Data.OleDb;
using DSJL.DBUtility;//Please add references
namespace DSJL.DAL
{
	/// <summary>
	/// 数据访问类:TB_StandardInfo
	/// </summary>
	public partial class TB_StandardInfo
	{
		public TB_StandardInfo()
		{}
		#region  BasicMethod

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
		return DbHelperOleDb.GetMaxID("ID", "TB_StandardInfo"); 
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int ID)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from TB_StandardInfo");
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
		public bool Add(DSJL.Model.TB_StandardInfo model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into TB_StandardInfo(");
			strSql.Append("Stand_Name,Stand_Date,Stand_Remark,Stand_Level,Stand_ParentID)");
			strSql.Append(" values (");
			strSql.Append("@Stand_Name,@Stand_Date,@Stand_Remark,@Stand_Level,@Stand_ParentID)");
			OleDbParameter[] parameters = {
					new OleDbParameter("@Stand_Name", OleDbType.VarChar,255),
					new OleDbParameter("@Stand_Date", OleDbType.VarChar,255),
					new OleDbParameter("@Stand_Remark", OleDbType.VarChar,255),
					new OleDbParameter("@Stand_Level", OleDbType.Integer,4),
					new OleDbParameter("@Stand_ParentID", OleDbType.Integer,4)};
			parameters[0].Value = model.Stand_Name;
			parameters[1].Value = model.Stand_Date;
			parameters[2].Value = model.Stand_Remark;
			parameters[3].Value = model.Stand_Level;
			parameters[4].Value = model.Stand_ParentID;

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
		public bool Update(DSJL.Model.TB_StandardInfo model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update TB_StandardInfo set ");
			strSql.Append("Stand_Name=@Stand_Name,");
			strSql.Append("Stand_Date=@Stand_Date,");
			strSql.Append("Stand_Remark=@Stand_Remark,");
			strSql.Append("Stand_Level=@Stand_Level,");
			strSql.Append("Stand_ParentID=@Stand_ParentID");
			strSql.Append(" where ID=@ID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@Stand_Name", OleDbType.VarChar,255),
					new OleDbParameter("@Stand_Date", OleDbType.VarChar,255),
					new OleDbParameter("@Stand_Remark", OleDbType.VarChar,255),
					new OleDbParameter("@Stand_Level", OleDbType.Integer,4),
					new OleDbParameter("@Stand_ParentID", OleDbType.Integer,4),
					new OleDbParameter("@ID", OleDbType.Integer,4)};
			parameters[0].Value = model.Stand_Name;
			parameters[1].Value = model.Stand_Date;
			parameters[2].Value = model.Stand_Remark;
			parameters[3].Value = model.Stand_Level;
			parameters[4].Value = model.Stand_ParentID;
			parameters[5].Value = model.ID;

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
			strSql.Append("delete from TB_StandardInfo ");
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
			strSql.Append("delete from TB_StandardInfo ");
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
		public DSJL.Model.TB_StandardInfo GetModel(int ID)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select ID,Stand_Name,Stand_Date,Stand_Remark,Stand_Level,Stand_ParentID from TB_StandardInfo ");
			strSql.Append(" where ID=@ID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ID", OleDbType.Integer,4)
			};
			parameters[0].Value = ID;

			DSJL.Model.TB_StandardInfo model=new DSJL.Model.TB_StandardInfo();
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
		public DSJL.Model.TB_StandardInfo DataRowToModel(DataRow row)
		{
			DSJL.Model.TB_StandardInfo model=new DSJL.Model.TB_StandardInfo();
			if (row != null)
			{
				if(row["ID"]!=null && row["ID"].ToString()!="")
				{
					model.ID=int.Parse(row["ID"].ToString());
				}
				if(row["Stand_Name"]!=null)
				{
					model.Stand_Name=row["Stand_Name"].ToString();
				}
				if(row["Stand_Date"]!=null)
				{
					model.Stand_Date=row["Stand_Date"].ToString();
				}
				if(row["Stand_Remark"]!=null)
				{
					model.Stand_Remark=row["Stand_Remark"].ToString();
				}
				if(row["Stand_Level"]!=null && row["Stand_Level"].ToString()!="")
				{
					model.Stand_Level=int.Parse(row["Stand_Level"].ToString());
				}
				if(row["Stand_ParentID"]!=null && row["Stand_ParentID"].ToString()!="")
				{
					model.Stand_ParentID=int.Parse(row["Stand_ParentID"].ToString());
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
			strSql.Append("select ID,Stand_Name,Stand_Date,Stand_Remark,Stand_Level,Stand_ParentID ");
			strSql.Append(" FROM TB_StandardInfo ");
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
			strSql.Append("select count(1) FROM TB_StandardInfo ");
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
			strSql.Append(")AS Row, T.*  from TB_StandardInfo T ");
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
			parameters[0].Value = "TB_StandardInfo";
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

