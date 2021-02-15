using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firebase.Messaging;
using WindowsAzure.Messaging;
using Android.Support.V4.App;
using SmartSUB_Remote_Monitor.Services;
namespace SmartSUB_Remote_Monitor.Droid
{
    [Service]
    [IntentFilter(new[] {"com.google.firebase.MESSAGING_EVENT"})]
    public class FirebaseService : FirebaseMessagingService
    {
        ISmartSUBNotificationActionService _notificationActionService;
        IDeviceInstallationService _deviceInstallationService;

        ISmartSUBNotificationActionService NotificationActionService
            => _notificationActionService ??
                (_notificationActionService =
                ServiceContainer.Resolve<ISmartSUBNotificationActionService>());

        IDeviceInstallationService DeviceInstallationService
            => _deviceInstallationService ??
                (_deviceInstallationService =
                ServiceContainer.Resolve<IDeviceInstallationService>());

        public FirebaseService()
        {
        }

        public override void OnNewToken(string token)
        {
            SendRegistrationToAzure(token);
        }

        private void SendRegistrationToAzure(string token)
        {
            try
            {
                NotificationHub hub = new NotificationHub("SmartSUB", "Endpoint=sb://notificationhubsmartsubalarms.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=UaaRk2GVs+umbGc96ijwafFIXBE8t6eB0NlM/1JN72Q=", this);

                Registration registration = hub.Register(token, new string[] { "default" });

                string pnsHandle = registration.PNSHandle;

                hub.RegisterTemplate(pnsHandle, "default template", "{\"data\":{\"message\":\"Notification Hub test notification\"}}", new string[] { "default" });
            }
            catch(Exception ex)
            {

            }
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);
            string messageBody = string.Empty;

            if (message.GetNotification() != null)
            {
                messageBody = message.GetNotification().Body;
            }
            else
            {
                messageBody = message.Data.Values.First();
            }

            SendLocalNotification(messageBody);

            NotificationActionService.TriggerAction(messageBody);

        }

        private void SendLocalNotification(string messageBody)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            intent.PutExtra("message", messageBody);

            var requestCode = new Random().Next();
            var pendingIntent = PendingIntent.GetActivity(this, requestCode, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new NotificationCompat.Builder(this)
                .SetContentTitle("SmartSUB")
                .SetSmallIcon(Resource.Drawable.ic_launcher)
                .SetContentText(messageBody)
                .SetAutoCancel(true)
                .SetShowWhen(false)
                .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManager.FromContext(this);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                NotificationChannel channel = new NotificationChannel(
                    "SmartSUBNotifyChannel",
                    "SmartSUB",
                    NotificationImportance.High);

                notificationManager.CreateNotificationChannel(channel);

                notificationBuilder.SetChannelId("SmartSUBNotifyChannel");
            }

            notificationManager.Notify(0, notificationBuilder.Build());
        }
    }
}