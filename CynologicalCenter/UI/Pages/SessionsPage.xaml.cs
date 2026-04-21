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
using CynologicalCenter.Models;

namespace CynologicalCenter.UI.Pages
{
    public partial class SessionsPage : Page
    {
        private List<Session> _allSessions = new();
        private Session? _selected;
        public SessionsPage() => InitializeComponent();

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CmbStatus.Items.Add("Всі");
            CmbStatus.Items.Add("Заплановано");
            CmbStatus.Items.Add("Виконано");
            CmbStatus.Items.Add("Скасовано");
            CmbStatus.SelectedIndex = 0;

            await LoadDataAsync();
        }

        private async System.Threading.Tasks.Task LoadDataAsync()
        {
            try
            {
                _allSessions = await App.SessionService.GetAllAsync();
                ApplyFilter();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Помилка завантаження: {ex.Message}");
            }
        }

        private void ApplyFilter()
        {
            string status = CmbStatus.SelectedItem?.ToString() ?? "Всі";
            GridSessions.ItemsSource = status == "Всі"
                ? _allSessions
                : _allSessions.FindAll(s => s.Status == status);
        }

        private void CmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => ApplyFilter();

        private void GridSessions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selected = GridSessions.SelectedItem as Session;
            bool isPlanned = _selected?.Status == "Заплановано";
            BtnComplete.IsEnabled = isPlanned;
            BtnCancel.IsEnabled = isPlanned;
        }

        private void BtnEnroll_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CynologicalCenter.UI.Dialogs.EnrollDialog();
            if (dialog.ShowDialog() == true)
                _ = LoadDataAsync();
        }

        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) return;
            var dialog = new CynologicalCenter.UI.Dialogs.CompleteSessionDialog(_selected.SessionId);
            if (dialog.ShowDialog() == true)
                _ = LoadDataAsync();
        }

        private async void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) return;

            var result = MessageBox.Show(
                $"Скасувати заняття #{_selected.SessionId}?",
                "Підтвердження",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            var (success, msg) = await App.SessionService.CancelAsync(_selected.SessionId);
            MessageBox.Show(msg, success ? "Успішно" : "Помилка",
                MessageBoxButton.OK,
                success ? MessageBoxImage.Information : MessageBoxImage.Error);

            if (success)
                await LoadDataAsync();
        }
    }
}
