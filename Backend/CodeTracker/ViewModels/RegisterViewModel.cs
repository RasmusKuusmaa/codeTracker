using CodeTracker.Helpers;
using CodeTracker.Views;
using System;
using System.Collections.Generic;
using System.Linq;
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
            LoginWIndow rw = new LoginWIndow();
            rw.Show();

            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                }
            }
        }
        public ICommand GoToLoginCommand { get; }

        public RegisterViewModel()
        {
            GoToLoginCommand = new RelayCommand(GoToLogin);
        }
    }
}
