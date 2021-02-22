using Dms.Cms.Messaging;
using Dms.Cms.SystemModel;
using SmartSUB_Remote_Monitor.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace SmartSUB_Remote_Monitor.ViewModel
{
    class StationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<Stations> stations { get; set; }
        public ObservableCollection<Stations> Stations
        {
            get { return stations; }
            set
            {
                stations = value;
                OnPropertyChanged("Stations");
            }
        }

        SystemInterface _systemInterface;

        public StationViewModel(SystemInterface systemInterface)
        {
            _systemInterface = systemInterface;
             //Stations = new Stations().GetStations(systemInterface);
        }

        public bool GetStations()
        {
            try
            {
                Stations = new Stations().GetStations(_systemInterface);
                return true;
            }
            catch 
            {
                return false;
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
