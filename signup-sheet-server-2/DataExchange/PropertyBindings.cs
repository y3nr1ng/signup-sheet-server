using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace signup_sheet_server
{
    public class PropertyBindings : INotifyPropertyChanged
    {
        private bool watchdog;
        public bool Watchdog
        {
            get
            {
                return this.watchdog;
            }
            set
            {
                if(this.watchdog == value)
                {
                    return;
                }
                else
                {
                    this.watchdog = value;
                    RaisePropertyChanged("ValueChanged");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
