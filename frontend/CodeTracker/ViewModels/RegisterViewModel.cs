using CodeTracker.Helpers;
using CodeTracker.Service;
using CodeTracker.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CodeTracker.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {


        private void GoToLogin()
        {
            LoginWIndow lw = new LoginWIndow();
            lw.Show();

            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                }
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

        private string _confirmPassword;

        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set { _confirmPassword = value;
                OnPropertyChanged();
            }
        }
        AuthService _authService = new AuthService();
        private async void RegisterUser()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                MessageBox.Show("all fields must be filled");
                return;
            }

            if (Password != ConfirmPassword)
            {
                MessageBox.Show("password mismatch");
                return;
            }

            bool success = await _authService.RegisterAsync(Username, Password);
            if (success)
            {
                MessageBox.Show("account created, you may now log in");
                GoToLogin();

            } else
            {
                MessageBox.Show("account creation failed");
                return;
            }
        }

        public ICommand GoToLoginCommand { get; }
        public ICommand RegisterUserCommand { get; }
        public RegisterViewModel()
        {
            RegisterUserCommand = new RelayCommand(RegisterUser);
            GoToLoginCommand = new RelayCommand(GoToLogin);
            
        }
    }
}
