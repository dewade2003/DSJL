using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DSJL.Model
{
    class TestCountModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool isChecked=false;

        public bool IsChecked {
            get {
                return isChecked;
            }
            set {
                isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        public string Count
        {
            get;
            set;
        }
    }
}
