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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CynologicalCenter.Data;
using CynologicalCenter.Helpers;
using CynologicalCenter.UI.Pages;
using MySql.Data.MySqlClient;

namespace CynologicalCenter.UI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxtUserInfo.Text = $"{CurrentUser.Username}\n({CurrentUser.Role})";

            if (CurrentUser.IsAdmin)
                BtnAdmin.Visibility = Visibility.Visible;
            MainFrame.Navigate(new DashboardPage());
        }

        private void NavDashboard_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new DashboardPage());
        private void NavOwners_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new OwnersPage());
        private void NavDogs_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new DogsPage());
        private void NavTrainers_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new TrainersPage());
        private void NavCourses_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new CoursesPage());
        private void NavSessions_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new SessionsPage());
        private void NavReports_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new ReportsPage());
        private void NavAdmin_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new AdminPage());
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.Logout();
            var login = new LoginWindow();
            login.Show();
            Close();
        }
    }
}
