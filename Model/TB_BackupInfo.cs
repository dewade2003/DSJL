using System;
using System.ComponentModel;
namespace DSJL.Model
{
	/// <summary>
	/// TB_BackupInfo:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
    public partial class TB_BackupInfo : INotifyPropertyChanged
	{
		public TB_BackupInfo()
		{}

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

		#region Model
		private int _id;
		private string _backupname;
		private string _backuppath;
		private string _backupdate;
        private string _index;

        public string Index
        {
            set { _index = value; }
            get { return _index; }
        }
        private bool _isChecked=false;

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
		public string BackupName
		{
			set{ _backupname=value;}
			get{return _backupname;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string BackupPath
		{
			set{ _backuppath=value;}
			get{return _backuppath;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string BackupDate
		{
			set{ _backupdate=value;}
			get{return _backupdate;}
		}


       
		#endregion Model

	}
}

