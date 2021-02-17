using Microsoft.Identity.Client;
using SmartSUB_Remote_Monitor.Model;
using SmartSUB_Remote_Monitor.ViewModel;
using Dms.Cms.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Dms.Cms.SystemModel;

namespace SmartSUB_Remote_Monitor.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StationView : ContentPage
    {
        SystemInterface _systemInterface;
        public StationView(SystemInterface systemInterface)
        {
            InitializeComponent();

            listStations.ItemTapped += OnTapEventAsync;

            StationViewModel vm = new StationViewModel(systemInterface);

            BindingContext = vm;

            _systemInterface = systemInterface;
        }

        private void OnTapEventAsync(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;
            var selectedItem = e.Item as Stations;
            string result = selectedItem.StationID.Substring(selectedItem.StationID.Length - 1, 1);
            int stationSelected = Convert.ToInt32(result);
            App.stationSelected = stationSelected;
            Navigation.PushAsync(new HomePage(_systemInterface));
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            int success = AlarmData.GetAlarms();
            if (success == 0)
                await DisplayAlert("Failure", "SmartSUB connection URL cannot not be empty. Please update within Settings page.", "OK");
            else if (success == 1)
                await DisplayAlert("Success", "Alarms added to database.", "OK");
            else if (success == 2)
                await DisplayAlert("Failure", "No data was present within SQL Server.", "OK");
            else if (success == 3)
                await DisplayAlert("Failure", "An error occured. Please check IP is correct within Settings.", "OK");
            else
            {
                await DisplayAlert("Failure", "Could not connect SQL server - please try again", "OK");
            }

        }

        private void Settings_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SettingsPage());
        }

        protected override void OnAppearing()
        {
            string initials = Users.GetUserInitials();
            UserAccount.Text = initials;
        }

        private void UserAccount_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AccountPage());
        }
    }
}