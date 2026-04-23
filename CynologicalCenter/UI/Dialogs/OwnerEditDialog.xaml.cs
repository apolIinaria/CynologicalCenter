using CynologicalCenter.Models;
using Microsoft.VisualBasic;
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
using Microsoft.VisualBasic;

namespace CynologicalCenter.UI.Dialogs
{
    public partial class OwnerEditDialog : Window
    {
        private readonly int? _ownerId;

        public OwnerEditDialog()
        {
            InitializeComponent();
            _ownerId = null;
        }

        public OwnerEditDialog(int ownerId)
        {
            InitializeComponent();
            _ownerId = ownerId;
            DogSection.Visibility = Visibility.Collapsed;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CmbBreed.ItemsSource = await App.Breeds.GetAllAsync();

            if (_ownerId.HasValue)
            {
                TxtTitle.Text = "Редагувати клієнта";
                var owner = await App.OwnerService.GetByIdAsync(_ownerId.Value);
                if (owner != null)
                {
                    TxtFullName.Text = owner.FullName;
                    TxtPhone.Text = owner.Phone ?? "";
                    TxtEmail.Text = owner.Email ?? "";
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

            if (string.IsNullOrWhiteSpace(TxtFullName.Text))
            {
                ShowError("ПІБ є обов'язковим полем");
                return;
            }

            var owner = new Owner
            {
                OwnerId = _ownerId ?? 0,
                FullName = TxtFullName.Text.Trim(),
                Phone = string.IsNullOrWhiteSpace(TxtPhone.Text)
                           ? null : TxtPhone.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(TxtEmail.Text)
                           ? null : TxtEmail.Text.Trim()
            };

            (bool success, string msg) result = _ownerId.HasValue
                ? await App.OwnerService.UpdateAsync(owner)
                : await App.OwnerService.AddAsync(owner);

            if (!result.success)
            {
                ShowError(result.msg);
                return;
            }

            if (!_ownerId.HasValue &&
                !string.IsNullOrWhiteSpace(TxtDogNickname.Text))
            {
                var owners = await App.Owners.GetAllAsync();
                var newOwner = owners
                    .OrderByDescending(o => o.OwnerId)
                    .FirstOrDefault();

                if (newOwner != null)
                {
                    var dog = new Dog
                    {
                        Nickname = TxtDogNickname.Text.Trim(),
                        BreedId = CmbBreed.SelectedValue as int?,
                        OwnerId = newOwner.OwnerId,
                        BirthDate = DpDogBirth.SelectedDate,
                        LastVaccination = DpVaccination.SelectedDate
                    };

                    try
                    {
                        await App.Dogs.AddAsync(dog);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(
                            $"Клієнта додано, але помилка при додаванні собаки:\n{ex.Message}",
                            "Увага", MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                }
            }

            DialogResult = true;
            Close();
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
