using Dms.Cms.DatabaseManager;
using Dms.Cms.Messaging;
using Dms.Cms.SystemModel;
using System;
using System.Collections.Generic;

namespace SmartSUB_Remote_Monitor.Model
{
    class Alarms
    {
        public string NodeID { get; set; }
        public string NodeName { get; set; }
        public string AlarmType { get; set; }
        public DateTime Timestamp { get; set; }

        List<Alarms> allActiveAlarms = new List<Alarms>();
        List<Alarms> allHistoricAlarms = new List<Alarms>();

        public ref List<Alarms> GetActiveAlarmsFromSmartSUB(SystemInterface systemInterface, ushort stationID)
        {
            SiteID siteID = new SiteID(stationID);
            DatabaseManagerStub databaseManager = new DatabaseManagerStub(systemInterface);


            TimeSpan lookback;
            lookback = TimeSpan.FromDays(1);

            databaseManager.GetAlarmRecords(siteID, DateTime.UtcNow - lookback, DateTime.UtcNow,
                (response, expected) => ExtractActiveAlarms(response, expected, systemInterface));

            return ref allActiveAlarms;
        }

        private void ExtractActiveAlarms(QueryResult response, IEnumerable<AlarmRecord> expected, SystemInterface systemInterface)
        {
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
                        if (alarm.Current.IsSet)
                        {
                            NodeID nodeIDRaw = alarm.Current.NodeID;
                            string nodeLogicalName = systemInterface.SystemInfo.NodeFromID(nodeIDRaw).ToString();

                            string nodeID = nodeIDRaw.ToString();
                            nodeID = nodeID.Remove(0, 9);
                            char[] charsToRemove = { ':', ')', '.' };
                            nodeID = nodeID.Trim(charsToRemove);

                            nodeLogicalName = nodeLogicalName.Substring(nodeLogicalName.LastIndexOf("."));
                            nodeLogicalName = nodeLogicalName.Remove(nodeLogicalName.IndexOf(" ") + 1);
                            nodeLogicalName = nodeLogicalName.Trim(charsToRemove);

                            allActiveAlarms.Add(new Alarms()
                            {
                                NodeID = nodeID,
                                NodeName = nodeLogicalName,
                                AlarmType = "",
                                Timestamp = alarm.Current.TimeStamp
                            });
                        }
                    }
                }
            }
        }

        public ref List<Alarms> GetHistoricAlarmsFromSmartSUB(SystemInterface systemInterface, ushort stationID)
        {
            SiteID siteID = new SiteID(stationID);
            DatabaseManagerStub databaseManager = new DatabaseManagerStub(systemInterface);


            TimeSpan lookback;
            lookback = TimeSpan.FromDays(1);

            databaseManager.GetAlarmRecords(siteID, DateTime.UtcNow - lookback, DateTime.UtcNow,
                (response, expected) => ExtractHistoricAlarms(response, expected, systemInterface));

            return ref allHistoricAlarms;
        }

        private void ExtractHistoricAlarms(QueryResult response, IEnumerable<AlarmRecord> expected, SystemInterface systemInterface)
        {
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
                        if (!alarm.Current.IsSet)
                        {
                            NodeID nodeIDRaw = alarm.Current.NodeID;
                            string nodeLogicalName = systemInterface.SystemInfo.NodeFromID(nodeIDRaw).ToString();

                            string nodeID = nodeIDRaw.ToString();
                            nodeID = nodeID.Remove(0, 9);
                            char[] charsToRemove = { ':', ')', '.' };
                            nodeID = nodeID.Trim(charsToRemove);

                            nodeLogicalName = nodeLogicalName.Substring(nodeLogicalName.LastIndexOf("."));
                            nodeLogicalName = nodeLogicalName.Remove(nodeLogicalName.IndexOf(" ") + 1);
                            nodeLogicalName = nodeLogicalName.Trim(charsToRemove);

                            allHistoricAlarms.Add(new Alarms()
                            {
                                NodeID = nodeID,
                                NodeName = nodeLogicalName,
                                AlarmType = "",
                                Timestamp = alarm.Current.TimeStamp
                            });
                        }
                    }
                }

            }
        }
    }
}
