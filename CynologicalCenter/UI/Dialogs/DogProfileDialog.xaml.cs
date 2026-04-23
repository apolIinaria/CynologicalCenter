using CynologicalCenter.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
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
    public partial class DogProfileDialog : Window
    {
        private readonly int _dogId;
        private int? _ownerId;
        public DogProfileDialog(int dogId)
        {
            InitializeComponent();
            _dogId = dogId;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BtnUploadPhoto.Visibility = RoleAccess.CanManageClients
                ? Visibility.Visible : Visibility.Collapsed;
            BtnEdit.Visibility = RoleAccess.CanManageClients
                ? Visibility.Visible : Visibility.Collapsed;

            try
            {
                var ds = await App.ReportService.GetDogReportAsync(_dogId);
                if (ds.Tables.Count == 0) return;

                if (ds.Tables[0].Rows.Count > 0)
                {
                    var row = ds.Tables[0].Rows[0];

                    TxtDogName.Text = row["nickname"]?.ToString() ?? "";
                    TxtDogBreed.Text = row["breed_name"]?.ToString() ?? "";
                    TxtAge.Text = row["вік"]?.ToString() ?? "";
                    TxtOwner.Text = row["власник"]?.ToString() ?? "";
                    TxtPhone.Text = row["phone"]?.ToString() ?? "";

                    string vaccStatus =
                        row["статус_вакцинації"]?.ToString() ?? "";
                    TxtVaccStatus.Text = vaccStatus switch
                    {
                        "Дійсна" => "Дійсна",
                        "Спливає" => "Cпливає",
                        "Прострочена" => "Прострочена",
                        _ => "Невідомо"
                    };
                    TxtVaccStatus.Foreground = vaccStatus switch
                    {
                        "Дійсна" => new SolidColorBrush(
                            Color.FromRgb(16, 185, 129)),
                        "Спливає" => new SolidColorBrush(
                            Color.FromRgb(245, 158, 11)),
                        "Прострочена" => new SolidColorBrush(
                            Color.FromRgb(239, 68, 68)),
                        _ => Brushes.Gray
                    };

                    string vaccDate =
                        row["last_vaccination"]?.ToString() ?? "";
                    TxtVaccDate.Text = !string.IsNullOrEmpty(vaccDate)
                        ? $"{Convert.ToDateTime(vaccDate):dd.MM.yyyy}"
                        : "Не вказано";
                }

                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    var row = ds.Tables[1].Rows[0];
                    TxtTotalSessions.Text =
                        row["всього_занять"]?.ToString() ?? "0";
                    TxtCompletedSessions.Text =
                        row["виконано"]?.ToString() ?? "0";
                    TxtAvgGrade.Text =
                        row["середня_оцінка"]?.ToString() ?? "—";
                    TxtRating.Text =
                        row["загальний_рейтинг"]?.ToString() ?? "—";
                }

                if (ds.Tables.Count > 2)
                    GridHistory.ItemsSource = ds.Tables[2].DefaultView;

                var dogData = await App.Dogs.GetByIdAsync(_dogId);
                _ownerId = dogData?.OwnerId;
                LoadPhoto(dogData?.PhotoPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження: {ex.Message}");
            }
        }

        private void LoadPhoto(string? photoPath)
        {
            if (photoPath == null) return;

            string fullPath = PhotoHelper.GetFullPath(
                photoPath, PhotoHelper.DefaultDog);

            if (!System.IO.File.Exists(fullPath)) return;

            try
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(fullPath);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.DecodePixelWidth = 128;
                bmp.EndInit();
                bmp.Freeze();

                DogPhotoBrush.ImageSource = bmp;
                DogPhotoEllipse.Visibility = Visibility.Visible;
                TxtDogEmoji.Visibility = Visibility.Collapsed;
            }
            catch { }
        }

        private async void BtnUploadPhoto_Click(object sender,
            RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Оберіть фото собаки (400x400 JPG/PNG)",
                Filter = "Зображення|*.jpg;*.jpeg;*.png",
                Multiselect = false
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                string relativePath = PhotoHelper
                    .SaveDogPhoto(_dogId, dialog.FileName);
                await App.Dogs.UpdatePhotoAsync(_dogId, relativePath);
                LoadPhoto(relativePath);

                MessageBox.Show("Фото успішно збережено!",
                    "Успішно", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}",
                    "Помилка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new DogEditDialog(_dogId);
            if (dialog.ShowDialog() == true)
                Window_Loaded(sender, new RoutedEventArgs());
        }

        private void TxtOwner_Click(object sender,
            System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_ownerId == null) return;
            var dialog = new OwnerProfileDialog(_ownerId.Value);
            dialog.ShowDialog();
        }
    }
}
