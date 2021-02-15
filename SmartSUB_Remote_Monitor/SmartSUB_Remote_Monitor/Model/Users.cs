using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSUB_Remote_Monitor.Model
{
    public class Users : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string givenName;
        public string GivenName
        {
            get { return givenName; }
            set
            {
                givenName = value;
                OnPropertyChanged("GivenName");
            }
        }

        private string familyName;
        public string FamilyName
        {
            get { return familyName; }
            set
            {
                familyName = value;
                OnPropertyChanged("FamilyName");
            }
        }

        public static string GetUserInitials()
        {
            string firstNameInitial = "A";
            string surnameInitial = "H";

            string userInitials = firstNameInitial + surnameInitial;
            return userInitials;
        }

            private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
