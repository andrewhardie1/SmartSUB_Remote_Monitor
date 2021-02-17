using Dms.Cms.SystemModel;
using SmartSUB_Remote_Monitor.Model;
using System;
using System.Collections.Generic;

namespace SmartSUB_Remote_Monitor.ViewModel
{
    class ActiveAlarmsViewModel
    {
        public List<Alarms> ActiveAlarms { get; set; }
        public ActiveAlarmsViewModel(SystemInterface systemInterface)
        {
            ActiveAlarms = new Alarms().GetActiveAlarmsFromSmartSUB(systemInterface, Convert.ToUInt16(App.stationSelected));
        }
    }
}
