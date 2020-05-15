using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartbeatApp.Classes
{
    public partial class mDatePicker : INotifyPropertyChanged
    {
        private DateTime _date;

        public DateTime Date
        {
            get { return _date; }
            set { _date = value; NotifyPropertyChanged("Date"); }
        }

        public mDatePicker()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
