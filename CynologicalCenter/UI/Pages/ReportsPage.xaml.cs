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
    public partial class ReportsPage : Page
    {
        public ReportsPage()
        {
            InitializeComponent();
            DpAuditFrom.SelectedDate = DateTime.Today.AddDays(-7);
            DpAuditTo.SelectedDate = DateTime.Today;
            DpSchedule.SelectedDate = DateTime.Today;
            _ = LoadCombosAsync();
        }

        private async System.Threading.Tasks.Task LoadCombosAsync()
        {
            try
            {
                CmbTrainer.ItemsSource = await App.TrainerService.GetAllAsync();
                CmbDog.ItemsSource = await App.Dogs.GetAllAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }

        private async void BtnAuditReport_Click(object sender, RoutedEventArgs e)
        {
            if (DpAuditFrom.SelectedDate == null || DpAuditTo.SelectedDate == null)
            {
                MessageBox.Show("Оберіть діапазон дат");
                return;
            }

            try
            {
                var ds = await App.ReportService.GetAuditAnalysisAsync(
                    DpAuditFrom.SelectedDate.Value,
                    DpAuditTo.SelectedDate.Value);

                var dialog = new CynologicalCenter.UI.Dialogs.AuditReportDialog(ds);
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }

        private async void BtnSecurityReport_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(TxtHours.Text, out int hours) || hours <= 0)
            {
                MessageBox.Show("Введіть коректну кількість годин");
                return;
            }

            try
            {
                var (ds, total) = await App.ReportService.GetSecurityReportAsync(hours);
                var dialog = new CynologicalCenter.UI.Dialogs.SecurityReportDialog(ds, total);
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }

        private async void BtnScheduleReport_Click(object sender, RoutedEventArgs e)
        {
            if (CmbTrainer.SelectedValue == null || DpSchedule.SelectedDate == null)
            {
                MessageBox.Show("Оберіть тренера та дату");
                return;
            }

            try
            {
                int trainerId = (int)CmbTrainer.SelectedValue;
                var ds = await App.ReportService.GetTrainerScheduleAsync(
                    trainerId, DpSchedule.SelectedDate.Value);

                var dialog = new CynologicalCenter.UI.Dialogs.ScheduleReportDialog(ds);
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }

        private async void BtnDogReport_Click(object sender, RoutedEventArgs e)
        {
            if (CmbDog.SelectedValue == null)
            {
                MessageBox.Show("Оберіть собаку");
                return;
            }

            try
            {
                int dogId = (int)CmbDog.SelectedValue;
                var ds = await App.ReportService.GetDogReportAsync(dogId);
                var dialog = new CynologicalCenter.UI.Dialogs.DogReportDialog(ds);
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }
    }
}
