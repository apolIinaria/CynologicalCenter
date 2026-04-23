using CynologicalCenter.Helpers;
using CynologicalCenter.Models.ViewModels;
using System;
using System.IO;
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

namespace CynologicalCenter.UI.Pages
{
    public partial class DogsPage : Page
    {
        private List<DogProfileViewModel> _allDogs = new();

        public DogsPage() => InitializeComponent();

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            BtnAdd.IsEnabled = RoleAccess.CanManageClients;
            BtnAdd.Visibility = RoleAccess.CanManageClients
                                ? Visibility.Visible
                                : Visibility.Collapsed;
            await LoadDataAsync();
        }

        private async System.Threading.Tasks.Task LoadDataAsync()
        {
            try
            {
                _allDogs = await App.Dogs.GetProfilesViewAsync();
                TxtCount.Text = _allDogs.Count.ToString();
                await RenderCardsAsync(_allDogs);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }
        private async System.Threading.Tasks.Task RenderCardsAsync(
            List<DogProfileViewModel> dogs)
        {
            WrapDogs.Children.Clear();

            foreach (var dog in dogs)
            {
                var dogData = await App.Dogs.GetByIdAsync(dog.DogId);
                var card = CreateDogCard(dog, dogData?.PhotoPath);
                WrapDogs.Children.Add(card);
            }
        }

        private Border CreateDogCard(DogProfileViewModel dog,
            string? photoPath)
        {
            var card = new Border
            {
                Width = 200,
                Margin = new Thickness(0, 0, 12, 12),
                Background = Brushes.White,
                CornerRadius = new CornerRadius(5),
                Cursor = RoleAccess.IsGuest
                               ? Cursors.Arrow : Cursors.Hand,
                Effect = (System.Windows.Media.Effects.Effect)
                    FindResource("CardShadow")
            };

            var photoContainer = new Border
            {
                Height = 120,
                Background = new SolidColorBrush(
                    Color.FromRgb(232, 240, 238)),
                CornerRadius = new CornerRadius(5, 5, 0, 0)
            };

            bool photoLoaded = false;
            if (photoPath != null)
            {
                string fullPath = PhotoHelper.GetFullPath(
                    photoPath, PhotoHelper.DefaultDog);
                if (File.Exists(fullPath))
                {
                    try
                    {
                        var bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.UriSource = new System.Uri(fullPath);
                        bmp.CacheOption = BitmapCacheOption.OnLoad;
                        bmp.DecodePixelWidth = 200;
                        bmp.EndInit();
                        bmp.Freeze();

                        photoContainer.Background = new ImageBrush
                        {
                            ImageSource = bmp,
                            Stretch = Stretch.UniformToFill
                        };
                        photoLoaded = true;
                    }
                    catch { }
                }
            }

            if (!photoLoaded)
            {
                photoContainer.Child = new TextBlock
                {
                    Text = "🐕",
                    FontSize = 48,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }

            var info = new StackPanel { Margin = new Thickness(14) };

            info.Children.Add(new TextBlock
            {
                Text = dog.Nickname,
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(
                    Color.FromRgb(26, 29, 46))
            });

            info.Children.Add(new TextBlock
            {
                Text = dog.Breed,
                FontSize = 12,
                Foreground = new SolidColorBrush(
                    Color.FromRgb(107, 114, 128)),
                Margin = new Thickness(0, 2, 0, 6),
                TextWrapping = TextWrapping.Wrap
            });

            string vaccText = dog.DaysSinceVaccination > 365
                ? "Вакцинація прострочена"
                : dog.DaysSinceVaccination > 300
                ? "Вакцинація спливає"
                : "Вакцинація дійсна";

            info.Children.Add(new TextBlock
            {
                Text = vaccText,
                FontSize = 11
            });

            var stack = new StackPanel();
            stack.Children.Add(photoContainer);
            stack.Children.Add(info);
            card.Child = stack;

            card.MouseLeftButtonUp += (s, e) =>
            {
                if (RoleAccess.IsGuest) return;
                var dialog = new CynologicalCenter.UI.Dialogs
                    .DogProfileDialog(dog.DogId);
                dialog.ShowDialog();
                _ = LoadDataAsync();
            };

            return card;
        }

        private void TxtSearch_TextChanged(object sender,
            TextChangedEventArgs e)
        {
            string q = TxtSearch.Text.ToLower();
            var filtered = string.IsNullOrWhiteSpace(q)
                ? _allDogs
                : _allDogs.Where(d =>
                    d.Nickname.ToLower().Contains(q) ||
                    d.Breed.ToLower().Contains(q) ||
                    d.Owner.ToLower().Contains(q)).ToList();
            _ = RenderCardsAsync(filtered);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CynologicalCenter.UI.Dialogs.DogEditDialog();
            if (dialog.ShowDialog() == true)
                _ = LoadDataAsync();
        }
    }
}
