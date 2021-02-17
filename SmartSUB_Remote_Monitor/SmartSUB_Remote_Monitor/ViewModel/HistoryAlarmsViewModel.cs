using Dms.Cms.SystemModel;
using SmartSUB_Remote_Monitor.Model;
using System;
using System.Collections.Generic;

namespace SmartSUB_Remote_Monitor.ViewModel
{
    class HistoryAlarmsViewModel
    {
        public List<Alarms> HistoricAlarms { get; set; }
        public HistoryAlarmsViewModel(SystemInterface systemInterface)
        {
            HistoricAlarms = new Alarms().GetHistoricAlarmsFromSmartSUB(systemInterface, Convert.ToUInt16(App.stationSelected));
        }
    }
}
