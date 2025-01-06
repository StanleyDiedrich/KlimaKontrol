using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlimaKontrol
{
    public  class Areas : INotifyPropertyChanged
    {
        private string areaName { get; set; }  
        private bool isSelected { get; set; }

        
        public string AreaName
        {
            get { return areaName; }
            set
            {
                areaName = value;
                OnPropertyChanged(nameof(AreaName));
            }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }


        public Areas (string areaname)
        {
            AreaName = areaname;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
