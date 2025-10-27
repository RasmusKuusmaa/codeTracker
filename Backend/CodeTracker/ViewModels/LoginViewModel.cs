using CodeTracker.Helpers;
using CodeTracker.Service;
using CodeTracker.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CodeTracker.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private void GoToRegister()
        {
            RegisterWindow rw = new RegisterWindow();
            rw.Show();
            
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                }
            }
        }
        AuthService _authService = new AuthService();
        private async void Login()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Username and password cannot be empty");
                return;
            };
            bool success = await _authService.LoginAsync(Username, Password);

            if (success)
            {

                MainWindow mw = new MainWindow();
                mw.Show();

                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this)
                    {
                        window.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Incorrect Credentials");
                return;
            }
        }


        private string _username;

        public string Username
        {
            get { return _username; }
            set { _username = value;
                OnPropertyChanged();
            }
        }
        private string _password;

        public string Password
        {
            get { return _password; }
            set { _password = value;
                OnPropertyChanged();
            }
        }


        public RelayCommand GoToRegisterCommand { get; }
        public RelayCommand LoginCommand { get; }
        public LoginViewModel()
        {
            GoToRegisterCommand = new RelayCommand(GoToRegister);
            LoginCommand = new RelayCommand(Login);
        }
    }
}
