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

namespace CynologicalCenter.UI.Dialogs
{
    public partial class CompleteSessionDialog : Window
    {
        private readonly int _sessionId;
        public CompleteSessionDialog(int sessionId)
        {
            InitializeComponent();
            _sessionId = sessionId;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var session = await App.Sessions.GetByIdAsync(_sessionId);
            if (session != null)
                TxtTitle.Text =
                    $"Заняття #{_sessionId} — {session.DogNickname} " +
                    $"({session.SessionDatetime:dd.MM.yyyy HH:mm})";
        }

        private async void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            TxtError.Text = "";

            if (!decimal.TryParse(TxtGrade.Text, out decimal grade))
            {
                TxtError.Text = "Введіть оцінку від 1 до 10";
                return;
            }

            var (success, msg) = await App.SessionService.CompleteAsync(
                _sessionId, grade, TxtComment.Text.Trim());

            if (success)
            {
                MessageBox.Show(msg, "Успішно",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                TxtError.Text = msg;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
