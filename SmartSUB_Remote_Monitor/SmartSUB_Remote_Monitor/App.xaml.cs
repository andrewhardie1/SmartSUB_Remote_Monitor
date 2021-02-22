using Microsoft.WindowsAzure.MobileServices;
using SmartSUB_Remote_Monitor.Model;
using Xamarin.Forms;
using Microsoft.Identity.Client;
using SmartSUB_Remote_Monitor.View;

namespace SmartSUB_Remote_Monitor
{
    public partial class App : Application
    {
        public static string DatabaseLocation = string.Empty;
        public static MobileServiceClient client = new MobileServiceClient("https://smartsub-databaseconnapp.azurewebsites.net");
        public static Users user = new Users();
        //public static AlarmData alarmdata = new AlarmData();
        public static NotificationMessage notificationMessage = new NotificationMessage();
        public static int stationSelected;
        public static string SmartSUBServerURL;

        public App()
        {
            InitializeComponent();
        }

        public App(string databaseLocation)
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());

            DatabaseLocation = databaseLocation;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
