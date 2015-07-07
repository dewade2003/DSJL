using System;
using System.ComponentModel;
namespace DSJL.Model
{
	/// <summary>
	/// TB_AthleteInfo:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
    public partial class TB_AthleteInfo : INotifyPropertyChanged
	{
		public TB_AthleteInfo()
		{}

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

		#region Model
		private int _id;
		private string _ath_code;
		private string _ath_name;
        private string _ath_pinyin = "";
		private string _ath_sex="男";
		private DateTime? _ath_birthday=DateTime.Now;
		private string _ath_height="";
		private string _ath_weight="";
		private string _ath_project="";
		private string _ath_mainproject="";
		private string _ath_trainyears="";
		private string _ath_level="";
		private string _ath_team="";
		private DateTime _ath_testdate=DateTime.Now;
		private string _ath_testaddress="";
		private string _ath_testmachine="";
		private string _ath_teststate="";
		private string _ath_remark="";
		private int _ath_testid;
        private string hidden = "0";

        public int Index
        {
            get;
            set;
        }

        private bool _isChecked = false;

        public bool IsChecked
        {
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
            get { return _isChecked; }
        }
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
		public string Ath_Code
		{
			set{ _ath_code=value;}
			get{return _ath_code;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Ath_Name
		{
			set{ _ath_name=value;}
			get{return _ath_name;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Ath_PinYin
		{
			set{ _ath_pinyin=value;}
			get{return _ath_pinyin;}
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
		public DateTime? Ath_Birthday
		{
			set{ _ath_birthday=value;}
			get{return _ath_birthday;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Ath_Height
		{
			set{ _ath_height=value;}
			get{return _ath_height;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Ath_Weight
		{
			set{ _ath_weight=value;}
			get{return _ath_weight;}
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
		public string Ath_TrainYears
		{
			set{ _ath_trainyears=value;}
			get{return _ath_trainyears;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Ath_Level
		{
			set{ _ath_level=value;}
			get{return _ath_level;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Ath_Team
		{
			set{ _ath_team=value;}
			get{return _ath_team;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime Ath_TestDate
		{
			set{ _ath_testdate=value;}
			get{return _ath_testdate;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Ath_TestAddress
		{
			set{ _ath_testaddress=value;}
			get{return _ath_testaddress;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Ath_TestMachine
		{
			set{ _ath_testmachine=value;}
			get{return _ath_testmachine;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Ath_TestState
		{
			set{ _ath_teststate=value;}
			get{return _ath_teststate;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Ath_Remark
		{
			set{ _ath_remark=value;}
			get{return _ath_remark;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int Ath_TestID
		{
			set{ _ath_testid=value;}
			get{return _ath_testid;}
		}

        public string Hidden {
            get { return hidden; }
            set {
                hidden = value;
            }
        }
		#endregion Model

	}
}

