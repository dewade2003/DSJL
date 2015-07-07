using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace DSJL.Model
{
    public class BaseModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string value)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(value));
            }
        }

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
                NotifyPropertyChanged("IsChecked");
            }
            get { return _isChecked; }
        }
    }
}
