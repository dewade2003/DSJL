using System;
using System.Data;
using System.Text;
using System.Data.OleDb;
using DSJL.DBUtility;//Please add references
namespace DSJL.DAL
{
	/// <summary>
	/// 数据访问类:TB_TestManager
	/// </summary>
	public partial class TB_TestManager
	{
		public TB_TestManager()
		{}
		#region  Method

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
		return DbHelperOleDb.GetMaxID("ID", "TB_TestManager"); 
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int ID)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from TB_TestManager");
			strSql.Append(" where ID=OLEDBID ");
			OleDbParameter[] parameters = {
					new OleDbParameter("OLEDBID", OleDbType.Integer,4)};
			parameters[0].Value = ID;

			return DbHelperOleDb.Exists(strSql.ToString(),parameters);
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(DSJL.Model.TB_TestManager model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into TB_TestManager(");
			strSql.Append("TestName,TestStartDate,TestEndDate,Remark)");
			strSql.Append(" values (");
			strSql.Append("OLEDBTestName,OLEDBTestStartDate,OLEDBTestEndDate,OLEDBRemark)");
			OleDbParameter[] parameters = {
					new OleDbParameter("OLEDBTestName", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBTestStartDate", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBTestEndDate", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBRemark", OleDbType.VarChar,255)};
			parameters[0].Value = model.TestName;
			parameters[1].Value = model.TestStartDate;
			parameters[2].Value = model.TestEndDate;
			parameters[3].Value = model.Remark;

			DbHelperOleDb.ExecuteSql(strSql.ToString(),parameters);
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(DSJL.Model.TB_TestManager model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update TB_TestManager set ");
			strSql.Append("TestName=OLEDBTestName,");
			strSql.Append("TestStartDate=OLEDBTestStartDate,");
			strSql.Append("TestEndDate=OLEDBTestEndDate,");
			strSql.Append("Remark=OLEDBRemark");
			strSql.Append(" where ID=OLEDBID");
			OleDbParameter[] parameters = {
					new OleDbParameter("OLEDBTestName", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBTestStartDate", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBTestEndDate", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBRemark", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBID", OleDbType.Integer,4)};
			parameters[0].Value = model.TestName;
			parameters[1].Value = model.TestStartDate;
			parameters[2].Value = model.TestEndDate;
			parameters[3].Value = model.Remark;
			parameters[4].Value = model.ID;

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
			strSql.Append("delete from TB_TestManager ");
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
			strSql.Append("delete from TB_TestManager ");
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
		public DSJL.Model.TB_TestManager GetModel(int ID)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select ID,TestName,TestStartDate,TestEndDate,Remark from TB_TestManager ");
			strSql.Append(" where ID=OLEDBID");
			OleDbParameter[] parameters = {
					new OleDbParameter("OLEDBID", OleDbType.Integer,4)
};
			parameters[0].Value = ID;

			DSJL.Model.TB_TestManager model=new DSJL.Model.TB_TestManager();
			DataSet ds=DbHelperOleDb.Query(strSql.ToString(),parameters);
			if(ds.Tables[0].Rows.Count>0)
			{
				if(ds.Tables[0].Rows[0]["ID"].ToString()!="")
				{
					model.ID=int.Parse(ds.Tables[0].Rows[0]["ID"].ToString());
				}
				model.TestName=ds.Tables[0].Rows[0]["TestName"].ToString();
				model.TestStartDate=ds.Tables[0].Rows[0]["TestStartDate"].ToString();
				model.TestEndDate=ds.Tables[0].Rows[0]["TestEndDate"].ToString();
				model.Remark=ds.Tables[0].Rows[0]["Remark"].ToString();
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
			strSql.Append("select ID,TestName,TestStartDate,TestEndDate,Remark ");
			strSql.Append(" FROM TB_TestManager ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
            strSql.Append(" order by id desc");
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
			parameters[0].Value = "TB_TestManager";
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

