using System;
namespace DSJL.Model
{
	/// <summary>
	/// TB_DBVersion:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class TB_DBVersion
	{
		public TB_DBVersion()
		{}
		#region Model
		private int _id;
		private int? _versioncode;
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
		public int? versioncode
		{
			set{ _versioncode=value;}
			get{return _versioncode;}
		}
		#endregion Model

	}
}

