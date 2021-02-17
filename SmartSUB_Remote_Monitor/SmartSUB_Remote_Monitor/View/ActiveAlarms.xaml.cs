using Dms.Cms.SystemModel;
using Microsoft.Identity.Client;
using SmartSUB_Remote_Monitor.Model;
using SmartSUB_Remote_Monitor.Services;
using SmartSUB_Remote_Monitor.ViewModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartSUB_Remote_Monitor
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActiveAlarms : ContentPage
    {
        public ActiveAlarms(SystemInterface systemInterface)
        {
            InitializeComponent();

            NotificationMessage.InsertOnStartup();

            notificationLabel.BindingContext = App.notificationMessage;

            ServiceContainer.Resolve<ISmartSUBNotificationActionService>()
                .ActionTriggered += NotificationActionTriggered;

            ActiveAlarmsViewModel vm = new ActiveAlarmsViewModel(systemInterface);

            BindingContext = vm;
        }

        async void NotificationActionTriggered(object sender, PushNotificationAction e)
        {
            DisplayAlert();

            if (App.notificationMessage.AutoSync == "Enabled")
            {
                await Task.Delay(TimeSpan.FromSeconds(5));

                int success = AlarmData.GetAlarms();

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    if (success == 1)
                        this.DisplayAlert("Success", "Alarms added to database.", "OK");
                    else if (success == 2)
                        this.DisplayAlert("Failure", "No data was present within SQL Server.", "OK");
                    else
                        this.DisplayAlert("Failure", "Could not connect SQL server - please try again", "OK");
                });
            }
        }

        void DisplayAlert()
        {
            NotificationMessage.UpdateNotificationResponse(App.notificationMessage);  
        }

        protected override void OnAppearing()
        {
            //base.OnAppearing();

            //var alarms = AlarmData.ReadActiveAlarms(App.stationSelected);
            //alarmListView.ItemsSource = alarms;
            
        }
    }
}