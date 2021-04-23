using Dms.Cms.DatabaseManager;
using Dms.Cms.Messaging;
using Dms.Cms.SystemModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace SmartSUB_Remote_Monitor.Model
{
    public class Stations : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public string stationID { get; set; }
        public string StationID
        {
            get { return stationID; }
            set
            {
                stationID = value;
                PropertyChanged(this, new PropertyChangedEventArgs("StationID"));
            }
        }

        public string numActiveAlarms { get; set; }
        public string NumActiveAlarms
        {
            get { return numActiveAlarms; }
            set
            {
                numActiveAlarms = value;
                PropertyChanged(this, new PropertyChangedEventArgs("NumActiveAlarms"));
            }
        }

        public string dateOfLatestAlarm { get; set; }
        public string DateOfLatestAlarm
        {
            get { return dateOfLatestAlarm; }
            set
            {
                dateOfLatestAlarm = value;
                PropertyChanged(this, new PropertyChangedEventArgs("DateOfLatestAlarm"));
            }
        }

        ObservableCollection<Stations> stations = new ObservableCollection<Stations>();

        public ref ObservableCollection<Stations> GetStations(ISystemInterface systemInterface)
        {
            foreach (int station in GetDistinctStations(systemInterface))
            {
                GetNumActiveAlarmsFromSmartSUB(systemInterface, Convert.ToUInt16(station));
            }
            return ref stations;
        }

        public List<int> GetDistinctStations(ISystemInterface systemInterface)
        {
            List<int> stations = new List<int>();

            var siteIDs = systemInterface.SystemInfo.SiteIds;

            foreach (int i in siteIDs)
            {
                if (i != 0)
                    stations.Add(i);
            }
            return stations;
        }

        public void GetNumActiveAlarmsFromSmartSUB(ISystemInterface systemInterface, ushort stationID)
        {
            SiteID siteID = new SiteID(stationID);

            DatabaseManagerStub databaseManager = new DatabaseManagerStub(systemInterface);

            databaseManager.GetActiveAlarms(siteID, (response, expected) => ExtractNumAlarms(response, expected, stationID, systemInterface));
        }

        public void ExtractNumAlarms(QueryResult response, IEnumerable<AlarmRecord> expected, ushort stationID, ISystemInterface systemInterface)
        {
            int count = 0;
            DateTime latestActiveAlarm = new DateTime(1970, 01, 01);

            if (response == QueryResult.Valid)
            {
                var alarm = expected.GetEnumerator();

                while (alarm.MoveNext())
                {
                    var nodeDefinition = systemInterface.SystemInfo.NodeFromID(alarm.Current.NodeID);
                    NodeType nodeType = nodeDefinition.Type;

                    if (nodeType.ID != Symbol.Intern("CompanyType")
                            && nodeType.ID != Symbol.Intern("RegionType")
                            && nodeType.ID != Symbol.Intern("SubstationType"))
                    {
                        count++;

                        if (latestActiveAlarm < alarm.Current.TimeStamp)
                        {
                            latestActiveAlarm = alarm.Current.TimeStamp;
                        }
                    }
                }

                if (latestActiveAlarm == new DateTime(1970, 01, 01))
                {
                    stations.Add(new Stations()
                    {
                        StationID = "Station: " + stationID,
                        NumActiveAlarms = "Active Alarms: " + count,
                        DateOfLatestAlarm = "No Active Alarms Found."
                    });
                }
                else
                {
                    stations.Add(new Stations()
                    {
                        StationID = "Station: " + stationID,
                        NumActiveAlarms = "Active Alarms: " + count,
                        DateOfLatestAlarm = "Latest Alarm Date: " + latestActiveAlarm
                    });
                }
            }
        }

        public bool UnitTestExample(String Expected)
        {
            if (Expected == "Bob")
            {
                return true;
            }
            return false;
        }
    }
}
