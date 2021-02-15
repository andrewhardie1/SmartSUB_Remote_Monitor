using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using SmartSUB_Remote_Monitor.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartSUB_Remote_Monitor
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        bool IsCheckedEnabledButton;
        bool IsCheckedDisabledButton;
        public SettingsPage()
        {
            InitializeComponent();

            EnableButton.BindingContext = IsCheckedEnabledButton;
            DisableButton.BindingContext = IsCheckedDisabledButton;
            ConnectionEntry.BindingContext = App.SmartSUBServerURL;
        }

        private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            RadioButton button = sender as RadioButton;

            NotificationMessage.SetAutoSync($"{button.Value}");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            NotificationMessage.GetAutoSync();

            if (App.notificationMessage.AutoSync == "Enabled")
            {
                IsCheckedEnabledButton = true;
                EnableButton.IsChecked = IsCheckedEnabledButton;

                IsCheckedDisabledButton = false;
                DisableButton.IsChecked = IsCheckedDisabledButton;
            }
            else if (App.notificationMessage.AutoSync == "Disabled")
            {
                IsCheckedEnabledButton = false;
                EnableButton.IsChecked = IsCheckedEnabledButton;

                IsCheckedDisabledButton = true;
                DisableButton.IsChecked = IsCheckedDisabledButton;
            }

            if (App.SmartSUBServerURL != null)
                ConnectionEntry.Text = App.SmartSUBServerURL;
        }

        private void ApplyButton_Clicked(object sender, EventArgs e)
        {
            App.SmartSUBServerURL = ConnectionEntry.Text;
            DisplayAlert("Success", "URL Updated.", "OK");
        }
    }
}