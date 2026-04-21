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
using CynologicalCenter.Models;

namespace CynologicalCenter.UI.Pages
{
    public partial class CoursesPage : Page
    {
        private List<Course> _allCourses = new();
        private Course? _selected;
        public CoursesPage() => InitializeComponent();

        private async void Page_Loaded(object sender, RoutedEventArgs e)
            => await LoadDataAsync();

        private async System.Threading.Tasks.Task LoadDataAsync()
        {
            try
            {
                _allCourses = await App.Courses.GetAllAsync();
                GridCourses.ItemsSource = _allCourses;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Помилка завантаження: {ex.Message}");
            }
        }

        private void GridCourses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selected = GridCourses.SelectedItem as Course;
            bool has = _selected != null;
            BtnEdit.IsEnabled = has;
            BtnDelete.IsEnabled = has;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CynologicalCenter.UI.Dialogs.CourseEditDialog();
            if (dialog.ShowDialog() == true)
                _ = LoadDataAsync();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) return;
            var dialog = new CynologicalCenter.UI.Dialogs.CourseEditDialog(_selected.CourseId);
            if (dialog.ShowDialog() == true)
                _ = LoadDataAsync();
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) return;

            var result = MessageBox.Show(
                $"Видалити курс {_selected.CourseName}?",
                "Підтвердження",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                await App.Courses.DeleteAsync(_selected.CourseId);
                MessageBox.Show("Курс видалено", "Успішно",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadDataAsync();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
