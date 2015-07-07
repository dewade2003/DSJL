using System;
namespace DSJL.Model
{
	/// <summary>
	/// TB_StandTestRefe:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class TB_StandTestRefe
	{
		public TB_StandTestRefe()
		{}
		#region Model
		private int _id;
		private int _standid;
		private int _testid;
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
		public int StandID
		{
			set{ _standid=value;}
			get{return _standid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int TestID
		{
			set{ _testid=value;}
			get{return _testid;}
		}
		#endregion Model

	}
}

