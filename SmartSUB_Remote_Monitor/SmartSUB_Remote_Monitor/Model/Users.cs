using Microsoft.Identity.Client;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSUB_Remote_Monitor.Model
{
    public class UserVerificationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string userRole;
        public string UserRole
        {
            get { return userRole; }
            set
            {
                userRole = value;
                OnPropertyChanged("UserRole");
            }
        }

        private string securityQuestion;
        public string SecurityQuestion
        {
            get { return securityQuestion; }
            set
            {
                securityQuestion = value;
                OnPropertyChanged("SecurityQuestion");
            }
        }

        private string securityAnswer;
        public string SecurityAnswer
        {
            get { return securityAnswer; }
            set
            {
                securityAnswer = value;
                OnPropertyChanged("SecurityAnswer");
            }
        }

        ObservableCollection<UserVerificationViewModel> users = new ObservableCollection<UserVerificationViewModel>();

        public bool CheckUserExists(string UserRole)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                if (conn.Table<UserVerificationViewModel>().Where(u => (u.UserRole == userRole)).Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public ref ObservableCollection<UserVerificationViewModel> GetUsers(string UserRole)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                UserVerificationViewModel userQualitrolAdmin = new UserVerificationViewModel()
                {
                    UserRole = "Admin",
                    SecurityQuestion = "What is your favourite animal?",
                    SecurityAnswer = "Dog"
                };

                UserVerificationViewModel userQualitrolEngineer = new UserVerificationViewModel()
                {
                    UserRole = "Engineer",
                    SecurityQuestion = "What is your lucky number?",
                    SecurityAnswer = "3"
                };

                conn.CreateTable<UserVerificationViewModel>();

                if (conn.Table<UserVerificationViewModel>().Where(u => (u.UserRole != userQualitrolAdmin.UserRole)).Any())
                {
                    conn.Insert(userQualitrolAdmin);
                }
                if (conn.Table<UserVerificationViewModel>().Where(u => (u.UserRole != userQualitrolEngineer.UserRole)).Any())
                {
                    conn.Insert(userQualitrolEngineer);
                }

                var queryResult = from s in conn.Table<UserVerificationViewModel>()
                                  where s.UserRole.Equals(UserRole)
                                  select s;

                UserVerificationViewModel userFromDatabase = queryResult.FirstOrDefault();

                users.Add(new UserVerificationViewModel()
                {
                    UserRole = userFromDatabase.UserRole,
                    SecurityQuestion = userFromDatabase.SecurityQuestion,
                    SecurityAnswer = userFromDatabase.SecurityAnswer
                });
            }

            return ref users;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
