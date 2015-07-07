using System;
namespace DSJL.Model
{
	/// <summary>
	/// TB_TestInfo:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class TB_TestInfo
	{
		public TB_TestInfo()
		{}
		#region Model
		private int _id;
		private int _ath_id;
		private DateTime _testdate;
		private DateTime _testtime;
		private string _joint_side;
		private string _test_mode;
		private string _joint;
		private string _plane;
		private string _motion_start;
		private string _motion_end;
		private string _speed1;
		private string _speed2;
		private string _acceleration1;
		private string _acceleration2;
		private string _break;
		private string _noofsets;
		private string _noofrepetitions;
		private string _insuredside;
		private string _gravitycomp;
		private string _therapist;
		private string _basefilename;
		private string _datafilename;
        private string _remark="";
        private string _actionID="";
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
		public int Ath_ID
		{
			set{ _ath_id=value;}
			get{return _ath_id;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime TestDate
		{
			set{ _testdate=value;}
			get{return _testdate;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime TestTime
		{
			set{ _testtime=value;}
			get{return _testtime;}
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
		public string Motion_Start
		{
			set{ _motion_start=value;}
			get{return _motion_start;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Motion_End
		{
			set{ _motion_end=value;}
			get{return _motion_end;}
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
		/// <summary>
		/// 
		/// </summary>
		public string Acceleration1
		{
			set{ _acceleration1=value;}
			get{return _acceleration1;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Acceleration2
		{
			set{ _acceleration2=value;}
			get{return _acceleration2;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Break
		{
			set{ _break=value;}
			get{return _break;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string NOOfSets
		{
			set{ _noofsets=value;}
			get{return _noofsets;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string NOOfRepetitions
		{
			set{ _noofrepetitions=value;}
			get{return _noofrepetitions;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string InsuredSide
		{
			set{ _insuredside=value;}
			get{return _insuredside;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Gravitycomp
		{
			set{ _gravitycomp=value;}
			get{return _gravitycomp;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Therapist
		{
			set{ _therapist=value;}
			get{return _therapist;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string BaseFileName
		{
			set{ _basefilename=value;}
			get{return _basefilename;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string DataFileName
		{
			set{ _datafilename=value;}
			get{return _datafilename;}
		}

        public string Remark {
            get {
                return _remark;
            }
            set {
                _remark = value;
            }
        }

        public string ActionID {
            get {
                return _actionID;
            }
            set {
                _actionID = value;
            }
        }
		#endregion Model

	}
}

