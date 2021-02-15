using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.OS;
using Android.Runtime;
using Firebase.Iid;
using Microsoft.Identity.Client;
using Microsoft.WindowsAzure.MobileServices;
using SmartSUB_Remote_Monitor.Droid.Services;
using SmartSUB_Remote_Monitor.Services;

namespace SmartSUB_Remote_Monitor.Droid
{
    [Activity(
    Label = "SmartSUB",
    LaunchMode = LaunchMode.SingleTop,
    Icon = "@drawable/ic_launcher",
    Theme = "@style/MainTheme",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, Android.Gms.Tasks.IOnSuccessListener
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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Bootstrap.Begin(() => new DeviceInstallationService());

            if (DeviceInstallationService.NotificationsSupported)
            {
                FirebaseInstanceId.GetInstance(Firebase.FirebaseApp.Instance)
                    .GetInstanceId()
                    .AddOnSuccessListener(this);
            }

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            CurrentPlatform.Init();

            string dbName = "smartsub_db.sqlite";
            string folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string fullPath = Path.Combine(folderPath, dbName);

            LoadApplication(new App(fullPath));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnSuccess(Java.Lang.Object result)
        => DeviceInstallationService.Token =
            result.Class.GetMethod("getToken").Invoke(result).ToString();

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
        }
    }
}