using System;
namespace DSJL.Model
{
	/// <summary>
	/// TB_Setting:ʵ����(����˵���Զ���ȡ���ݿ��ֶε�������Ϣ)
	/// </summary>
	[Serializable]
	public partial class TB_Setting
	{
		public TB_Setting()
		{}
		#region Model
		private int _id;
		private string _setname;
		private string _setvalue;
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
		public string SetName
		{
			set{ _setname=value;}
			get{return _setname;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string SetValue
		{
			set{ _setvalue=value;}
			get{return _setvalue;}
		}
		#endregion Model

	}
}

