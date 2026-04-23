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
using Microsoft.Win32;
using CynologicalCenter.Helpers;

namespace CynologicalCenter.UI.Dialogs
{
    public partial class TrainerProfileDialog : Window
    {
        private readonly int _trainerId;

        public TrainerProfileDialog(int trainerId)
        {
            InitializeComponent();
            _trainerId = trainerId;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BtnUploadPhoto.Visibility = RoleAccess.CanManageTrainers
                ? Visibility.Visible : Visibility.Collapsed;
            BtnDeleteTrainer.Visibility = RoleAccess.IsAdmin
                ? Visibility.Visible : Visibility.Collapsed;
            BtnEdit.Visibility = RoleAccess.CanManageTrainers
                ? Visibility.Visible : Visibility.Collapsed;

            try
            {
                var trainer = await App.TrainerService
                    .GetByIdAsync(_trainerId);
                if (trainer == null) return;

                var parts = trainer.FullName.Split(' ');
                TxtInitialsAvatar.Text = parts.Length >= 2
                    ? $"{parts[0][0]}{parts[1][0]}".ToUpper()
                    : trainer.FullName[0].ToString().ToUpper();

                TxtName.Text = trainer.FullName;
                TxtExperience.Text =
                    $"Досвід: {trainer.ExperienceYears ?? 0} років";
                TxtPhone.Text = trainer.Phone ?? "—";
                TxtEmail.Text = trainer.Email ?? "—";
                TxtHireDate.Text = trainer.HireDate.HasValue
                    ? trainer.HireDate.Value.ToString("dd.MM.yyyy")
                    : "—";

                var kpiList = await App.TrainerService.GetKpiAsync();
                var kpi = kpiList.FirstOrDefault(
                    k => k.TrainerId == _trainerId);
                TxtTotalSessions.Text =
                    kpi?.TotalSessions.ToString() ?? "0";
                TxtAvgGrade.Text = kpi?.AvgGrade.HasValue == true
                    ? $"{kpi.AvgGrade:N2}" : "—";
                TxtRating.Text = kpi != null ? $"#{kpi.Rating}" : "—";

                var courseIds = await App.TrainerService
                    .GetCourseIdsAsync(_trainerId);
                var allCourses = await App.Courses.GetAllAsync();
                CoursesList.ItemsSource = allCourses
                    .Where(c => courseIds.Contains(c.CourseId))
                    .ToList();

                LoadPhoto(trainer.PhotoPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }

        private void LoadPhoto(string? photoPath)
        {
            if (photoPath == null) return;

            string fullPath = PhotoHelper.GetFullPath(
                photoPath, PhotoHelper.DefaultTrainer);

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

                TrainerPhotoBrush.ImageSource = bmp;
                TrainerPhotoEllipse.Visibility = Visibility.Visible;
                TxtInitialsAvatar.Visibility = Visibility.Collapsed;
            }
            catch { }
        }

        private async void BtnUploadPhoto_Click(object sender,
            RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Оберіть фото тренера (400x400 JPG/PNG)",
                Filter = "Зображення|*.jpg;*.jpeg;*.png",
                Multiselect = false
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                string relativePath = PhotoHelper
                    .SaveTrainerPhoto(_trainerId, dialog.FileName);
                await App.Trainers.UpdatePhotoAsync(
                    _trainerId, relativePath);
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
        private async void BtnDeleteTrainer_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                $"Видалити тренера {TxtName.Text}?",
                "Підтвердження",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                var (success, msg) = await App.TrainerService
                    .DeleteAsync(_trainerId);
                MessageBox.Show(msg,
                    success ? "Успішно" : "Помилка",
                    MessageBoxButton.OK,
                    success ? MessageBoxImage.Information
                            : MessageBoxImage.Error);
                if (success) { DialogResult = true; Close(); }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new TrainerEditDialog(_trainerId);
            if (dialog.ShowDialog() == true)
                Window_Loaded(sender, new RoutedEventArgs());
        }
    }
}
