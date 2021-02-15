using System;
using SmartSUB_Remote_Monitor.Model;

namespace SmartSUB_Remote_Monitor.Services
{
    public interface ISmartSUBNotificationActionService : INotificationActionService
    {
        event EventHandler<PushNotificationAction> ActionTriggered;
    }
}