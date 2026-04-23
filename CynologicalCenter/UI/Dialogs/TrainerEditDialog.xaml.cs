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


namespace CynologicalCenter.UI.Dialogs
{
    public partial class TrainerEditDialog : Window
    {
        private readonly int? _trainerId;
        private List<Course> _courses = new();

        public TrainerEditDialog()
        {
            InitializeComponent();
            _trainerId = null;
        }

        public TrainerEditDialog(int trainerId)
        {
            InitializeComponent();
            _trainerId = trainerId;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _courses = await App.Courses.GetAllAsync();
            CoursesItemsControl.ItemsSource = _courses;

            if (_trainerId.HasValue)
            {
                TxtTitle.Text = "Редагувати тренера";
                var trainer = await App.TrainerService
                    .GetByIdAsync(_trainerId.Value);
                if (trainer != null)
                {
                    TxtFullName.Text = trainer.FullName;
                    TxtPhone.Text = trainer.Phone ?? "";
                    TxtEmail.Text = trainer.Email ?? "";
                    TxtExperience.Text =
                        trainer.ExperienceYears?.ToString() ?? "";
                    DpHireDate.SelectedDate = trainer.HireDate;
                }

                var courseIds = await App.TrainerService
                    .GetCourseIdsAsync(_trainerId.Value);

                Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Loaded,
                    new System.Action(() =>
                    {
                        foreach (var item in GetCheckBoxes())
                        {
                            if (item.Tag is int id &&
                                courseIds.Contains(id))
                                item.IsChecked = true;
                        }
                    }));
            }
        }

        private List<CheckBox> GetCheckBoxes()
        {
            var result = new List<CheckBox>();
            foreach (var item in CoursesItemsControl.Items)
            {
                var container = CoursesItemsControl
                    .ItemContainerGenerator
                    .ContainerFromItem(item) as ContentPresenter;
                if (container == null) continue;
                var cb = FindVisualChild<CheckBox>(container);
                if (cb != null) result.Add(cb);
            }
            return result;
        }

        private static T? FindVisualChild<T>(
            System.Windows.DependencyObject parent)
            where T : System.Windows.DependencyObject
        {
            for (int i = 0;
                 i < System.Windows.Media.VisualTreeHelper
                     .GetChildrenCount(parent); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper
                    .GetChild(parent, i);
                if (child is T t) return t;
                var found = FindVisualChild<T>(child);
                if (found != null) return found;
            }
            return null;
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            ErrorBorder.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(TxtFullName.Text))
            {
                ShowError("ПІБ є обов'язковим полем");
                return;
            }

            var trainer = new Trainer
            {
                TrainerId = _trainerId ?? 0,
                FullName = TxtFullName.Text.Trim(),
                Phone = string.IsNullOrWhiteSpace(TxtPhone.Text)
                                  ? null : TxtPhone.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(TxtEmail.Text)
                                  ? null : TxtEmail.Text.Trim(),
                ExperienceYears = int.TryParse(TxtExperience.Text,
                                  out int exp) ? exp : null,
                HireDate = DpHireDate.SelectedDate
            };

            var (success, msg) = _trainerId.HasValue
                ? await App.TrainerService.UpdateAsync(trainer)
                : await App.TrainerService.AddAsync(trainer);

            if (!success) { ShowError(msg); return; }

            var selectedIds = GetCheckBoxes()
                .Where(cb => cb.IsChecked == true)
                .Select(cb => (int)cb.Tag)
                .ToList();

            int id = _trainerId ?? (await App.Trainers.GetAllAsync())
                .OrderByDescending(t => t.TrainerId)
                .First().TrainerId;

            await App.TrainerService.SetCoursesAsync(id, selectedIds);

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
