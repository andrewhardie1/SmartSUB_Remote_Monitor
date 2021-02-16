using Dms.Cms.DatabaseManager;
using Dms.Cms.SystemModel;
using SQLite;
using System;
using System.Collections.Generic;

namespace SmartSUB_Remote_Monitor.Model
{
    public class Stations
    {
        public string StationID { get; set; }
        public string NumActiveAlarms { get; set; }
        public string DateOfLatestAlarm { get; set; }

        List<Stations> stations = new List<Stations>();

        public ref List<Stations> GetStations(SystemInterface systemInterface)
        {
            foreach (int station in GetDistinctStations(systemInterface))
            {
                GetNumActiveAlarmsFromSmartSUB(systemInterface, Convert.ToUInt16(station));
            }
            return ref stations;
        }

        public static List<int> GetDistinctStations(SystemInterface systemInterface)
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

        public void GetNumActiveAlarmsFromSmartSUB(SystemInterface systemInterface, ushort stationID)
        {
            SiteID siteID = new SiteID(stationID);
            DatabaseManagerStub databaseManager = new DatabaseManagerStub(systemInterface);

            databaseManager.GetActiveAlarms(siteID, (response, expected) => ExtractNumAlarms(response, expected, stationID, systemInterface));
        }

        private void ExtractNumAlarms(QueryResult response, IEnumerable<AlarmRecord> expected, ushort stationID, SystemInterface systemInterface)
        {
            int count = 0;
            DateTime latestActiveAlarm = new DateTime(1970, 01, 01);

            if (response == QueryResult.Valid)
            {
                var enumerator = expected.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    count++;

                    if (latestActiveAlarm < enumerator.Current.TimeStamp)
                    {
                        latestActiveAlarm = enumerator.Current.TimeStamp;
                    }
                }

                stations.Add(new Stations()
                {
                    StationID = "Station: " + stationID,
                    NumActiveAlarms = "Active Alarms: " + count,
                    DateOfLatestAlarm = "Latest Alarm Date: " + latestActiveAlarm
                });
            }
        }
    }
}
