using System;
using System.Collections.Generic;
using System.Linq;
using SmartSUB_Remote_Monitor.Model;

namespace SmartSUB_Remote_Monitor.Services
{
    public class NotificationActionService : ISmartSUBNotificationActionService
    {
        readonly Dictionary<string, PushNotificationAction> _actionMappings = new Dictionary<string, PushNotificationAction>
        {
            { "New data is available for download.", PushNotificationAction.ActionA },
            { "action_b", PushNotificationAction.ActionB }
        };

        public event EventHandler<PushNotificationAction> ActionTriggered = delegate { };

        public void TriggerAction(string action)
        {
            if (!_actionMappings.TryGetValue(action, out var pushAction))
                return;

            List<Exception> exceptions = new List<Exception>();

            foreach (var handler in ActionTriggered?.GetInvocationList())
            {
                try
                {
                    handler.DynamicInvoke(this, pushAction);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Any())
                throw new AggregateException(exceptions);
        }
    }
}