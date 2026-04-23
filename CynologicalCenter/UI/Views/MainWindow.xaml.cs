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
        private Button? _activeNavButton;
        public MainWindow() => InitializeComponent();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxtUsername.Text = CurrentUser.Username;

            switch (CurrentUser.Role)
            {
                case "admin":
                    TxtRole.Text = "Адміністратор";
                    BadgeRole.Background = new SolidColorBrush(
                        Color.FromRgb(63, 125, 106));
                    TxtRole.Foreground = Brushes.White;
                    BtnAdmin.Visibility = Visibility.Visible;
                    break;
                case "operator":
                    TxtRole.Text = "Кінолог";
                    BadgeRole.Background = new SolidColorBrush(
                        Color.FromRgb(76, 154, 106));
                    TxtRole.Foreground = Brushes.White;
                    break;
                default:
                    TxtRole.Text = "Стажер";
                    BadgeRole.Background = new SolidColorBrush(
                        Color.FromRgb(140, 140, 140));
                    TxtRole.Foreground = Brushes.White;
                    break;
            }

            if (RoleAccess.IsGuest)
            {
                BtnReports.Visibility = Visibility.Collapsed;
            }

            Navigate(new DashboardPage(), BtnDashboard);
        }

        private void Navigate(System.Windows.Controls.Page page,
            Button activeBtn)
        {
            if (_activeNavButton != null)
                _activeNavButton.Style =
                    (Style)FindResource("NavButton");

            activeBtn.Style =
                (Style)FindResource("NavButtonActive");
            _activeNavButton = activeBtn;

            MainFrame.Navigate(page);
        }

        private void NavDashboard_Click(object sender, RoutedEventArgs e)
            => Navigate(new DashboardPage(), BtnDashboard);

        private void NavOwners_Click(object sender, RoutedEventArgs e)
            => Navigate(new OwnersPage(), BtnOwners);

        private void NavDogs_Click(object sender, RoutedEventArgs e)
            => Navigate(new DogsPage(), BtnDogs);

        private void NavTrainers_Click(object sender, RoutedEventArgs e)
            => Navigate(new TrainersPage(), BtnTrainers);

        private void NavCourses_Click(object sender, RoutedEventArgs e)
            => Navigate(new CoursesPage(), BtnCourses);

        private void NavSessions_Click(object sender, RoutedEventArgs e)
            => Navigate(new SessionsPage(), BtnSessions);

        private void NavReports_Click(object sender, RoutedEventArgs e)
            => Navigate(new ReportsPage(), BtnReports);

        private void NavAdmin_Click(object sender, RoutedEventArgs e)
            => Navigate(new AdminPage(), BtnAdmin);

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.Logout();
            new LoginWindow().Show();
            Close();
        }
    }
}
