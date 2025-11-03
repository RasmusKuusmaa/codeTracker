using CodeTracker.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CodeTracker;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainFrame.Navigate(new DashBoardPage());
    }

    private void DashBoardBtn_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new DashBoardPage());
    }

    private void ProjectsBtn_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new ProjectsPage());
    }

    private void SessionsBtn_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new SessionsPage());
    }

    private void LanguagesBtn_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new LanguagesPage());
    }

    private void HistoryBtn_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new HistoryPage());
    }
}