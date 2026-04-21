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
    public partial class AdminPage : Page
    {
        public AdminPage() => InitializeComponent();

        private async void Page_Loaded(object sender, RoutedEventArgs e)
            => await LoadDataAsync();

        private async System.Threading.Tasks.Task LoadDataAsync()
        {
            try
            {
                var logs = await App.Audit.GetByDateRangeAsync(
                    DateTime.Today.AddDays(-30), DateTime.Today);
                GridAudit.ItemsSource = logs;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження: {ex.Message}");
            }
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
            => await LoadDataAsync();
    }
}
