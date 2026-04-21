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
    public partial class TrainersPage : Page
    {
        private List<Trainer> _allTrainers = new();
        private Trainer? _selected;
        public TrainersPage() => InitializeComponent();

        private async void Page_Loaded(object sender, RoutedEventArgs e)
            => await LoadDataAsync();

        private async System.Threading.Tasks.Task LoadDataAsync()
        {
            try
            {
                _allTrainers = await App.TrainerService.GetAllAsync();
                GridTrainers.ItemsSource = _allTrainers;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Помилка завантаження: {ex.Message}");
            }
        }

        private void GridTrainers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selected = GridTrainers.SelectedItem as Trainer;
            bool has = _selected != null;
            BtnEdit.IsEnabled = has;
            BtnDelete.IsEnabled = has;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CynologicalCenter.UI.Dialogs.TrainerEditDialog();
            if (dialog.ShowDialog() == true)
                _ = LoadDataAsync();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) return;
            var dialog = new CynologicalCenter.UI.Dialogs.TrainerEditDialog(_selected.TrainerId);
            if (dialog.ShowDialog() == true)
                _ = LoadDataAsync();
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) return;

            var result = MessageBox.Show(
                $"Видалити тренера {_selected.FullName}?",
                "Підтвердження",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            var (success, msg) = await App.TrainerService.DeleteAsync(_selected.TrainerId);
            MessageBox.Show(msg, success ? "Успішно" : "Помилка",
                MessageBoxButton.OK,
                success ? MessageBoxImage.Information : MessageBoxImage.Error);

            if (success)
                await LoadDataAsync();
        }

        private void BtnKpi_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CynologicalCenter.UI.Dialogs.TrainerKpiDialog();
            dialog.ShowDialog();
        }
    }
}
