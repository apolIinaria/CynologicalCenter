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
using CynologicalCenter.Models;
using Microsoft.VisualBasic;

namespace CynologicalCenter.UI.Dialogs
{
    public partial class DogEditDialog : Window
    {
        private readonly int? _dogId;
        private readonly int? _preselectedOwnerId;
        public DogEditDialog()
        {
            InitializeComponent();
            _dogId = null;
            _preselectedOwnerId = null;
        }

        public DogEditDialog(int dogId)
        {
            InitializeComponent();
            _dogId = dogId;
            _preselectedOwnerId = null;
        }

        public DogEditDialog(int? dogId = null, int? ownerId = null)
        {
            InitializeComponent();
            _dogId = dogId;
            _preselectedOwnerId = ownerId;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CmbBreed.ItemsSource = await App.Breeds.GetAllAsync();
            CmbOwner.ItemsSource = await App.Owners.GetActiveAsync();

            if (_preselectedOwnerId.HasValue)
            {
                CmbOwner.SelectedValue = _preselectedOwnerId.Value;
                CmbOwner.IsEnabled = false;
            }

            if (_dogId.HasValue)
            {
                TxtTitle.Text = "Редагувати собаку";
                var dog = await App.Dogs.GetByIdAsync(_dogId.Value);
                if (dog != null)
                {
                    TxtNickname.Text = dog.Nickname;
                    CmbBreed.SelectedValue = dog.BreedId;
                    CmbOwner.SelectedValue = dog.OwnerId;
                    DpBirthDate.SelectedDate = dog.BirthDate;
                    DpVaccination.SelectedDate = dog.LastVaccination;
                }
            }
        }

        private async void BtnAddBreed_Click(object sender, RoutedEventArgs e)
        {
            string input = Interaction.InputBox(
                "Введіть назву нової породи:",
                "Нова порода", "");

            if (string.IsNullOrWhiteSpace(input)) return;

            try
            {
                await App.Breeds.AddAsync(new Models.Breed
                {
                    BreedName = input.Trim()
                });

                var breeds = await App.Breeds.GetAllAsync();
                CmbBreed.ItemsSource = breeds;

                var newBreed = breeds
                    .OrderByDescending(b => b.BreedId)
                    .FirstOrDefault();
                if (newBreed != null)
                    CmbBreed.SelectedValue = newBreed.BreedId;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            ErrorBorder.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(TxtNickname.Text))
            {
                ShowError("Кличка є обов'язковою");
                return;
            }

            var dog = new Dog
            {
                DogId = _dogId ?? 0,
                Nickname = TxtNickname.Text.Trim(),
                BreedId = CmbBreed.SelectedValue as int?,
                OwnerId = CmbOwner.SelectedValue as int?,
                BirthDate = DpBirthDate.SelectedDate,
                LastVaccination = DpVaccination.SelectedDate
            };

            try
            {
                if (_dogId.HasValue)
                    await App.Dogs.UpdateAsync(dog);
                else
                    await App.Dogs.AddAsync(dog);

                DialogResult = true;
                Close();
            }
            catch (System.Exception ex)
            {
                ShowError($"Помилка: {ex.Message}");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ShowError(string msg)
        {
            TxtError.Text = msg;
            ErrorBorder.Visibility = Visibility.Visible;
        }
    }
}
