using Dms.Cms.SystemModel;
using SmartSUB_Remote_Monitor.Model;
using SmartSUB_Remote_Monitor.Services;
using SmartSUB_Remote_Monitor.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartSUB_Remote_Monitor
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryAlarms : ContentPage
    {
        public HistoryAlarms(SystemInterface systemInterface)
        {
            InitializeComponent();

            NotificationMessage.InsertOnStartup();

            //notificationMessage = new NotificationMessage();

            notificationLabel.BindingContext = App.notificationMessage;

            ServiceContainer.Resolve<ISmartSUBNotificationActionService>()
                .ActionTriggered += NotificationActionTriggered;

            HistoryAlarmsViewModel vm = new HistoryAlarmsViewModel(systemInterface);

            BindingContext = vm;

        }

        void NotificationActionTriggered(object sender, PushNotificationAction e)
    => DisplayAlert();

        void DisplayAlert()
        {
            NotificationMessage.UpdateNotificationResponse(App.notificationMessage);
        }

        protected override void OnAppearing()
        {
            //base.OnAppearing();

            //var alarms = AlarmData.ReadInctiveAlarms(App.stationSelected);
            //postListView.ItemsSource = alarms;
        }
    }
}
   