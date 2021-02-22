using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using SmartSUB_Remote_Monitor.Services;
using SmartSUB_Remote_Monitor.ViewModel;
using Dms.Cms.SessionHandler.Stubs;
using Dms.Cms.Messaging;
using SmartSUB_Remote_Monitor.View;
using Dms.Cms.SystemModel;

namespace SmartSUB_Remote_Monitor
{
    public partial class MainPage : ContentPage
    {
        public MainPageViewModel Model { get; set; } = new MainPageViewModel();

        public MainPage()
        {
            InitializeComponent();

            containerStackLayout.BindingContext = Model;
        }

        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            ConnectionManager connectionManager = new ConnectionManager();

            if (this.UserNameEntry.Text == null && this.PasswordEntry.Text == null)
            {
                this.UserNameEntry.Text = "";
                this.PasswordEntry.Text = "";
            }

            if (connectionManager.Connect(this.HostnameEntry.Text,
                this.UserNameEntry.Text,
                this.PasswordEntry.Text,
                Convert.ToUInt16(this.PortEntry.Text),
                Symbol.Intern(this.GroupIDPicker.SelectedItem.ToString()),
                "en-gb"))
            {
                Model.SystemInterface = connectionManager.SystemInterface;
                App.SmartSUBServerURL = this.HostnameEntry.Text;
                await Navigation.PushAsync(new StationView(Model.SystemInterface));
            }
            else
            {
                await DisplayAlert("An error has occurred", "Could not login. Please verify connection details.", "Dismiss");
            }
        }
    }
}
