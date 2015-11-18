using System;
using System.Data;
using System.Text;
using System.Data.OleDb;
using DSJL.DBUtility;//Please add references
namespace DSJL.DAL
{
	/// <summary>
	/// 数据访问类:TB_StandTestRefe
	/// </summary>
	public partial class TB_StandTestRefe
	{
		public TB_StandTestRefe()
		{}
		#region  Method

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
		return DbHelperOleDb.GetMaxID("ID", "TB_StandTestRefe"); 
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int ID)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from TB_StandTestRefe");
			strSql.Append(" where ID=OLEDBID ");
			OleDbParameter[] parameters = {
					new OleDbParameter("OLEDBID", OleDbType.Integer,4)};
			parameters[0].Value = ID;

			return DbHelperOleDb.Exists(strSql.ToString(),parameters);
		}

        public bool Exists(string sql) {
            return DbHelperOleDb.Exists(sql);
        }


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(DSJL.Model.TB_StandTestRefe model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into TB_StandTestRefe(");
			strSql.Append("StandID,TestID)");
			strSql.Append(" values (");
			strSql.Append("OLEDBStandID,OLEDBTestID)");
			OleDbParameter[] parameters = {
					new OleDbParameter("OLEDBStandID", OleDbType.Integer,4),
					new OleDbParameter("OLEDBTestID", OleDbType.Integer,4)};
			parameters[0].Value = model.StandID;
			parameters[1].Value = model.TestID;

			DbHelperOleDb.ExecuteSql(strSql.ToString(),parameters);
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(DSJL.Model.TB_StandTestRefe model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update TB_StandTestRefe set ");
			strSql.Append("StandID=OLEDBStandID,");
			strSql.Append("TestID=OLEDBTestID");
			strSql.Append(" where ID=OLEDBID");
			OleDbParameter[] parameters = {
					new OleDbParameter("OLEDBStandID", OleDbType.Integer,4),
					new OleDbParameter("OLEDBTestID", OleDbType.Integer,4),
					new OleDbParameter("OLEDBID", OleDbType.Integer,4)};
			parameters[0].Value = model.StandID;
			parameters[1].Value = model.TestID;
			parameters[2].Value = model.ID;

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
			strSql.Append("delete from TB_StandTestRefe ");
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

        public bool Delete(int testInfoID, int standID) {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from TB_StandTestRefe ");
            strSql.Append(" where testid=OLEDBTESTID and standid=OLEDBSTANDID");
            OleDbParameter[] parameters = {
					new OleDbParameter("OLEDBTESTID", OleDbType.Integer,4),
                    new OleDbParameter("OLEDBSTANDID", OleDbType.Integer,4)
};
            parameters[0].Value = testInfoID;
            parameters[1].Value = standID;

            int rows = DbHelperOleDb.ExecuteSql(strSql.ToString(), parameters);
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
			strSql.Append("delete from TB_StandTestRefe ");
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
		public DSJL.Model.TB_StandTestRefe GetModel(int ID)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select ID,StandID,TestID from TB_StandTestRefe ");
			strSql.Append(" where ID=OLEDBID");
			OleDbParameter[] parameters = {
					new OleDbParameter("OLEDBID", OleDbType.Integer,4)
};
			parameters[0].Value = ID;

			DSJL.Model.TB_StandTestRefe model=new DSJL.Model.TB_StandTestRefe();
			DataSet ds=DbHelperOleDb.Query(strSql.ToString(),parameters);
			if(ds.Tables[0].Rows.Count>0)
			{
				if(ds.Tables[0].Rows[0]["ID"].ToString()!="")
				{
					model.ID=int.Parse(ds.Tables[0].Rows[0]["ID"].ToString());
				}
				if(ds.Tables[0].Rows[0]["StandID"].ToString()!="")
				{
					model.StandID=int.Parse(ds.Tables[0].Rows[0]["StandID"].ToString());
				}
				if(ds.Tables[0].Rows[0]["TestID"].ToString()!="")
				{
					model.TestID=int.Parse(ds.Tables[0].Rows[0]["TestID"].ToString());
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
			strSql.Append("select ID,StandID,TestID ");
			strSql.Append(" FROM TB_StandTestRefe ");
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
			parameters[0].Value = "TB_StandTestRefe";
			parameters[1].Value = "ID";
			parameters[2].Value = PageSize;
			parameters[3].Value = PageIndex;
			parameters[4].Value = 0;
			parameters[5].Value = 0;
			parameters[6].Value = strWhere;	
			return DbHelperOleDb.RunProcedure("UP_GetRecordByPage",parameters,"ds");
		}*/

		#endregion  Method

        public DataSet GetStandTestInfo(int standID) {
            //string sql = "select ath.*,test.*,"
            //        + "(select dict_value from tb_dict where dict_groupid=3 and dict_key=test.joint_side) as djointside,"
            //        + "(select dict_value from tb_dict where dict_groupid=1 and dict_key=test.test_mode) as dtestmode,"
            //        + "(select dict_value from tb_dict where dict_groupid=2 and dict_key=test.joint) as djoint,"
            //        + "(select dict_value from tb_dict where dict_groupid=(select id from tb_dict where dict_groupid=2 and dict_key=test.joint) and dict_key=test.plane and instr(dict_groupid2,test.test_mode)>0) as dplane,"
            //        + "(select dict_value from tb_dict where dict_groupid=4 and dict_key=test.InsuredSide) as dInsuredSide,"
            //        + "(select dict_value from tb_dict where dict_groupid=5 and dict_key=test.Gravitycomp) as dGravitycomp "
            //        + "from tb_athleteinfo as ath inner join tb_testinfo as test on ath.id=test.ath_id where test.id in (select testid from tb_standtestrefe as refe where refe.standid=" + standID + ") ";
            string sql = "select ath.*,test.* from tb_athleteinfo as ath,tb_testinfo as test,tb_standtestrefe as refe where ath.id=test.ath_id and test.id=refe.testid and refe.standid="+standID;
            return DbHelperOleDb.Query(sql);
        }
	}
}

