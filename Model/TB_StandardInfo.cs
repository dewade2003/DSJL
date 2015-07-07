using System;
namespace DSJL.Model
{
	/// <summary>
	/// TB_StandardInfo:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class TB_StandardInfo: BaseModel
	{
		public TB_StandardInfo()
		{}
		#region Model
		private int _id;
		private string _stand_name;
		private string _stand_date=DateTime.Now.ToString("yyyy-MM-ddd");
        private string _stand_remark = "";
		private int? _stand_level;
		private int? _stand_parentid;
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
		public string Stand_Name
		{
            set { _stand_name = value; NotifyPropertyChanged("Stand_Name"); }
			get{return _stand_name;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Stand_Date
		{
			set{ _stand_date=value;}
			get{return _stand_date;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Stand_Remark
		{
			set{ _stand_remark=value;}
			get{return _stand_remark;}
		}
		/// <summary>
		/// 级别
		/// </summary>
		public int? Stand_Level
		{
			set{ _stand_level=value;}
			get{return _stand_level;}
		}
		/// <summary>
		/// 上级ID
		/// </summary>
		public int? Stand_ParentID
		{
			set{ _stand_parentid=value;}
			get{return _stand_parentid;}
		}
		#endregion Model

	}
}

