using Dms.Cms.SystemModel;
using SmartSUB_Remote_Monitor.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSUB_Remote_Monitor.ViewModel
{
    class SystemLogViewModel
    {
        public List<SystemLog> SystemLog { get; set; }
        public SystemLogViewModel(SystemInterface systemInterface)
        {
            SystemLog = new SystemLog().GetLogs(systemInterface);
        }
    }
}
