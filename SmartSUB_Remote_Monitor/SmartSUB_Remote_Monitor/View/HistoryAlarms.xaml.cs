using Dms.Cms.SystemModel;
using SmartSUB_Remote_Monitor.Model;
using SmartSUB_Remote_Monitor.Services;
using SmartSUB_Remote_Monitor.ViewModel;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartSUB_Remote_Monitor
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryAlarms : ContentPage
    {
        HistoryAlarmsViewModel viewModel;

        public HistoryAlarms(SystemInterface systemInterface, HomePage homePage)
        {
            InitializeComponent();

            homePage.GetDataButtonClickedEvent += HomePage_GetDataButtonClickedEvent;

            NotificationMessage.InsertOnStartup();

            //notificationMessage = new NotificationMessage();

            notificationLabel.BindingContext = App.notificationMessage;

            ServiceContainer.Resolve<ISmartSUBNotificationActionService>()
                .ActionTriggered += NotificationActionTriggered;
            
            viewModel = new HistoryAlarmsViewModel(systemInterface);

            viewModel.GetHistoricAlarms();

            BindingContext = viewModel;

        }

        private async void HomePage_GetDataButtonClickedEvent(object sender, System.EventArgs e)
        {
            bool result = viewModel.GetHistoricAlarms();

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

                viewModel.GetHistoricAlarms();
            }
        }

        void DisplayAlert()
        {
            NotificationMessage.UpdateNotificationResponse(App.notificationMessage);
        }
    }
}
   