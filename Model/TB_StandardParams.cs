using System;
namespace DSJL.Model
{
	/// <summary>
	/// TB_StandardParams:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class TB_StandardParams
	{
		public TB_StandardParams()
		{}
		#region Model
		private int _id;
		private int? _standid;
		private string _standname="";
		private string _ath_sex="不限";
		private int? _ath_ageminlimit=-1;
		private int? _ath_agemaxlimit=-1;
		private string _ath_project="不限";
		private string _ath_mainproject="不限";
        private string _joint_side = "-1";
		private string _test_mode="-1";
		private string _joint="-1";
		private string _plane="-1";
		private string _speed1="-1";
		private string _speed2="-1";
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
		public int? StandID
		{
			set{ _standid=value;}
			get{return _standid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string StandName
		{
			set{ _standname=value;}
			get{return _standname;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Ath_Sex
		{
			set{ _ath_sex=value;}
			get{return _ath_sex;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? Ath_AgeMinLimit
		{
			set{ _ath_ageminlimit=value;}
			get{return _ath_ageminlimit;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? Ath_AgeMaxLimit
		{
			set{ _ath_agemaxlimit=value;}
			get{return _ath_agemaxlimit;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Ath_Project
		{
			set{ _ath_project=value;}
			get{return _ath_project;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Ath_MainProject
		{
			set{ _ath_mainproject=value;}
			get{return _ath_mainproject;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Joint_Side
		{
			set{ _joint_side=value;}
			get{return _joint_side;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Test_Mode
		{
			set{ _test_mode=value;}
			get{return _test_mode;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Joint
		{
			set{ _joint=value;}
			get{return _joint;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Plane
		{
			set{ _plane=value;}
			get{return _plane;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Speed1
		{
			set{ _speed1=value;}
			get{return _speed1;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Speed2
		{
			set{ _speed2=value;}
			get{return _speed2;}
		}
		#endregion Model

	}
}

