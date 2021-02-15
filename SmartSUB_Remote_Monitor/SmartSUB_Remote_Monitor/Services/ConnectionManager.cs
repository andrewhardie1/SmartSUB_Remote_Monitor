using System;
using Dms.Cms.Messaging;
using Dms.Cms.SessionHandler;
using Dms.Cms.SessionHandler.Connection;
using Dms.Cms.SessionHandler.Stubs;
using Dms.Cms.SystemModel;
using Dms.Cms.SystemModel.Utility;
using System.Net;

namespace SmartSUB_Remote_Monitor.Services
{
    public sealed class ConnectionManager
    {
        private readonly object interlock = new object();
        private TcpMessageClient client;
        private Symbol groupID;
        private SystemInterface systemInterface;
        private Message _idl;
        private IPEndPoint sessionId_;
        private String remIPAddress;  // this session as seen by remote
        private UInt16 remPort;  // this session as seen by remote

        public event EventHandler ConnectionDropped;
        public Action<string, string> LogMessage;
        private string hostName;
        private string appVersion;
        private ushort portNumber;
        private TransportSecurity security;

        public ConnectionManager(string hostName, string appVersion)
        {
            this.hostName = hostName;
            this.appVersion = appVersion;
        }

        public ConnectionManager()
        { }

        public bool IsConnected { get; private set; }

        public Symbol GroupID
        {
            get { return this.groupID; }
        }

        public string Hostname
        {
            get { return this.hostName; }
            set
            {
                this.hostName = value;
            }
        }

        public ushort Port
        {
            get { return this.portNumber; }
            set
            {
                this.portNumber = value;
            }
        }

        public TransportSecurity Security
        {
            get { return this.security; }
            set
            {
                this.security = value;
            }
        }

        public SystemInterface SystemInterface
        {
            get { return this.systemInterface; }
        }

        public Message Idl
        {
            get { return this._idl; }
        }

        public IPEndPoint SessionId => sessionId_;
        public String RemIPAddress => remIPAddress;
        public UInt16 RemPort => remPort;

        public ILoginInfo LoginInfo { get; private set; }

        private void OnConnectionDropped(object sender, EventArgs e)
        {
            IsConnected = false;
            var handler = ConnectionDropped;

            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public void Disconnect()
        {
            IsConnected = false;

            TcpMessageClient client;

            lock (this.interlock)
            {
                client = this.client;
                this.client = null;
            }

            if (client == null)
                return;

            client.DropPipe();
            client.Dispose();
        }

        //HostName, Username, password, PortNumber, SelectedGroup)
        public bool Connect(string hostname, string username, string password, ushort portNumber, Symbol groupID, string locale)
        {
            IsConnected = false;


            var logonInfo = new StubLoginInfo
            {
                Hostname = hostname,
                //Hostname = User specified hostname,
                Port = portNumber,
                //Port = 1338,
                Security = TransportSecurity.Insecure,
                //Security = 0,
                UserName = username,
                GroupID = groupID,
                Locale = locale
                //GroupID = Symbol.Intern("Pdmusers")
            };

            LoginInfo = logonInfo;

            var client = new TcpMessageClient(
                hostname,
                portNumber,
                TransportSecurity.Insecure);

            lock (this.interlock)
                this.client = client;

            // insert logging between stub and client
            BidiSharedPipe shared = new BidiSharedPipe();

            if (this.LogMessage != null)
            {
                shared.Send.MessageReceived += new LogPipe("sent", this.LogMessage).PostMessage;
                shared.Recv.MessageReceived += new LogPipe("recv", this.LogMessage).PostMessage;
            }

            shared.Send.MessageReceived += client.SendPipe.PostMessage;

            client.ReceivePipe = shared.Recv;

            shared.Recv.PipeDropped += OnConnectionDropped;

            client.StartReceiving();

            // create a stub system interface and get back some IDL
            Message idl;

            var connectSettings = new ConnectSettings
            {
                Client = shared,
                Secret = password,
                UiClientVersion = appVersion,
                ClientType = "SSEClientUi"
            };

            var systemInterface = SystemInterfaceStub.Connect(connectSettings, logonInfo, out idl);

            lock (this.interlock)
            {
                idl.Extract(Symbol.Intern("GroupID"), out this.groupID);
                this.systemInterface = systemInterface;
                _idl = idl;
                IsConnected = true;
            }

            sessionId_ = client.SessionId;
            remIPAddress = connectSettings.remIPAddress;  // this session as seen by remote
            remPort = connectSettings.remPort;  // this session as seen by remote
            return IsConnected;
        }

        /// <summary>
        /// Fetches the user options from the server and closes connection
        /// </summary>
        public void GetUserOptions(ILoginInfo loginInfo, Action<ConnectionSystemState, UserOptions> callback)
        {
            AsyncAction.Invoke(GetUserOptionsSync, loginInfo, callback);
        }

        public void GetUserOptionsSync(ILoginInfo loginInfo, Action<ConnectionSystemState, UserOptions> callback)
        {
            try
            {
                Hostname = loginInfo.Hostname;
                Port = loginInfo.Port;
                Security = loginInfo.Security;

                // create tcp client
                TcpMessageClient client = new TcpMessageClient(Hostname, Port, Security);

                client.StartReceiving();

                UserOptionsRequest request = new UserOptionsRequest(callback);

                client.ReceivePipe = request;

                client.SendPipe.PostMessage(new Message(Symbol.Intern("UserOptions")).Append(Symbol.Intern("GetGroups"), Symbol.Intern("GetGroupIDs")));
            }
            catch (Exception) // may be IO, may be other
            {
                callback(ConnectionSystemState.Unknown, new UserOptions());
            }
        }

        private class UserOptionsRequest : IMessagePipe
        {
            private Action<ConnectionSystemState, UserOptions> callback;

            internal UserOptionsRequest(Action<ConnectionSystemState, UserOptions> callback)
            {
                this.callback = callback;
            }

            public bool PostMessage(Message message)
            {
                if (callback == null) return false;

                if (message.Name == Symbol.Intern("UserOptions"))
                    callback(
                        ConnectionSystemState.Ready,
                        new PropertySetter().SetProperties(new UserOptions(), message));
                else
                    if (message.Name == Symbol.Intern("SystemNotReady"))
                    callback(ConnectionSystemState.NotReady, null);
                else
                    callback(ConnectionSystemState.Unknown, null);

                callback = null;

                return true;
            }

            public void DropPipe()
            {
                if (callback != null)
                    callback(ConnectionSystemState.Unknown, null);
            }
        }
    }
}
