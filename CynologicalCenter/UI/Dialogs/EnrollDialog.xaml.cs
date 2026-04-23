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

namespace CynologicalCenter.UI.Dialogs
{
    public partial class EnrollDialog : Window
    {
        public EnrollDialog() => InitializeComponent();

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DpDate.SelectedDate = DateTime.Today;
            CmbDog.ItemsSource = await App.Dogs.GetAllAsync();
            CmbTrainer.ItemsSource = await App.TrainerService.GetAllAsync();
        }

        private async void CmbTrainer_SelectionChanged(object sender,
            System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CmbTrainer.SelectedValue == null)
            {
                CmbCourse.ItemsSource = null;
                CmbCourse.IsEnabled = false;
                return;
            }

            int trainerId = (int)CmbTrainer.SelectedValue;

            try
            {
                var courseIds = await App.TrainerService
                    .GetCourseIdsAsync(trainerId);
                var allCourses = await App.Courses.GetAllAsync();
                var trainerCourses = allCourses
                    .Where(c => courseIds.Contains(c.CourseId))
                    .ToList();

                CmbCourse.ItemsSource = trainerCourses;
                CmbCourse.IsEnabled = trainerCourses.Count > 0;
                CmbCourse.SelectedIndex = -1;

                if (trainerCourses.Count == 0)
                    TxtError.Text = "Цей тренер не має допущених курсів";
                else
                    TxtError.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }

        private async void BtnEnroll_Click(object sender, RoutedEventArgs e)
        {
            ErrorBorder.Visibility = Visibility.Collapsed;
            TxtError.Text = "";

            if (CmbDog.SelectedValue == null ||
                CmbTrainer.SelectedValue == null ||
                CmbCourse.SelectedValue == null ||
                DpDate.SelectedDate == null)
            {
                ShowError("Заповніть всі поля");
                return;
            }

            if (!TimeSpan.TryParse(TxtTime.Text, out TimeSpan time))
            {
                ShowError("Невірний формат часу (HH:mm)");
                return;
            }

            DateTime dt = DpDate.SelectedDate.Value.Date + time;

            var (success, msg) = await App.EnrollmentService.EnrollAsync(
                (int)CmbDog.SelectedValue,
                (int)CmbTrainer.SelectedValue,
                (int)CmbCourse.SelectedValue,
                dt);

            if (success)
            {
                MessageBox.Show(msg, "Успішно",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                ShowError(msg);
            }
        }

        private void ShowError(string msg)
        {
            TxtError.Text = msg;
            ErrorBorder.Visibility = Visibility.Visible;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}