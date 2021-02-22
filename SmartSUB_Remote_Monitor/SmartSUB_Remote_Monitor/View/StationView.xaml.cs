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
        StationViewModel viewModel;
        public StationView(SystemInterface systemInterface)
        {
            InitializeComponent();

            listStations.ItemTapped += OnTapEventAsync;

            viewModel = new StationViewModel(systemInterface);

            viewModel.GetStations();

            BindingContext = viewModel;

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
            bool result = viewModel.GetStations();

            if (result)
            {
                await DisplayAlert("Success", "Alarms have been updated", "OK");
            }
            else
            {
                await DisplayAlert("Failure", "An error occured. Please check IP is correct within Settings.", "OK");
            }
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