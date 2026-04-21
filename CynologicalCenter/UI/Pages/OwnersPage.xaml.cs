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
using CynologicalCenter.Models.ViewModels;

namespace CynologicalCenter.UI.Pages
{
    public partial class OwnersPage : Page
    {
        private List<ActiveClientViewModel> _allOwners = new();
        private ActiveClientViewModel? _selected;
        public OwnersPage() => InitializeComponent();

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }

        private async System.Threading.Tasks.Task LoadDataAsync()
        {
            try
            {
                _allOwners = await App.OwnerService.GetActiveClientsViewAsync();
                GridOwners.ItemsSource = _allOwners;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Помилка завантаження: {ex.Message}");
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string q = TxtSearch.Text.ToLower();
            GridOwners.ItemsSource = string.IsNullOrWhiteSpace(q)
                ? _allOwners
                : _allOwners.Where(o =>
                    o.FullName.ToLower().Contains(q) ||
                    (o.Phone ?? "").Contains(q) ||
                    (o.Email ?? "").ToLower().Contains(q)).ToList();
        }

        private void GridOwners_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selected = GridOwners.SelectedItem as ActiveClientViewModel;
            bool hasSelection = _selected != null;
            BtnEdit.IsEnabled = hasSelection;
            BtnDeactivate.IsEnabled = hasSelection;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CynologicalCenter.UI.Dialogs.OwnerEditDialog();
            if (dialog.ShowDialog() == true)
                _ = LoadDataAsync();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) return;
            var dialog = new CynologicalCenter.UI.Dialogs.OwnerEditDialog(_selected.OwnerId);
            if (dialog.ShowDialog() == true)
                _ = LoadDataAsync();
        }

        private async void BtnDeactivate_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) return;

            var result = MessageBox.Show(
                $"Деактивувати клієнта {_selected.FullName}?",
                "Підтвердження",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            var (success, msg) = await App.OwnerService.DeactivateAsync(_selected.OwnerId);
            MessageBox.Show(msg, success ? "Успішно" : "Помилка",
                MessageBoxButton.OK,
                success ? MessageBoxImage.Information : MessageBoxImage.Error);

            if (success)
                await LoadDataAsync();
        }
    }
}
