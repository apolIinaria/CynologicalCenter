using System.Windows;
using System;
using CynologicalCenter.Helpers;

namespace CynologicalCenter.UI.Views
{
    public partial class LoginWindow : Window
    {
        private int _attempts = 0;
        private const int MaxAttempts = 5;

        public LoginWindow() => InitializeComponent();

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            ErrorBorder.Visibility = Visibility.Collapsed;

            string username = TxtUsername.Text.Trim();
            string password = TxtPassword.Password;

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                ShowError("Введіть логін та пароль");
                return;
            }

            var (success, role) = await App.SecurityService
                .LoginAsync(username, password);

            if (!success)
            {
                _attempts++;
                TxtPassword.Clear();

                if (_attempts >= MaxAttempts)
                {
                    ShowError($"Забагато спроб. Спробуйте пізніше.");
                    TxtUsername.IsEnabled = false;
                    TxtPassword.IsEnabled = false;
                    return;
                }

                ShowError(
                    $"Невірний логін або пароль. " +
                    $"Спроба {_attempts} з {MaxAttempts}");
                TxtPassword.Focus();
                return;
            }

            CurrentUser.Login(username, role);
            new MainWindow().Show();
            Close();
        }

        private void ShowError(string msg)
        {
            TxtError.Text = msg;
            ErrorBorder.Visibility = Visibility.Visible;
        }
    }
}