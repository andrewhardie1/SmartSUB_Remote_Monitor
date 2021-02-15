using Dms.Cms.DatabaseManager;
using Dms.Cms.Messaging;
using Dms.Cms.SystemModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace SmartSUB_Remote_Monitor.Model
{
    public class Stations
    {
        public string StationID { get; set; }
        public string NumActiveAlarms { get; set; }
        public string DateOfLatestAlarm { get; set; }

        static readonly Symbol NoRecord = Symbol.Intern("NoRecord");
        static readonly Symbol DBError = Symbol.Intern("DBError");
        static readonly Symbol UnsupportedQuery = Symbol.Intern("UnsupportedQuery");
        static readonly Symbol BadCommand = Symbol.Intern("BadCommand");

        private readonly PropertySetter setter;
        public PropertySetter PropertySetter
        {
            get { return setter; }
        }

        public List<Stations> GetStations(SystemInterface systemInterface)
        {
            List<Stations> stations = new List<Stations>();

            foreach (int x in GetDistinctStations(systemInterface))
            {
                stations.Add(new Stations()
                {
                    StationID = "Station: " + x.ToString(),
                    NumActiveAlarms = "Active Alarms: " + GetNumActiveAlarms(systemInterface, Convert.ToUInt16(x)),
                    DateOfLatestAlarm = "Latest Alarm Date: "
                });
            }

            return stations;
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

        public int GetNumActiveAlarms(SystemInterface systemInterface, ushort stationID)
        {
            Message nodes = new Message();
            Symbol nodeId = Symbol.Intern("NodeID");

            SharedPipe returnPipe = CreateSingleRecordPipe(message => AlarmDataReceived(message, Symbols.AlarmStatus));

            int count = 1;

            SiteID siteID = new SiteID(stationID);

            var allNodes = systemInterface.SystemInfo.NodesBySite(siteID);

            var node = allNodes.GetEnumerator();

            while (node.MoveNext())
            {
                nodes.Append(nodeId, node.Current.ID);
            }

            //system_model::Message alarmStatusRequest(symbols::syGetRecords );
            //alarmStatusRequest << symbols::syNodeID << nodes
            //                   << symbols::syUpdateKind << ISubscriptionHandler::ALARM_UPDATES
            //                   << symbols::syRecord << Message(symbols::syAlarmRecord);

            //Message nodeIDs = new Message();
            //nodeIDs.Append(nodeId, new NodeID(1, 17));

            //Message alarmStatusRequest = new Message(Symbol.Intern("GetRecords"));
            //alarmStatusRequest.Append(nodeId, nodeIDs);
            //alarmStatusRequest.Append(Symbol.Intern("UpdateKind"), SubscriptionKind.AlarmState);
            //alarmStatusRequest.Append(Symbol.Intern("Record"), Symbol.Intern("AlarmRecord"));


            //// Send off the command.
            ////systemInterface_.NodeController().SendNodeCommand(
            ////                systemInterface_.SystemInfo().NodeIDFromLogicalName(_T("DatabaseManager")),
            ////                util::DateTime::Now(),
            ////                alarmStatusRequest,
            ////                owner_);

            //systemInterface.NodeController.SendNodeCommand(
            //    systemInterface.SystemInfo.NodeIDFromLogicalName("DatabaseManager"),
            //    systemInterface.SystemDateTime.UtcNow,
            //    alarmStatusRequest,
            //    returnPipe);

            DatabaseManagerStub databaseManager = new DatabaseManagerStub(systemInterface);

            Action<QueryResult, IEnumerable<AlarmRecord>> action = (r, e) => 
            {
                if (r == QueryResult.Valid) 
                { 
                }
            };

            databaseManager.GetActiveAlarms(siteID, action);

            return count;
        }

        public SharedPipe CreateSingleRecordPipe(Action<Message> callback)
        {
            var callbackPipe = new SingleRecordPipe(callback);

            return new SharedPipe(callbackPipe.MessageReceived, callbackPipe.PipeDropped);
        }

        private class SingleRecordPipe
        {
            private Action<Message> callback;

            public SingleRecordPipe(Action<Message> callback)
            {
                this.callback = callback;
            }

            public bool MessageReceived(Message message)
            {
                Action<Message> callback;

                lock (this)
                {
                    if (this.callback == null) return true;
                    callback = this.callback;
                    this.callback = null;
                }

                try
                {
                    callback(message);
                }
                catch (Exception ex)
                {
                    throw ex;
                };

                return true;
            }

            public void PipeDropped(object sender, EventArgs e)
            {
                Action<Message> callback;

                lock (this)
                {
                    if (this.callback == null) return;
                    callback = this.callback;
                    this.callback = null;
                }

                callback(new Message(Symbols.Error));
            }
        }

        private bool AlarmDataReceived(Message response, Symbol expected)
        {
            QueryResult result = GetResultCode(response, expected);

            if (result == QueryResult.Valid)
            {
                //List<T> entries = new List<T>();

                foreach (var field in response)
                {
                    Message entry = field.Value as Message;
                    if (entry == null) continue;

                    //entries.Add(PropertySetter.SetProperties(new T(), entry));
                }

                //callback(entries);
                return true;
            }
            else
            {
                //callback(null);

                return false;
            }
        }

        private static QueryResult GetResultCode(Message response, Symbol expected)
        {
            if (response != null)
            {
                if (response.Name == DBError)
                    return QueryResult.DatabaseError;

                if (response.Name == BadCommand)
                    return QueryResult.DatabaseError;

                if (response.Name == UnsupportedQuery)
                    return QueryResult.UnsupportedQuery;

                if (response.Name == NoRecord)
                    return QueryResult.NoRecord;

                if (response.Name == expected)
                    return QueryResult.Valid;
            }

            return QueryResult.DatabaseError;
        }

        public static string GetLatestAlarmDate(int StationID)
        {
            string date;
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<AlarmData>();

                var db = conn.Table<AlarmData>();
                var result = db.Where(s => s.SiteID == StationID).OrderByDescending(d => d.E_Date).FirstOrDefault();
                date = result.E_Date.ToString();
            }
            return date;
        }

        //public static int GetNumActiveAlarms(int StationID)
        //{
        //    int count;
        //    using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
        //    {
        //        conn.CreateTable<AlarmData>();

        //        var db = conn.Table<AlarmData>();
        //        count = db.Where(s => s.SiteID == StationID).Count(alarms => alarms.AlarmRead == 2);
        //    }
        //    return count;
        //}
    }
}
