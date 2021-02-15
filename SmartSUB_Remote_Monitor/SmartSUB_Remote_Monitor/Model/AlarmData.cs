using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;

namespace SmartSUB_Remote_Monitor.Model
{
    public class AlarmData : INotifyPropertyChanged
    {
        private Int16 nodeID;
        public Int16 NodeID
        {
            get { return nodeID; }
            set
            {
                nodeID = value;
                OnPropertyChanged("nodeID");
            }
        }

        private Int16 siteID;
        public Int16 SiteID
        {
            get { return siteID; }
            set
            {
                siteID = value;
                OnPropertyChanged("SiteID");
            }
        }

        private DateTime e_date;
        public DateTime E_Date
        {
            get { return e_date; }
            set
            {
                e_date = value;
                OnPropertyChanged("SitE_DateeID");
            }
        }

        private string nodeName;
        public string NodeName
        {
            get { return nodeName; }
            set
            {
                nodeName = value;
                OnPropertyChanged("NodeName");
            }
        }

        private int alarmRead;
        public int AlarmRead
        {
            get { return alarmRead; }
            set
            {
                alarmRead = value;
                OnPropertyChanged("AlarmRead");
            }
        }

        private int alarmType;
        public int AlarmType
        {
            get { return alarmType; }
            set
            {
                alarmType = value;
                OnPropertyChanged("AlarmType");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public static List<AlarmData> ReadActiveAlarms(int stationSelected)
        {
            List<AlarmData> alarms;

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<AlarmData>();

                var alarmFalse = from s in conn.Table<AlarmData>()
                                 where s.SiteID.Equals(stationSelected)
                                 where s.AlarmRead.Equals("2")
                                 select s;

                alarms = alarmFalse.ToList();
            }

            return alarms;
        }

        public static List<AlarmData> ReadInctiveAlarms(int stationSelected)
        {
            List<AlarmData> alarms;

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<AlarmData>();

                var alarmFalse = from s in conn.Table<AlarmData>()
                                 where s.SiteID.Equals(stationSelected)
                                 where s.AlarmRead.Equals("1")
                                 select s;

                alarms = alarmFalse.ToList();
            }

            return alarms;
        }

        public static int GetAlarms()
        {
            int result;

            if (App.SmartSUBServerURL == null)
            {
                result = 0;
                return result;
            }
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = App.SmartSUBServerURL; //"192.168.1.137"
            builder.UserID = "sa";
            builder.Password = "Password123";
            builder.InitialCatalog = "SS_Mobile";

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<AlarmData>(); //Create table

                try
                {
                    using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                    {
                        string sqlServerQuery = "SELECT * FROM dat_alarms WHERE nodeid = 4 OR nodeid = 5 OR nodeid = 229";

                        using (SqlCommand command = new SqlCommand(sqlServerQuery, connection))
                        {
                            try
                            {
                                connection.Open(); //Open SQL server connection
                            }
                            catch (SqlException ex)
                            {
                                return result = 3;
                            }

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    AlarmData alarm = new AlarmData()
                                    {
                                        SiteID = reader.GetInt16(0),
                                        NodeID = reader.GetInt16(1),
                                        E_Date = reader.GetDateTime(2),
                                        AlarmType = reader.GetInt32(3),
                                        AlarmRead = reader.GetInt32(5),
                                        NodeName = ""
                                    };

                                    if (reader.GetInt16(1) == 4 && reader.GetInt32(3) == 109)
                                        alarm.nodeName = "Ambient Temp: Alarm";
                                    else if (reader.GetInt16(1) == 4 && reader.GetInt32(3) == 830)
                                        alarm.nodeName = "Ambient Temp: Warning";
                                    else if (reader.GetInt16(1) == 5 && reader.GetInt32(3) == 109)
                                        alarm.nodeName = "DGA C2H2: Alarm";
                                    else if (reader.GetInt16(1) == 5 && reader.GetInt32(3) == 830)
                                        alarm.nodeName = "DGA C2H2: Warning";
                                    else if (reader.GetInt16(1) == 229 && reader.GetInt32(3) == 109)
                                        alarm.nodeName = "Ambient Temp: Alarm";
                                    else if (reader.GetInt16(1) == 229 && reader.GetInt32(3) == 830)
                                        alarm.nodeName = "Ambient Temp: Warning";
                                    else if (reader.GetInt16(1) == 230 && reader.GetInt32(3) == 109)
                                        alarm.nodeName = "DGA C2H2: Alarm";
                                    else if (reader.GetInt16(1) == 230 && reader.GetInt32(3) == 830)
                                        alarm.nodeName = "DGA C2H2: Warning";
                                    else
                                        return result = 2;

                                    if (conn.Table<AlarmData>().Where(u => (u.E_Date == alarm.E_Date)
                                    && (u.NodeID == alarm.NodeID)
                                    && (u.AlarmType == alarm.AlarmType)
                                    && (u.AlarmRead != alarm.AlarmRead)).Any())
                                    {
                                        //var updateSQLiteDB = conn.Query<AlarmData>("UPDATE AlarmData SET AlarmRead='" + alarm.AlarmRead + "' where E_Date = '" + alarm.E_Date + "'").FirstOrDefault();
                                        var updateSQLiteDB = conn.Query<AlarmData>("UPDATE AlarmData SET AlarmRead = " + alarm.AlarmRead + " where E_Date =" + alarm.E_Date.Ticks).FirstOrDefault();
                                        conn.Update(updateSQLiteDB);
                                    }
                                    else if (conn.Table<AlarmData>().Where(u => (u.E_Date == alarm.E_Date)
                                    && (u.NodeID == alarm.NodeID)
                                    && (u.AlarmRead == alarm.AlarmRead)
                                    && (u.SiteID == alarm.SiteID)
                                    && (u.AlarmType == alarm.AlarmType)).Any())
                                    {
                                        //Do nothing
                                    }
                                    else
                                    {
                                        conn.Insert(alarm);
                                    }
                                }
                            }

                            conn.CreateTable<NotificationMessage>();

                            NotificationMessage notificationMessage = new NotificationMessage();

                            notificationMessage.UserResponded = "true";
                            notificationMessage.Id = "0";

                            if (conn.Table<NotificationMessage>().Where(u => u.Id == notificationMessage.Id).Any())
                            {
                                if (notificationMessage.UserResponded == "true")
                                {
                                    var setResponse = conn.Query<NotificationMessage>("UPDATE NotificationMessage SET UserResponded='" + notificationMessage.UserResponded + "' where Id = '" + notificationMessage.Id + "'").FirstOrDefault();
                                    conn.Update(setResponse);
                                }

                                var queryResult = from s in conn.Table<NotificationMessage>()
                                                  where s.UserResponded.Equals("true")
                                                  select s;

                                bool has = queryResult.Any(s => s.UserResponded == "true");

                                if (has)
                                {
                                    App.notificationMessage.Message = "";
                                }
                            }
                        }
                        return result = 1;
                    }
                }
                catch (Exception ex)
                {
                    result = 3;
                    return result;
                }
            }
        }
    }
}
