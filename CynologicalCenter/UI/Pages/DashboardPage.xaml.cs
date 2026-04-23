using CynologicalCenter.Helpers;
using CynologicalCenter.Models.ViewModels;
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
using System.Windows.Threading;

namespace CynologicalCenter.UI.Pages
{
    public partial class DashboardPage : Page
    {
        private DispatcherTimer? _clockTimer;
        private List<ExpiredVaccinationViewModel> _expiredDogs = new();

        public DashboardPage() => InitializeComponent();

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            StartClock();

            try
            {
                string hour = DateTime.Now.Hour switch
                {
                    < 12 => "Доброго ранку",
                    < 17 => "Добрий день",
                    _ => "Добрий вечір"
                };
                TxtGreeting.Text = $"{hour}, {CurrentUser.Username}!";
                TxtDate.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy",
                    new System.Globalization.CultureInfo("uk-UA"));

                var clients = await App.OwnerService.GetActiveClientsViewAsync();
                TxtActiveClients.Text = clients.Count.ToString();

                var trainers = await App.TrainerService.GetAllAsync();
                TxtTrainersCount.Text = trainers.Count.ToString();

                _expiredDogs = await App.Dogs.GetExpiredVaccinationViewAsync();
                TxtExpiredVacc.Text = _expiredDogs.Count.ToString();

                if (_expiredDogs.Count == 0)
                {
                    TxtVaccLabel.Text = "Всі собаки здорові";
                    TxtVaccHint.Visibility = Visibility.Collapsed;
                }
                else
                {
                    TxtVaccLabel.Text = "Прострочена вакцинація";
                    TxtVaccHint.Visibility = Visibility.Visible;
                }

                var schedule = await App.SessionService.GetPublicScheduleAsync();
                string todayStr = DateTime.Today.ToString("dd.MM.yyyy");
                var today = schedule
                    .Where(s => s.Status == "Заплановано" &&
                                s.SessionTime.StartsWith(todayStr))
                    .ToList();

                TxtTodaySessions.Text = today.Count.ToString();
                TxtTodayBadge.Text = $"{today.Count} занять";

                if (today.Count == 0)
                {
                    EmptySchedule.Visibility = Visibility.Visible;
                    GridSchedule.Visibility = Visibility.Collapsed;
                    TodayBadge.Visibility = Visibility.Collapsed;
                }
                else
                {
                    GridSchedule.ItemsSource = today;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження: {ex.Message}");
            }
        }

        private void VaccCard_Click(object sender,
            System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_expiredDogs.Count == 0)
            {
                MessageBox.Show("Всі собаки мають дійсну вакцинацію!",
                    "Статус вакцинації",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            var dialog = new CynologicalCenter.UI.Dialogs
                .VaccinationAlertDialog(_expiredDogs);
            dialog.ShowDialog();
        }

        private void StartClock()
        {
            UpdateClock();
            _clockTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _clockTimer.Tick += (s, e) => UpdateClock();
            _clockTimer.Start();
            Unloaded += (s, e) => _clockTimer?.Stop();
        }

        private void UpdateClock()
        {
            TxtClock.Text = DateTime.Now.ToString("HH:mm:ss",
                new System.Globalization.CultureInfo("uk-UA"));
        }
    }
}
