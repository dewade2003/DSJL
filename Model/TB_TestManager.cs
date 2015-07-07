using System;
namespace DSJL.Model
{
	/// <summary>
	/// TB_TestManager:ʵ����(����˵���Զ���ȡ���ݿ��ֶε�������Ϣ)
	/// </summary>
	[Serializable]
	public partial class TB_TestManager
	{
		public TB_TestManager()
		{}
		#region Model
		private int _id;
		private string _testname;
		private string _teststartdate;
		private string _testenddate;
		private string _remark="";
		/// <summary>
		/// 
		/// </summary>
		public int ID
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string TestName
		{
			set{ _testname=value;}
			get{return _testname;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string TestStartDate
		{
			set{ _teststartdate=value;}
			get{return _teststartdate;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string TestEndDate
		{
			set{ _testenddate=value;}
			get{return _testenddate;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Remark
		{
			set{ _remark=value;}
			get{return _remark;}
		}
		#endregion Model

	}
}

