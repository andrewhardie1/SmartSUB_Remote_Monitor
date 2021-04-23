using Dms.Cms.SystemModel;
using SmartSUB_Remote_Monitor.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace SmartSUB_Remote_Monitor.ViewModel
{
    class UserVerificationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<Model.UserVerificationViewModel> users { get; set; }
        public ObservableCollection<Model.UserVerificationViewModel> Users
        {
            get { return users; }
            set
            {
                users = value;
                OnPropertyChanged("Users");
            }
        }

        String _userRole;

        public UserVerificationViewModel(String UserRole)
        {
            _userRole = UserRole;
        }

        public bool GetUsers()
        {
            try
            {
                Users = new Model.UserVerificationViewModel().GetUsers(_userRole);
                
                if (Users == null)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Test(int num)
        {
            if (num == 1)
            {
                return true;
            }
            return false;
        }
    }
}
