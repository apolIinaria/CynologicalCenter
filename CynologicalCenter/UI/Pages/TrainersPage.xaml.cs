using CynologicalCenter.Helpers;
using CynologicalCenter.Models;
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
    public partial class TrainersPage : Page
    {
        private List<Trainer> _allTrainers = new();

        public TrainersPage() => InitializeComponent();

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            BtnAdd.IsEnabled = RoleAccess.CanManageTrainers;
            BtnAdd.Visibility = RoleAccess.CanManageTrainers
                                ? Visibility.Visible
                                : Visibility.Collapsed;
            await LoadDataAsync();
        }

        private async System.Threading.Tasks.Task LoadDataAsync()
        {
            try
            {
                _allTrainers = await App.TrainerService.GetAllAsync();
                await RenderCardsAsync(_allTrainers);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task RenderCardsAsync(
            List<Trainer> trainers)
        {
            WrapTrainers.Children.Clear();
            foreach (var t in trainers)
            {
                var card = CreateTrainerCard(t);
                WrapTrainers.Children.Add(card);
            }
            await System.Threading.Tasks.Task.CompletedTask;
        }

        private Border CreateTrainerCard(Trainer trainer)
        {
            var card = new Border
            {
                Width = 200,
                Margin = new Thickness(0, 0, 12, 12),
                Background = Brushes.White,
                CornerRadius = new CornerRadius(5),
                Cursor = Cursors.Hand,
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
            if (trainer.PhotoPath != null)
            {
                string fullPath = PhotoHelper.GetFullPath(
                    trainer.PhotoPath, PhotoHelper.DefaultTrainer);
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
                var parts = trainer.FullName.Split(' ');
                string initials = parts.Length >= 2
                    ? $"{parts[0][0]}{parts[1][0]}".ToUpper()
                    : trainer.FullName[0].ToString().ToUpper();

                photoContainer.Child = new TextBlock
                {
                    Text = initials,
                    FontSize = 36,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(
                        Color.FromRgb(63, 125, 106)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }

            var info = new StackPanel { Margin = new Thickness(14) };

            info.Children.Add(new TextBlock
            {
                Text = trainer.FullName,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(
                    Color.FromRgb(26, 29, 46)),
                TextWrapping = TextWrapping.Wrap
            });

            info.Children.Add(new TextBlock
            {
                Text = $"Років досвіду: {trainer.ExperienceYears ?? 0}",
                FontSize = 12,
                Foreground = new SolidColorBrush(
                    Color.FromRgb(107, 114, 128)),
                Margin = new Thickness(0, 4, 0, 0)
            });

            var stack = new StackPanel();
            stack.Children.Add(photoContainer);
            stack.Children.Add(info);
            card.Child = stack;

            card.MouseLeftButtonUp += (s, e) =>
            {
                var dialog = new CynologicalCenter.UI.Dialogs
                    .TrainerProfileDialog(trainer.TrainerId);
                dialog.ShowDialog();
                _ = LoadDataAsync();
            };

            return card;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CynologicalCenter.UI.Dialogs.TrainerEditDialog();
            if (dialog.ShowDialog() == true)
                _ = LoadDataAsync();
        }

        private void BtnKpi_Click(object sender, RoutedEventArgs e)
        {
            new CynologicalCenter.UI.Dialogs.TrainerKpiDialog().ShowDialog();
        }
    }
}
