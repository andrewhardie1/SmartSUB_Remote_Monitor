using Dms.Cms.DatabaseManager;
using Dms.Cms.Messaging;
using Dms.Cms.SystemModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartSUB_Remote_Monitor.Model
{
    class SystemLog
    {
        public DateTime Timestamp { get; set; }
        public Symbol LogicalName { get; set; }
        public Message SystemLogMessage { get; set; }

        List<SystemLog> systemLogs = new List<SystemLog>();

        public ref List<SystemLog> GetLogs(SystemInterface systemInterface)
        {
            bool allSites = true;
            var sites = new HashSet<NodeID>();
            IEnumerable<NodeID> nodeIDs;

            if (allSites)
            {
                sites.UnionWith(
                  nodeIDs = systemInterface.SystemInfo.SiteIds.Select(site => NodeID.AllNodesInSite(new SiteID(site))));
            }
            else
            {
                SiteID siteZero = new SiteID(0);
                nodeIDs = new[] { NodeID.AllNodesInSite(siteZero) };
            }

            DatabaseManagerStub databaseManager = new DatabaseManagerStub(systemInterface);
            databaseManager.GetSystemLogRecords(nodeIDs, (response, expected) => ExtractSystemLogs(response, expected));

            return ref systemLogs;
        }

        private void ExtractSystemLogs(QueryResult response, IEnumerable<SystemLogRecord> expected)
        {
            int count = 0;

            if (response == QueryResult.Valid)
            {
                var logs = expected.GetEnumerator();

                while (logs.MoveNext())
                {
                    if (count < 1)
                    {

                        NodeID nodeID = logs.Current.NodeID;
                        Symbol name = logs.Current.LogType;
                        DateTime time = logs.Current.TimeStamp;
                        var rdr = new TextMessageReader(logs.Current.LogProperties.ToString());
                        Message logMessage = rdr.NextMessage();

                        SystemLogRecord systemLogRecord = new SystemLogRecord(nodeID, name, time, logMessage);

                        systemLogs.Add(new SystemLog()
                        {
                            Timestamp = time,
                            LogicalName = name,
                            SystemLogMessage = logMessage
                        });
                    }
                    else
                    {
                        break;
                    }
                    count++;
                }
            }
        }

        //public static string GetLocalisedDetailedText(SystemLogRecord logRecord)
        //{
        //    ILocalisationTable localisation = MainWindowViewModel.Instance.Localisation;
        //    return LocalisationDetailsHelpers.DetailText(
        //                false,
        //                logRecord.LogType,
        //                logRecord.NodeID,
        //                logRecord.LogProperties,
        //                localisation,
        //                GlobalState.SystemInterface.SystemInfo);
        //}
    }
}
