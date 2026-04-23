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
using CynologicalCenter.Helpers;
using CynologicalCenter.Models;

namespace CynologicalCenter.UI.Dialogs
{
    public partial class OwnerProfileDialog : Window
    {
        private readonly int _ownerId;
        private Owner? _owner;

        public OwnerProfileDialog(int ownerId)
        {
            InitializeComponent();
            _ownerId = ownerId;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _owner = await App.OwnerService.GetByIdAsync(_ownerId);
                if (_owner == null) return;

                var parts = _owner.FullName.Split(' ');
                TxtInitials.Text = parts.Length >= 2
                    ? $"{parts[0][0]}{parts[1][0]}".ToUpper()
                    : _owner.FullName[0].ToString().ToUpper();

                TxtName.Text = _owner.FullName;
                TxtStatus.Text = _owner.IsActive ? "Активний клієнт"
                                                  : "Деактивований";
                TxtPhone.Text = _owner.Phone ?? "—";
                TxtEmail.Text = _owner.Email ?? "—";

                ActionButtons.Visibility = RoleAccess.CanManageClients
                    ? Visibility.Visible : Visibility.Collapsed;
                BtnDeactivate.IsEnabled = RoleAccess.CanDeactivateClients
                                           && _owner.IsActive;

                var dogs = await App.Dogs.GetByOwnerIdAsync(_ownerId);
                WrapDogs.Children.Clear();

                foreach (var dog in dogs)
                {
                    var card = CreateDogCard(dog);
                    WrapDogs.Children.Add(card);
                }

                if (dogs.Count == 0)
                {
                    WrapDogs.Children.Add(new TextBlock
                    {
                        Text = "Собак не знайдено",
                        FontSize = 13,
                        Foreground = new SolidColorBrush(
                            Color.FromRgb(107, 114, 128))
                    });
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }

            AddDogPanel.Visibility = RoleAccess.CanManageClients 
                ? Visibility.Visible : Visibility.Collapsed;
        }

        private Border CreateDogCard(Dog dog)
        {
            var card = new Border
            {
                Width = 160,
                Margin = new Thickness(0, 0, 12, 12),
                Background = Brushes.White,
                CornerRadius = new CornerRadius(10),
                Cursor = Cursors.Hand,
                Effect = (System.Windows.Media.Effects.Effect)
                    FindResource("CardShadow")
            };

            var photoContainer = new Border
            {
                Height = 100,
                Background = new SolidColorBrush(
                    Color.FromRgb(237, 233, 254)),
                CornerRadius = new CornerRadius(10, 10, 0, 0)
            };

            bool photoLoaded = false;
            if (dog.PhotoPath != null)
            {
                string fullPath = Helpers.PhotoHelper.GetFullPath(
                    dog.PhotoPath, Helpers.PhotoHelper.DefaultDog);
                if (System.IO.File.Exists(fullPath))
                {
                    try
                    {
                        var bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.UriSource = new Uri(fullPath);
                        bmp.CacheOption = BitmapCacheOption.OnLoad;
                        bmp.DecodePixelWidth = 160;
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
                    FontSize = 32,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }

            var info = new StackPanel { Margin = new Thickness(12, 8, 12, 12) };

            info.Children.Add(new TextBlock
            {
                Text = dog.Nickname,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(
                    Color.FromRgb(26, 29, 46))
            });

            info.Children.Add(new TextBlock
            {
                Text = dog.BreedName ?? "Порода невідома",
                FontSize = 11,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(
                    Color.FromRgb(107, 114, 128)),
                TextWrapping = TextWrapping.Wrap
            });

            var stack = new StackPanel();
            stack.Children.Add(photoContainer);
            stack.Children.Add(info);
            card.Child = stack;

            card.MouseLeftButtonUp += (s, e) =>
            {
                var dialog = new DogProfileDialog(dog.DogId);
                dialog.ShowDialog();
            };

            return card;
        }

        private void BtnAddDog_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new DogEditDialog();
            if (dialog.ShowDialog() == true)
                Window_Loaded(sender, new RoutedEventArgs());
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OwnerEditDialog(_ownerId);
            if (dialog.ShowDialog() == true)
                Window_Loaded(sender, new RoutedEventArgs());
        }

        private async void BtnDeactivate_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                $"Деактивувати клієнта {_owner?.FullName}?",
                "Підтвердження",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            var (success, msg) = await App.OwnerService
                .DeactivateAsync(_ownerId);
            MessageBox.Show(msg,
                success ? "Успішно" : "Помилка",
                MessageBoxButton.OK,
                success ? MessageBoxImage.Information
                        : MessageBoxImage.Error);

            if (success) Close();
        }
    }
}
