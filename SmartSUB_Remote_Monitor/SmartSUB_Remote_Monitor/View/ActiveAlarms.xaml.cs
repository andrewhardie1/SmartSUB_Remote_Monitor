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
        ActiveAlarmsViewModel viewModel;

        public ActiveAlarms(SystemInterface systemInterface, HomePage homePage)
        {
            InitializeComponent();

            homePage.GetDataButtonClickedEvent += HomePage_GetDataButtonClickedEvent;

            NotificationMessage.InsertOnStartup();

            notificationLabel.BindingContext = App.notificationMessage;

            ServiceContainer.Resolve<ISmartSUBNotificationActionService>()
                .ActionTriggered += NotificationActionTriggered;

            viewModel = new ActiveAlarmsViewModel(systemInterface);

            viewModel.GetActiveAlarms();

            BindingContext = viewModel;
            
        }

        private async void HomePage_GetDataButtonClickedEvent(object sender, EventArgs e)
        {
            bool result = viewModel.GetActiveAlarms();

            if (result)
            {
                await DisplayAlert("Success", "Alarms have been updated", "OK");
            }
            else
            {
                await DisplayAlert("Failure", "An error occured. Please check IP is correct within Settings.", "OK");
            }
        }

        async void NotificationActionTriggered(object sender, PushNotificationAction e)
        {
            DisplayAlert();

            if (App.notificationMessage.AutoSync == "Enabled")
            {
                await Task.Delay(TimeSpan.FromSeconds(5));

                bool result = viewModel.GetActiveAlarms();

                if (result)
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.DisplayAlert("Success", "Alarms have been updated", "OK");
                    });

                    App.notificationMessage.Message = "";
                }
                else
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        this.DisplayAlert("Failure", "An error occured. Please check IP is correct within Settings.", "OK");
                    });
                }
            }
        }

        void DisplayAlert()
        {
            NotificationMessage.UpdateNotificationResponse(App.notificationMessage);  
        }
    }
}