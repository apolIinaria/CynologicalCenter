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
    public partial class DashboardPage : Page
    {
        public DashboardPage() => InitializeComponent();

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var clients = await App.OwnerService.GetActiveClientsViewAsync();
                TxtActiveClients.Text = clients.Count.ToString();

                var expired = await App.Dogs.GetExpiredVaccinationViewAsync();
                TxtExpiredVacc.Text = expired.Count.ToString();
                GridExpired.ItemsSource = expired;

                var schedule = await App.SessionService.GetPublicScheduleAsync();
                var today = schedule
                    .Where(s => s.Status == "Заплановано")
                    .ToList();
                TxtTodaySessions.Text = today.Count.ToString();
                GridSchedule.ItemsSource = today;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Помилка завантаження: {ex.Message}");
            }
        }
    }
}
