using Dms.Cms.Messaging;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Dms.Cms.SessionHandler.Connection;
using Dms.Cms.SessionHandler;
using Dms.Cms.SessionHandler.Stubs;
using SmartSUB_Remote_Monitor.Services;
using System;
using System.Timers;
using Dms.Cms.SystemModel;

namespace SmartSUB_Remote_Monitor.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private string hostName;
        private ushort portNumber;
        private string connectionStatus;
        private List<Symbol> groupIDs;
        private List<string> localeIDs;
        private Symbol selectedGroup;
        private string selectedLocale;
        private bool userOptionsReceived;
        private bool _isAuthenticationRequired;
        private string userName;
        private SystemInterface systemInterface;

        public SystemInterface SystemInterface
        {
            get
            {
                return systemInterface;
            }
            set
            {
                if (systemInterface != value)
                {
                    systemInterface = value;
                    OnPropertyChanged("SystemInterface");
                }
            }
        }

        public string HostName
        {
            get
            {
                return hostName;
            }
            set
            {
                if (hostName != value)
                {
                    userOptionsReceived = false;
                    hostName = value;
                    OnPropertyChanged("HostName");
                }
            }
        }

        public ushort PortNumber
        {
            get
            {
                return portNumber;
            }
            set
            {
                if (portNumber != value)
                {
                    userOptionsReceived = false;
                    portNumber = value;
                    OnPropertyChanged("PortNumber");
                }
            }
        }

        public string ConnectionStatus
        {
            get
            {
                return connectionStatus;
            }
            set
            {
                if (connectionStatus != value)
                {
                    connectionStatus = value;
                    OnPropertyChanged("ConnectionStatus");
                }
            }
        }

        public List<Symbol> GroupIDs
        {
            get
            {
                return groupIDs;
            }
            set
            {
                if (groupIDs != value)
                {
                    groupIDs = value;
                    OnPropertyChanged("GroupIDs");
                }
            }
        }

        public Symbol SelectedGroup
        {
            get
            {
                return selectedGroup;
            }
            set
            {
                if (selectedGroup != value)
                {
                    selectedGroup = value;
                    OnPropertyChanged("SelectedGroup");
                }
            }
        }

        public List<string> LocaleIDs
        {
            get
            {
                return localeIDs;
            }
            set
            {
                if (localeIDs != value)
                {
                    localeIDs = value;
                    OnPropertyChanged("LocaleIDs");
                }
            }
        }

        public string SelectedLocale
        {
            get
            {
                return selectedLocale;
            }
            set
            {
                if (selectedLocale != value)
                {
                    selectedLocale = value;
                    OnPropertyChanged("SelectedLocale");
                }
            }
        }

        public bool IsAuthenticationRequired
        {
            get
            {
                return _isAuthenticationRequired;
            }
            set
            {
                if (_isAuthenticationRequired != value)
                {
                    _isAuthenticationRequired = value;
                    OnPropertyChanged("IsAuthenticationRequired");
                }
            }
        }

        public string Username
        {
            get
            {
                return userName;
            }
            set
            {
                if (userName != value)
                {
                    userName = value;
                    OnPropertyChanged("Username");
                }
            }
        }

        public MainPageViewModel()
        {
            userOptionsReceived = false;

            SetTimerForUserOptions();
        }

        private void SetTimerForUserOptions()
        {
            Timer timer = new Timer();
            timer.Interval = 5000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ExecuteUserOptionsCommand();
        }

        public void ExecuteUserOptionsCommand()
        {
            if (!userOptionsReceived)
            {
                ConnectionStatus = "Requesting options from CMS service...";
                IsAuthenticationRequired = false;
            }
                

            Services.ConnectionManager manager = new Services.ConnectionManager();

            var login = new StubLoginInfo
            {
                Hostname = HostName,
                Security = 0,
                Port = PortNumber
            };

            manager.GetUserOptions(login,
                                        (state, options) =>
                                        {
                                            App.Current.Dispatcher.BeginInvokeOnMainThread(new Action(() => UserOptionsRetrieved(state, options, manager)));
                                        });
        }

        public void UserOptionsRetrieved(ConnectionSystemState state, UserOptions options, Services.ConnectionManager manager)
        {
            if (manager == null
                || (manager.Hostname == HostName
                    && manager.Port == PortNumber))
            {
                if (options == null || options.GroupID == null || options.Locale == null)
                {
                    GroupIDs = new List<Symbol>();
                    SelectedGroup = null;
                    LocaleIDs = new List<string>();
                    SelectedLocale = string.Empty;
                    userOptionsReceived = false;
                    IsAuthenticationRequired = false;
                    if (state == ConnectionSystemState.NotReady)
                    {
                        ConnectionStatus = "Initialising connection...";
                        IsAuthenticationRequired = false;
                    }
                    else
                    {
                        ConnectionStatus = "Cannot connect";
                        IsAuthenticationRequired = false;
                    }
                }
                else if (manager != null)
                {
                    GroupIDs = options.GroupID.ToList();
                    LocaleIDs = options.Locale.ToList();

                    SelectedGroup = GroupIDs[0];
                    SelectedLocale = LocaleIDs[0];

                    ConnectionStatus = "Server contacted";
                    IsAuthenticationRequired = true;

                    userOptionsReceived = true;

                    if (!options.AuthenticationRequired)
                        Username = string.Empty;
                }
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
