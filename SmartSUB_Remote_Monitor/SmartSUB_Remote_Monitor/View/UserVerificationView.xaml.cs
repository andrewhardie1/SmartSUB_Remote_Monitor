using Dms.Cms.SystemModel;
using SmartSUB_Remote_Monitor.Model;
using SmartSUB_Remote_Monitor.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartSUB_Remote_Monitor.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserVerificationView : ContentPage
    {
        SystemInterface _systemInterface;
        ViewModel.UserVerificationViewModel viewModel;
        Model.UserVerificationViewModel selectedUser;

        public UserVerificationView(SystemInterface systemInterface, string UserRole)
        {
            InitializeComponent();
            _systemInterface = systemInterface;
            viewModel = new ViewModel.UserVerificationViewModel(UserRole);
            viewModel.GetUsers();
            BindingContext = viewModel;
            selectedUser = viewModel.Users.FirstOrDefault();
        }

        private async void VerifyButton_Clicked(object sender, EventArgs e)
        {
            
            if (SecurityAnswerEntry.Text == selectedUser.SecurityAnswer)
            {
                await Navigation.PushAsync(new StationView(_systemInterface));
            }
            else
            {
                await DisplayAlert("Validation Failed", "Incorrect answer.", "Dismiss");
            }
        }
    }
}