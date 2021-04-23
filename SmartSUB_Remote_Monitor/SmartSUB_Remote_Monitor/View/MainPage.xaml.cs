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
using SmartSUB_Remote_Monitor.Model;

namespace SmartSUB_Remote_Monitor
{
    public partial class MainPage : ContentPage
    {
        public MainPageViewModel Model { get; set; } = new MainPageViewModel();
        Model.UserVerificationViewModel user = new Model.UserVerificationViewModel();

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

            try
            {
                if (connectionManager.Connect(this.HostnameEntry.Text,
                    this.UserNameEntry.Text,
                    this.PasswordEntry.Text,
                    Convert.ToUInt16(this.PortEntry.Text),
                    Symbol.Intern(this.GroupIDPicker.SelectedItem.ToString()),
                    "en-gb"))
                {
                    Model.SystemInterface = connectionManager.SystemInterface;
                    App.SmartSUBServerURL = this.HostnameEntry.Text;
                    App.user.UserRole = this.GroupIDPicker.SelectedItem.ToString();
                    if (user.CheckUserExists(this.UserNameEntry.Text))
                    {
                        await Navigation.PushAsync(new UserVerificationView(Model.SystemInterface, this.UserNameEntry.Text));
                    }
                    else
                    {
                        await DisplayAlert("Error", "User does not exist - contact system admin.", "OK");
                    }

                }
            }
            catch
            {
                await DisplayAlert("An error has occurred", "Could not login. Please verify user credentials.", "Dismiss");
            }
        }
    }
}
