using System;
namespace DSJL.Model
{
	/// <summary>
	/// TB_AdminInfo:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class TB_AdminInfo
	{
		public TB_AdminInfo()
		{}
		#region Model
		private int _id;
		private string _adminname;
		private string _adminpwd;
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
		public string AdminName
		{
			set{ _adminname=value;}
			get{return _adminname;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string AdminPwd
		{
			set{ _adminpwd=value;}
			get{return _adminpwd;}
		}
		#endregion Model

	}
}

