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
    public partial class DogsPage : Page
    {
        private List<DogProfileViewModel> _allDogs = new();
        private DogProfileViewModel? _selected;

        public DogsPage() => InitializeComponent();

        private async void Page_Loaded(object sender, RoutedEventArgs e)
            => await LoadDataAsync();

        private async System.Threading.Tasks.Task LoadDataAsync()
        {
            try
            {
                _allDogs = await App.Dogs.GetProfilesViewAsync();
                GridDogs.ItemsSource = _allDogs;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Помилка завантаження: {ex.Message}");
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string q = TxtSearch.Text.ToLower();
            GridDogs.ItemsSource = string.IsNullOrWhiteSpace(q)
                ? _allDogs
                : _allDogs.Where(d =>
                    d.Nickname.ToLower().Contains(q) ||
                    d.Breed.ToLower().Contains(q) ||
                    d.Owner.ToLower().Contains(q)).ToList();
        }

        private void GridDogs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selected = GridDogs.SelectedItem as DogProfileViewModel;
            bool has = _selected != null;
            BtnEdit.IsEnabled = has;
            BtnProfile.IsEnabled = has;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CynologicalCenter.UI.Dialogs.DogEditDialog();
            if (dialog.ShowDialog() == true)
                _ = LoadDataAsync();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) return;
            var dialog = new CynologicalCenter.UI.Dialogs.DogEditDialog(_selected.DogId);
            if (dialog.ShowDialog() == true)
                _ = LoadDataAsync();
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) return;
            var dialog = new CynologicalCenter.UI.Dialogs.DogProfileDialog(_selected.DogId);
            dialog.ShowDialog();
        }
    }
}
