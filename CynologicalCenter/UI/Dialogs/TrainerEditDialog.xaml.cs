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
        public TrainerEditDialog()
        {
            InitializeComponent();
            _trainerId = null;
            AddSaveCancel();
        }

        public TrainerEditDialog(int trainerId)
        {
            InitializeComponent();
            _trainerId = trainerId;
            AddSaveCancel();
        }

        private void AddSaveCancel()
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 8, 0, 0)
            };
            var txtError = new TextBlock
            {
                Name = "TxtError",
                Foreground = System.Windows.Media.Brushes.Red,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 12, 0)
            };
            var btnCancel = new Button
            {
                Content = "Скасувати",
                Height = 32,
                Padding = new Thickness(16, 0, 16, 0),
                Margin = new Thickness(0, 0, 8, 0)
            };
            var btnSave = new Button
            {
                Content = "Зберегти",
                Height = 32,
                Padding = new Thickness(16, 0, 16, 0),
                Background = System.Windows.Media.Brushes.DodgerBlue,
                Foreground = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(0)
            };
            btnCancel.Click += (s, e) => { DialogResult = false; Close(); };
            btnSave.Click += BtnSave_Click;
            panel.Children.Add(txtError);
            panel.Children.Add(btnCancel);
            panel.Children.Add(btnSave);

            var grid = Content as Grid;
            grid?.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Grid.SetRow(panel, grid!.RowDefinitions.Count - 1);
            grid.Children.Add(panel);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var courses = await App.Courses.GetAllAsync();
            LstCourses.ItemsSource = courses;

            if (_trainerId.HasValue)
            {
                TxtTitle.Text = "Редагувати тренера";
                var trainer = await App.TrainerService.GetByIdAsync(_trainerId.Value);
                if (trainer != null)
                {
                    TxtFullName.Text = trainer.FullName;
                    TxtPhone.Text = trainer.Phone ?? "";
                    TxtEmail.Text = trainer.Email ?? "";
                    TxtExperience.Text = trainer.ExperienceYears?.ToString() ?? "";
                    DpHireDate.SelectedDate = trainer.HireDate;
                }

                var courseIds = await App.TrainerService.GetCourseIdsAsync(_trainerId.Value);
                foreach (var item in LstCourses.Items)
                {
                    if (item is Course c && courseIds.Contains(c.CourseId))
                        LstCourses.SelectedItems.Add(item);
                }
            }
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtFullName.Text))
            {
                MessageBox.Show("ПІБ є обов'язковим");
                return;
            }

            var trainer = new Trainer
            {
                TrainerId = _trainerId ?? 0,
                FullName = TxtFullName.Text.Trim(),
                Phone = string.IsNullOrWhiteSpace(TxtPhone.Text) ? null : TxtPhone.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(TxtEmail.Text) ? null : TxtEmail.Text.Trim(),
                ExperienceYears = int.TryParse(TxtExperience.Text, out int exp) ? exp : null,
                HireDate = DpHireDate.SelectedDate
            };

            var (success, msg) = _trainerId.HasValue
                ? await App.TrainerService.UpdateAsync(trainer)
                : await App.TrainerService.AddAsync(trainer);

            if (!success) { MessageBox.Show(msg); return; }

            var selectedCourseIds = LstCourses.SelectedItems
                .Cast<Course>()
                .Select(c => c.CourseId)
                .ToList();

            int id = _trainerId ?? (await App.Trainers.GetAllAsync())
                .OrderByDescending(t => t.TrainerId).First().TrainerId;

            await App.TrainerService.SetCoursesAsync(id, selectedCourseIds);

            DialogResult = true;
            Close();
        }
    }
}
