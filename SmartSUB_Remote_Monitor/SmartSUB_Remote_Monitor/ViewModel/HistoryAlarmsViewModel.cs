using Dms.Cms.SystemModel;
using SmartSUB_Remote_Monitor.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SmartSUB_Remote_Monitor.ViewModel
{
    class HistoryAlarmsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<Alarms> historicAlarms { get; set; }
        public ObservableCollection<Alarms> HistoricAlarms
        {
            get { return historicAlarms; }
            set
            {
                historicAlarms = value;
                OnPropertyChanged("HistoricAlarms");
            }
        }

        SystemInterface _systemInterface;

        public HistoryAlarmsViewModel(SystemInterface systemInterface)
        {
            _systemInterface = systemInterface;
        }

        public bool GetHistoricAlarms()
        {
            try
            {
                HistoricAlarms = new Alarms().GetHistoricAlarmsFromSmartSUB(_systemInterface, Convert.ToUInt16(App.stationSelected));
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
