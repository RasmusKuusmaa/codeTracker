using CodeTracker.Helpers;
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

        private void Login()
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
        public RelayCommand GoToRegisterCommand { get; }
        public RelayCommand LoginCommand { get; }
        public LoginViewModel()
        {
            GoToRegisterCommand = new RelayCommand(GoToRegister);
            LoginCommand = new RelayCommand(Login);
        }
    }
}
