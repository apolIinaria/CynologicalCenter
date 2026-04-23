using CynologicalCenter.Helpers;
using CynologicalCenter.Models;
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
    public partial class CourseEditDialog : Window
    {
        private readonly int? _courseId;
        public CourseEditDialog()
        {
            InitializeComponent();
            _courseId = null;
        }

        public CourseEditDialog(int courseId)
        {
            InitializeComponent();
            _courseId = courseId;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BtnDelete.Visibility = (_courseId.HasValue && RoleAccess.IsAdmin)
                ? Visibility.Visible : Visibility.Collapsed;

            if (_courseId.HasValue)
            {
                TxtTitle.Text = "Редагувати курс";
                var course = await App.Courses.GetByIdAsync(_courseId.Value);
                if (course != null)
                {
                    TxtName.Text = course.CourseName;
                    TxtDescription.Text = course.Description ?? "";
                    TxtPrice.Text = course.Price?.ToString() ?? "";
                    TxtMinAge.Text = course.MinAgeMonths?.ToString() ?? "";
                }
            }
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            TxtError.Text = "";

            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                TxtError.Text = "Назва є обов'язковою";
                return;
            }

            var course = new Course
            {
                CourseId = _courseId ?? 0,
                CourseName = TxtName.Text.Trim(),
                Description = string.IsNullOrWhiteSpace(TxtDescription.Text)
                               ? null : TxtDescription.Text.Trim(),
                Price = decimal.TryParse(TxtPrice.Text, out decimal p) ? p : null,
                MinAgeMonths = int.TryParse(TxtMinAge.Text, out int m) ? m : null
            };

            try
            {
                if (_courseId.HasValue)
                    await App.Courses.UpdateAsync(course);
                else
                    await App.Courses.AddAsync(course);

                DialogResult = true;
                Close();
            }
            catch (System.Exception ex)
            {
                TxtError.Text = $"Помилка: {ex.Message}";
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!_courseId.HasValue) return;

            var result = MessageBox.Show(
                $"Видалити курс {TxtName.Text}?\n\nЦе також видалить всі пов'язані записи.",
                "Підтвердження видалення",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                await App.Courses.DeleteAsync(_courseId.Value);
                MessageBox.Show("Курс видалено", "Успішно",
                    MessageBoxButton.OK, MessageBoxImage.Information);
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
