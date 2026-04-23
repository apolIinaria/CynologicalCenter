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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CynologicalCenter.UI.Pages
{
    public partial class CoursesPage : Page
    {
        private List<Course> _allCourses = new();

        public CoursesPage() => InitializeComponent();

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            BtnAdd.IsEnabled = RoleAccess.IsAdmin;
            BtnAdd.Visibility = RoleAccess.IsAdmin
                                ? Visibility.Visible
                                : Visibility.Collapsed;
            await LoadDataAsync();
        }

        private async System.Threading.Tasks.Task LoadDataAsync()
        {
            try
            {
                _allCourses = await App.Courses.GetAllAsync();
                RenderCards(_allCourses);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }

        private void RenderCards(List<Course> courses)
        {
            WrapCourses.Children.Clear();
            foreach (var c in courses)
                WrapCourses.Children.Add(CreateCourseCard(c));
        }

        private Border CreateCourseCard(Course course)
        {
            var card = new Border
            {
                Width = 240,
                Margin = new Thickness(0, 0, 12, 12),
                Background = Brushes.White,
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(20),
                Cursor = RoleAccess.IsAdmin
                               ? Cursors.Hand : Cursors.Arrow,
                Effect = (System.Windows.Media.Effects.Effect)
                    FindResource("CardShadow")
            };

            var name = new TextBlock
            {
                Text = course.CourseName,
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(
                    Color.FromRgb(26, 29, 46)),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 8)
            };

            var desc = new TextBlock
            {
                Text = course.Description ?? "Опис відсутній",
                FontSize = 12,
                Foreground = new SolidColorBrush(
                    Color.FromRgb(107, 114, 128)),
                TextWrapping = TextWrapping.Wrap,
                MaxHeight = 48,
                Margin = new Thickness(0, 0, 0, 12)
            };

            var detailsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            if (course.Price.HasValue)
            {
                var priceBadge = new Border
                {
                    Background = new SolidColorBrush(
                        Color.FromRgb(209, 250, 229)),
                    CornerRadius = new CornerRadius(5),
                    Padding = new Thickness(10, 4, 10, 4),
                    Margin = new Thickness(0, 0, 8, 0)
                };
                priceBadge.Child = new TextBlock
                {
                    Text = $"{course.Price:N0} грн",
                    FontSize = 11,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(
                        Color.FromRgb(16, 185, 129))
                };
                detailsPanel.Children.Add(priceBadge);
            }

            if (course.MinAgeMonths.HasValue)
            {
                var ageBadge = new Border
                {
                    Background = new SolidColorBrush(
                        Color.FromRgb(254, 243, 199)),
                    CornerRadius = new CornerRadius(5),
                    Padding = new Thickness(10, 4, 10, 4)
                };
                ageBadge.Child = new TextBlock
                {
                    Text = $"від {course.MinAgeMonths} міс.",
                    FontSize = 11,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(
                        Color.FromRgb(245, 158, 11))
                };
                detailsPanel.Children.Add(ageBadge);
            }

            var panel = new StackPanel();
            panel.Children.Add(name);
            panel.Children.Add(desc);
            panel.Children.Add(detailsPanel);
            card.Child = panel;

            if (RoleAccess.IsAdmin)
            {
                card.MouseLeftButtonUp += (s, e) =>
                {
                    var dialog = new CynologicalCenter.UI.Dialogs
                        .CourseEditDialog(course.CourseId);
                    if (dialog.ShowDialog() == true)
                        _ = LoadDataAsync();
                };
            }

            return card;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CynologicalCenter.UI.Dialogs
                .CourseEditDialog();
            if (dialog.ShowDialog() == true)
                _ = LoadDataAsync();
        }
    }
}
