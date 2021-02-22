using SmartSUB_Remote_Monitor.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartSUB_Remote_Monitor
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountPage : ContentPage
    {
        public AccountPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (App.user.UserRole != null)
            {
                userLabel.Text = $"Logged in as: {App.user.UserRole}";
            }
            else
            {
                userLabel.Text = "User account not found.";
            }
        }

        async void LogoutButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
        }
    }
}