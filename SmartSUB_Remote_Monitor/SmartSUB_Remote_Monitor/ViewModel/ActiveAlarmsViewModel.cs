using Dms.Cms.SystemModel;
using SmartSUB_Remote_Monitor.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SmartSUB_Remote_Monitor.ViewModel
{
    class ActiveAlarmsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<Alarms> activeAlarms { get; set; }
        public ObservableCollection<Alarms> ActiveAlarms
        {
            get { return activeAlarms; }
            set
            {
                activeAlarms = value;
                OnPropertyChanged("ActiveAlarms");
            }
        }

        SystemInterface _systemInterface;
        public ActiveAlarmsViewModel(SystemInterface systemInterface)
        {
            _systemInterface = systemInterface;
        }

        public bool GetActiveAlarms()
        {
            try
            {
                ActiveAlarms = new Alarms().GetActiveAlarmsFromSmartSUB(_systemInterface, Convert.ToUInt16(App.stationSelected));
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
