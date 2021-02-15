using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSUB_Remote_Monitor.Model
{
    public class NotificationMessage : INotifyPropertyChanged
    {
        private string id;
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }

        private string userResponded;
        public string UserResponded
        {
            get { return userResponded; }
            set
            {
                userResponded = value;
                OnPropertyChanged("UserResponded");
            }
        }

        private string autoSync;
        public string AutoSync
        {
            get { return autoSync; }
            set
            {
                autoSync = value;
                OnPropertyChanged("AutoSync");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public static void UpdateNotificationResponse(NotificationMessage notificationMessage)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<NotificationMessage>();

                notificationMessage.UserResponded = "false";
                notificationMessage.Id = "0";

                if (conn.Table<NotificationMessage>().Where(u => u.Id == notificationMessage.Id).Any())
                {
                    if (notificationMessage.UserResponded == "false")
                    {
                        var setResponse = conn.Query<NotificationMessage>("UPDATE NotificationMessage SET UserResponded='" + notificationMessage.UserResponded + "' where Id = '" + notificationMessage.Id + "'").FirstOrDefault();
                        conn.Update(setResponse);
                    }

                    var queryResult = from s in conn.Table<NotificationMessage>()
                                      where s.UserResponded.Equals("false")
                                      select s;

                    bool has = queryResult.Any(s => s.UserResponded == "false");

                    if (has)
                    {
                        notificationMessage.Message = "Alarms Available";
                    }
                }
                else
                {
                    return;
                }
            }
        }

        public static void InsertOnStartup()
        {
            NotificationMessage notificationMessage = new NotificationMessage()
            {
                Id = "0",
                Message = "",
                UserResponded = "true"
            };

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<NotificationMessage>();

                if (conn.Table<NotificationMessage>().Where(u => u.Id == notificationMessage.Id).Any())
                {
                    return;
                }
                else
                {
                    conn.Insert(notificationMessage);
                }
            }
        }

        public static List<NotificationMessage> NotificationMessageToDisplay()
        {
            List<NotificationMessage> messages;

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<NotificationMessage>();

                var queryResult = from s in conn.Table<NotificationMessage>()
                                  where s.UserResponded.Equals("false")
                                  select s;

                messages = queryResult.ToList();
            }

            return messages;
        }

        public static void SetAutoSync(string setting)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<NotificationMessage>();

                App.notificationMessage.UserResponded = "true";
                App.notificationMessage.Id = "0";
                App.notificationMessage.Message = "";

                if (setting == "Enabled")
                {
                    App.notificationMessage.AutoSync = setting;
                }
                else if (setting == "Disabled")
                {
                    App.notificationMessage.AutoSync = setting;
                }

                if (conn.Table<NotificationMessage>().Where(u => u.Id == App.notificationMessage.Id).Any())
                {
                    var setAutoSync = conn.Query<NotificationMessage>("UPDATE NotificationMessage SET AutoSync='" + App.notificationMessage.AutoSync + "' where Id = '" + App.notificationMessage.Id + "'").FirstOrDefault();
                    conn.Update(setAutoSync);
                }
                else
                {
                    conn.Insert(App.notificationMessage);
                }
            }
        }

        public static void GetAutoSync()
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<NotificationMessage>();

                App.notificationMessage.Id = "0";

                if (conn.Table<NotificationMessage>().Where(u => u.Id == App.notificationMessage.Id).Any())
                {
                    var getAutoSync = conn.Query<NotificationMessage>("SELECT * FROM NotificationMessage WHERE Id = '" + App.notificationMessage.Id + "'").FirstOrDefault();
                    App.notificationMessage.AutoSync = getAutoSync.AutoSync;
                }
            }
        }
    }
}
