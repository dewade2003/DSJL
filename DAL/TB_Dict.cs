using System;
using System.Data;
using System.Text;
using System.Data.OleDb;
using DSJL.DBUtility;//Please add references
namespace DSJL.DAL
{
	/// <summary>
	/// 数据访问类:TB_Dict
	/// </summary>
	public partial class TB_Dict
	{
		public TB_Dict()
		{}
		#region  Method

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
		return DbHelperOleDb.GetMaxID("ID", "TB_Dict"); 
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int ID)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from TB_Dict");
			strSql.Append(" where ID=OLEDBID ");
			OleDbParameter[] parameters = {
					new OleDbParameter("OLEDBID", OleDbType.Integer,4)};
			parameters[0].Value = ID;

			return DbHelperOleDb.Exists(strSql.ToString(),parameters);
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(DSJL.Model.TB_Dict model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into TB_Dict(");
			strSql.Append("Dict_GroupID,Dict_Value,Dict_Key,actionone,actiontwo)");
			strSql.Append(" values (");
			strSql.Append("OLEDBDict_GroupID,OLEDBDict_Value,OLEDBDict_Key,OLEDBactionone,OLEDBactiontwo)");
			OleDbParameter[] parameters = {
					new OleDbParameter("OLEDBDict_GroupID", OleDbType.Integer,4),
					new OleDbParameter("OLEDBDict_Value", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBDict_Key", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBactionone", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBactiontwo", OleDbType.VarChar,255)};
			parameters[0].Value = model.Dict_GroupID;
			parameters[1].Value = model.Dict_Value;
			parameters[2].Value = model.Dict_Key;
			parameters[3].Value = model.actionone;
			parameters[4].Value = model.actiontwo;

			DbHelperOleDb.ExecuteSql(strSql.ToString(),parameters);
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(DSJL.Model.TB_Dict model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update TB_Dict set ");
			strSql.Append("Dict_GroupID=OLEDBDict_GroupID,");
			strSql.Append("Dict_Value=OLEDBDict_Value,");
			strSql.Append("Dict_Key=OLEDBDict_Key,");
			strSql.Append("actionone=OLEDBactionone,");
			strSql.Append("actiontwo=OLEDBactiontwo");
			strSql.Append(" where ID=OLEDBID");
			OleDbParameter[] parameters = {
					new OleDbParameter("OLEDBDict_GroupID", OleDbType.Integer,4),
					new OleDbParameter("OLEDBDict_Value", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBDict_Key", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBactionone", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBactiontwo", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBID", OleDbType.Integer,4)};
			parameters[0].Value = model.Dict_GroupID;
			parameters[1].Value = model.Dict_Value;
			parameters[2].Value = model.Dict_Key;
			parameters[3].Value = model.actionone;
			parameters[4].Value = model.actiontwo;
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
			strSql.Append("delete from TB_Dict ");
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
			strSql.Append("delete from TB_Dict ");
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
		public DSJL.Model.TB_Dict GetModel(int ID)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select ID,Dict_GroupID,Dict_Value,Dict_Key,actionone,actiontwo from TB_Dict ");
			strSql.Append(" where ID=OLEDBID");
			OleDbParameter[] parameters = {
					new OleDbParameter("OLEDBID", OleDbType.Integer,4)
};
			parameters[0].Value = ID;

			DSJL.Model.TB_Dict model=new DSJL.Model.TB_Dict();
			DataSet ds=DbHelperOleDb.Query(strSql.ToString(),parameters);
			if(ds.Tables[0].Rows.Count>0)
			{
				if(ds.Tables[0].Rows[0]["ID"].ToString()!="")
				{
					model.ID=int.Parse(ds.Tables[0].Rows[0]["ID"].ToString());
				}
				if(ds.Tables[0].Rows[0]["Dict_GroupID"].ToString()!="")
				{
					model.Dict_GroupID=int.Parse(ds.Tables[0].Rows[0]["Dict_GroupID"].ToString());
				}
				model.Dict_Value=ds.Tables[0].Rows[0]["Dict_Value"].ToString();
				model.Dict_Key=ds.Tables[0].Rows[0]["Dict_Key"].ToString();
				model.actionone=ds.Tables[0].Rows[0]["actionone"].ToString();
				model.actiontwo=ds.Tables[0].Rows[0]["actiontwo"].ToString();
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
			strSql.Append("select ID,Dict_GroupID,Dict_Value,Dict_Key,actionone,actiontwo ");
			strSql.Append(" FROM TB_Dict ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			return DbHelperOleDb.Query(strSql.ToString());
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
			parameters[0].Value = "TB_Dict";
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

