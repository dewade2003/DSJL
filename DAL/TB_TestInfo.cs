using System;
using System.Data;
using System.Text;
using System.Data.OleDb;
using DSJL.DBUtility;//Please add references
namespace DSJL.DAL
{
	/// <summary>
	/// 数据访问类:TB_TestInfo
	/// </summary>
	public partial class TB_TestInfo
	{
		public TB_TestInfo()
		{}
		#region  Method

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
		return DbHelperOleDb.GetMaxID("ID", "TB_TestInfo"); 
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int ID)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from TB_TestInfo");
			strSql.Append(" where ID=OLEDBID ");
			OleDbParameter[] parameters = {
					new OleDbParameter("OLEDBID", OleDbType.Integer,4)};
			parameters[0].Value = ID;

			return DbHelperOleDb.Exists(strSql.ToString(),parameters);
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public void Add(DSJL.Model.TB_TestInfo model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into TB_TestInfo(");
			strSql.Append("Ath_ID,TestDate,TestTime,Joint_Side,Test_Mode,Joint,Plane,Motion_Start,Motion_End,Speed1,Speed2,Acceleration1,Acceleration2,Break,NOOfSets,NOOfRepetitions,InsuredSide,Gravitycomp,Therapist,BaseFileName,DataFileName,Remark,ActionID)");
			strSql.Append(" values (");
			strSql.Append("OLEDBAth_ID,OLEDBTestDate,OLEDBTestTime,OLEDBJoint_Side,OLEDBTest_Mode,OLEDBJoint,OLEDBPlane,OLEDBMotion_Start,OLEDBMotion_End,OLEDBSpeed1,OLEDBSpeed2,OLEDBAcceleration1,OLEDBAcceleration2,OLEDBBreak,OLEDBNOOfSets,OLEDBNOOfRepetitions,OLEDBInsuredSide,OLEDBGravitycomp,OLEDBTherapist,OLEDBBaseFileName,OLEDBDataFileName,OLEDBRemark,OLEDBActionID)");
			OleDbParameter[] parameters = {
					new OleDbParameter("OLEDBAth_ID", OleDbType.Integer,4),
					new OleDbParameter("OLEDBTestDate", OleDbType.Date),
					new OleDbParameter("OLEDBTestTime", OleDbType.Date),
					new OleDbParameter("OLEDBJoint_Side", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBTest_Mode", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBJoint", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBPlane", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBMotion_Start", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBMotion_End", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBSpeed1", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBSpeed2", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAcceleration1", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAcceleration2", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBBreak", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBNOOfSets", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBNOOfRepetitions", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBInsuredSide", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBGravitycomp", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBTherapist", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBBaseFileName", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBDataFileName", OleDbType.VarChar,255),
                    new OleDbParameter("OLEDBRemark", OleDbType.VarChar,255),
                    new OleDbParameter("OLEDBActionID", OleDbType.VarChar,255)};
			parameters[0].Value = model.Ath_ID;
			parameters[1].Value = model.TestDate;
			parameters[2].Value = model.TestTime;
			parameters[3].Value = model.Joint_Side;
			parameters[4].Value = model.Test_Mode;
			parameters[5].Value = model.Joint;
			parameters[6].Value = model.Plane;
			parameters[7].Value = model.Motion_Start;
			parameters[8].Value = model.Motion_End;
			parameters[9].Value = model.Speed1;
			parameters[10].Value = model.Speed2;
			parameters[11].Value = model.Acceleration1;
			parameters[12].Value = model.Acceleration2;
			parameters[13].Value = model.Break;
			parameters[14].Value = model.NOOfSets;
			parameters[15].Value = model.NOOfRepetitions;
			parameters[16].Value = model.InsuredSide;
			parameters[17].Value = model.Gravitycomp;
			parameters[18].Value = model.Therapist;
			parameters[19].Value = model.BaseFileName;
			parameters[20].Value = model.DataFileName;
            parameters[21].Value = model.Remark;
            parameters[22].Value = model.ActionID;

			DbHelperOleDb.ExecuteSql(strSql.ToString(),parameters);
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(DSJL.Model.TB_TestInfo model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update TB_TestInfo set ");
			strSql.Append("Ath_ID=OLEDBAth_ID,");
			strSql.Append("TestDate=OLEDBTestDate,");
			strSql.Append("TestTime=OLEDBTestTime,");
			strSql.Append("Joint_Side=OLEDBJoint_Side,");
			strSql.Append("Test_Mode=OLEDBTest_Mode,");
			strSql.Append("Joint=OLEDBJoint,");
			strSql.Append("Plane=OLEDBPlane,");
			strSql.Append("Motion_Start=OLEDBMotion_Start,");
			strSql.Append("Motion_End=OLEDBMotion_End,");
			strSql.Append("Speed1=OLEDBSpeed1,");
			strSql.Append("Speed2=OLEDBSpeed2,");
			strSql.Append("Acceleration1=OLEDBAcceleration1,");
			strSql.Append("Acceleration2=OLEDBAcceleration2,");
			strSql.Append("Break=OLEDBBreak,");
			strSql.Append("NOOfSets=OLEDBNOOfSets,");
			strSql.Append("NOOfRepetitions=OLEDBNOOfRepetitions,");
			strSql.Append("InsuredSide=OLEDBInsuredSide,");
			strSql.Append("Gravitycomp=OLEDBGravitycomp,");
			strSql.Append("Therapist=OLEDBTherapist,");
			strSql.Append("BaseFileName=OLEDBBaseFileName,");
			strSql.Append("DataFileName=OLEDBDataFileName");
			strSql.Append(" where ID=OLEDBID");
			OleDbParameter[] parameters = {
					new OleDbParameter("OLEDBAth_ID", OleDbType.Integer,4),
					new OleDbParameter("OLEDBTestDate", OleDbType.Date),
					new OleDbParameter("OLEDBTestTime", OleDbType.Date),
					new OleDbParameter("OLEDBJoint_Side", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBTest_Mode", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBJoint", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBPlane", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBMotion_Start", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBMotion_End", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBSpeed1", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBSpeed2", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAcceleration1", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBAcceleration2", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBBreak", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBNOOfSets", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBNOOfRepetitions", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBInsuredSide", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBGravitycomp", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBTherapist", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBBaseFileName", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBDataFileName", OleDbType.VarChar,255),
					new OleDbParameter("OLEDBID", OleDbType.Integer,4)};
			parameters[0].Value = model.Ath_ID;
			parameters[1].Value = model.TestDate;
			parameters[2].Value = model.TestTime;
			parameters[3].Value = model.Joint_Side;
			parameters[4].Value = model.Test_Mode;
			parameters[5].Value = model.Joint;
			parameters[6].Value = model.Plane;
			parameters[7].Value = model.Motion_Start;
			parameters[8].Value = model.Motion_End;
			parameters[9].Value = model.Speed1;
			parameters[10].Value = model.Speed2;
			parameters[11].Value = model.Acceleration1;
			parameters[12].Value = model.Acceleration2;
			parameters[13].Value = model.Break;
			parameters[14].Value = model.NOOfSets;
			parameters[15].Value = model.NOOfRepetitions;
			parameters[16].Value = model.InsuredSide;
			parameters[17].Value = model.Gravitycomp;
			parameters[18].Value = model.Therapist;
			parameters[19].Value = model.BaseFileName;
			parameters[20].Value = model.DataFileName;
			parameters[21].Value = model.ID;

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
			strSql.Append("delete from TB_TestInfo ");
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
			strSql.Append("delete from TB_TestInfo ");
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
		public DSJL.Model.TB_TestInfo GetModel(int ID)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select ID,Ath_ID,TestDate,TestTime,Joint_Side,Test_Mode,Joint,Plane,Motion_Start,Motion_End,Speed1,Speed2,Acceleration1,Acceleration2,Break,NOOfSets,NOOfRepetitions,InsuredSide,Gravitycomp,Therapist,BaseFileName,DataFileName,Remark,ActionID from TB_TestInfo ");
			strSql.Append(" where ID=OLEDBID");
			OleDbParameter[] parameters = {
					new OleDbParameter("OLEDBID", OleDbType.Integer,4)
};
			parameters[0].Value = ID;

			DSJL.Model.TB_TestInfo model=new DSJL.Model.TB_TestInfo();
			DataSet ds=DbHelperOleDb.Query(strSql.ToString(),parameters);
			if(ds.Tables[0].Rows.Count>0)
			{
				if(ds.Tables[0].Rows[0]["ID"].ToString()!="")
				{
					model.ID=int.Parse(ds.Tables[0].Rows[0]["ID"].ToString());
				}
				if(ds.Tables[0].Rows[0]["Ath_ID"].ToString()!="")
				{
					model.Ath_ID=int.Parse(ds.Tables[0].Rows[0]["Ath_ID"].ToString());
				}
				if(ds.Tables[0].Rows[0]["TestDate"].ToString()!="")
				{
					model.TestDate=DateTime.Parse(ds.Tables[0].Rows[0]["TestDate"].ToString());
				}
				if(ds.Tables[0].Rows[0]["TestTime"].ToString()!="")
				{
					model.TestTime=DateTime.Parse(ds.Tables[0].Rows[0]["TestTime"].ToString());
				}
				model.Joint_Side=ds.Tables[0].Rows[0]["Joint_Side"].ToString();
				model.Test_Mode=ds.Tables[0].Rows[0]["Test_Mode"].ToString();
				model.Joint=ds.Tables[0].Rows[0]["Joint"].ToString();
				model.Plane=ds.Tables[0].Rows[0]["Plane"].ToString();
				model.Motion_Start=ds.Tables[0].Rows[0]["Motion_Start"].ToString();
				model.Motion_End=ds.Tables[0].Rows[0]["Motion_End"].ToString();
				model.Speed1=ds.Tables[0].Rows[0]["Speed1"].ToString();
				model.Speed2=ds.Tables[0].Rows[0]["Speed2"].ToString();
				model.Acceleration1=ds.Tables[0].Rows[0]["Acceleration1"].ToString();
				model.Acceleration2=ds.Tables[0].Rows[0]["Acceleration2"].ToString();
				model.Break=ds.Tables[0].Rows[0]["Break"].ToString();
				model.NOOfSets=ds.Tables[0].Rows[0]["NOOfSets"].ToString();
				model.NOOfRepetitions=ds.Tables[0].Rows[0]["NOOfRepetitions"].ToString();
				model.InsuredSide=ds.Tables[0].Rows[0]["InsuredSide"].ToString();
				model.Gravitycomp=ds.Tables[0].Rows[0]["Gravitycomp"].ToString();
				model.Therapist=ds.Tables[0].Rows[0]["Therapist"].ToString();
				model.BaseFileName=ds.Tables[0].Rows[0]["BaseFileName"].ToString();
				model.DataFileName=ds.Tables[0].Rows[0]["DataFileName"].ToString();

                model.Remark = ds.Tables[0].Rows[0]["Remark"].ToString();
                model.ActionID = ds.Tables[0].Rows[0]["ActionID"].ToString();
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
			strSql.Append("select ID,Ath_ID,TestDate,TestTime,Joint_Side,Test_Mode,Joint,Plane,Motion_Start,Motion_End,Speed1,Speed2,Acceleration1,Acceleration2,Break,NOOfSets,NOOfRepetitions,InsuredSide,Gravitycomp,Therapist,BaseFileName,DataFileName,Remark,ActionID ");
			strSql.Append(" FROM TB_TestInfo ");
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
			parameters[0].Value = "TB_TestInfo";
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

