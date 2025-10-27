using CodeTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CodeTracker.Views
{
    /// <summary>
    /// Interaction logic for LoginWIndow.xaml
    /// </summary>
    public partial class LoginWIndow : Window
    {
        public LoginWIndow()
        {
            InitializeComponent();
        }

        private void Passbx_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is LoginViewModel vm)
            {
                vm.Password = Passbx.Password;
            }
        }
    }
}
