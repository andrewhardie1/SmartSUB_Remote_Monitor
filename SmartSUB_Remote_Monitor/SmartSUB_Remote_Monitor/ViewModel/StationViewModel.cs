using Dms.Cms.Messaging;
using Dms.Cms.SystemModel;
using SmartSUB_Remote_Monitor.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;

namespace SmartSUB_Remote_Monitor.ViewModel
{
    public class StationViewModel : INotifyPropertyChanged, INodeListener
    {
        public List<Stations> Stations { get; set; }

        private ISubscriptionHandler subscriptionHandler;
        private List<NodeID> nodes = new List<NodeID>();

        private string subscribed;
        public string Subscribed
        {
            get
            {
                return subscribed;
            }
            set
            {
                if (subscribed != value)
                {
                    subscribed = value;
                    OnPropertyChanged("Subscribed");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public StationViewModel(SystemInterface systemInterface)
        {
             Stations = new Stations().GetStations(systemInterface);
        }

        public void AttachSubscriptions(SystemInterface systemInterface)
        {
            subscriptionHandler = systemInterface.SubscriptionHandler;
            subscriptionHandler.AttachGlobalListener(this, SubscriptionKind.AlarmState);
        }

        public void OnNodeStatusUpdate(NodeID nodeID, DateTime timestamp, NodeStatus nodeStatus)
        {
            throw new NotImplementedException();
        }

        public void OnPropertyUpdate(NodeID nodeID, DateTime timestamp, Message properties)
        {
            throw new NotImplementedException();
        }

        public void OnAlarmUpdate(AlarmRecord alarmRecord)
        {
            //Do Nothing
        }
    }
}
