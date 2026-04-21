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
            CmbCourse.ItemsSource = await App.Courses.GetAllAsync();
        }

        private async void BtnEnroll_Click(object sender, RoutedEventArgs e)
        {
            TxtError.Text = "";

            if (CmbDog.SelectedValue == null ||
                CmbTrainer.SelectedValue == null ||
                CmbCourse.SelectedValue == null ||
                DpDate.SelectedDate == null)
            {
                TxtError.Text = "Заповніть всі поля";
                return;
            }

            if (!TimeSpan.TryParse(TxtTime.Text, out TimeSpan time))
            {
                TxtError.Text = "Невірний формат часу (HH:mm)";
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
                TxtError.Text = msg;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
