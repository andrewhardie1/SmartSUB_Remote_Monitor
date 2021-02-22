using System;
using Dms.Cms.SystemModel;
using Microsoft.Identity.Client;
using SmartSUB_Remote_Monitor.Model;
using SmartSUB_Remote_Monitor.View;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartSUB_Remote_Monitor
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : TabbedPage
    {
        SystemInterface _systemInterface;

        public event EventHandler GetDataButtonClickedEvent;

        public HomePage(SystemInterface systemInterface)
        {
            InitializeComponent();
            _systemInterface = systemInterface;
            this.Children.Add(new ActiveAlarms(systemInterface, this) { IconImageSource= "ic_action_warning.png", Title= "Active Alarms" });
            this.Children.Add(new HistoryAlarms(systemInterface, this) { IconImageSource = "ic_action_history.png", Title = "History Alarms" });
            GetDataButton.Clicked += GetDataButtonClicked;
        }

        protected void GetDataButtonClicked(object sender, EventArgs e)
        {
            GetDataButtonClickedEvent?.Invoke(sender, e);
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            //int success = AlarmData.GetAlarms();

            //if (success == 0)
            //    await DisplayAlert("Failure", "SmartSUB connection URL cannot not be empty. Please update within Settings page.", "OK");
            //else if(success == 1)
            //    await DisplayAlert("Success", "Alarms added to database.", "OK");
            //else if (success == 2)
            //    await DisplayAlert("Failure", "No data was present within SQL Server.", "OK");
            //else if (success == 3)
            //    await DisplayAlert("Failure", "An error occured. Please check IP is correct within Settings.", "OK");
            //else
            //{
            //    await DisplayAlert("Failure", "Could not connect SQL server - please try again", "OK");
            //}
        }

        private void Settings_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SettingsPage());
        }

        private void UserAccount_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AccountPage());
        }
    }
}