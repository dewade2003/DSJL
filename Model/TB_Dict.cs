using System;
namespace DSJL.Model
{
	/// <summary>
	/// TB_Dict:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class TB_Dict
	{
		public TB_Dict()
		{}
		#region Model
		private int _id;
		private int _dict_groupid;
		private string _dict_value;
		private string _dict_key;
		private string _actionone;
		private string _actiontwo;
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
		public int Dict_GroupID
		{
			set{ _dict_groupid=value;}
			get{return _dict_groupid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Dict_Value
		{
			set{ _dict_value=value;}
			get{return _dict_value;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Dict_Key
		{
			set{ _dict_key=value;}
			get{return _dict_key;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string actionone
		{
			set{ _actionone=value;}
			get{return _actionone;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string actiontwo
		{
			set{ _actiontwo=value;}
			get{return _actiontwo;}
		}
		#endregion Model

	}
}

