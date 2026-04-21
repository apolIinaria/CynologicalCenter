using System.Windows;
using System;
using CynologicalCenter.Helpers;

namespace CynologicalCenter.UI.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow() => InitializeComponent();
        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            TxtError.Visibility = Visibility.Collapsed;
            string username = TxtUsername.Text.Trim();
            string password = TxtPassword.Password;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ShowError("Введіть логін та пароль");
                return;
            }

            var (success, role) = await App.SecurityService.LoginAsync(username, password);
            if (!success)
            {
                ShowError("Невірний логін або пароль");
                TxtPassword.Clear();
                return;
            }

            CurrentUser.Login(username, role);

            var main = new MainWindow();
            main.Show();
            Close();
        }

        private void ShowError(string msg)
        {
            TxtError.Text = msg;
            TxtError.Visibility = Visibility.Visible;
        }
    }
}