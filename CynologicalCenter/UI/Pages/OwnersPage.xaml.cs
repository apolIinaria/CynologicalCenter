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

namespace CynologicalCenter.UI.Pages
{
    public partial class OwnersPage : Page
    {
        private List<ActiveClientViewModel> _allOwners = new();

        public OwnersPage() => InitializeComponent();

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            BtnAdd.IsEnabled = RoleAccess.CanManageClients;
            BtnAdd.Visibility = RoleAccess.CanManageClients
                                 ? Visibility.Visible
                                 : Visibility.Collapsed;
            await LoadDataAsync();
        }

        private async System.Threading.Tasks.Task LoadDataAsync()
        {
            try
            {
                _allOwners = await App.OwnerService.GetActiveClientsViewAsync();
                TxtCount.Text = _allOwners.Count.ToString();
                RenderCards(_allOwners);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }

        private void RenderCards(List<ActiveClientViewModel> owners)
        {
            WrapOwners.Children.Clear();

            foreach (var owner in owners)
            {
                var card = CreateOwnerCard(owner);
                WrapOwners.Children.Add(card);
            }
        }

        private Border CreateOwnerCard(ActiveClientViewModel owner)
        {
            var card = new Border
            {
                Width = 220,
                Margin = new Thickness(0, 0, 12, 12),
                Background = Brushes.White,
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(16),
                Cursor = Cursors.Hand,
                Effect = (System.Windows.Media.Effects.Effect)
                    FindResource("CardShadow")
            };

            string initials = GetInitials(owner.FullName);
            var avatar = new Border
            {
                Width = 48,
                Height = 48,
                CornerRadius = new CornerRadius(5),
                Background = new SolidColorBrush(
                    Color.FromRgb(63, 125, 106)),
                Margin = new Thickness(0, 0, 0, 12),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            avatar.Child = new TextBlock
            {
                Text = initials,
                Foreground = Brushes.White,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var name = new TextBlock
            {
                Text = owner.FullName,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = (Brush)FindResource("TextPrimaryBrush"),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 4)
            };

            var dogsInfo = new TextBlock
            {
                Text = $"Собак: {owner.DogsCount}",
                FontSize = 12,
                Foreground = (Brush)FindResource("TextSecondaryBrush"),
                Margin = new Thickness(0, 0, 0, 4)
            };

            string lastVisit = owner.LastVisit.HasValue
                ? owner.LastVisit.Value.ToString("dd.MM.yyyy")
                : "Ще не було";
            var visitInfo = new TextBlock
            {
                Text = $"Останній візит: {lastVisit}",
                FontSize = 12,
                Foreground = (Brush)FindResource("TextLightBrush")
            };

            var panel = new StackPanel();
            panel.Children.Add(avatar);
            panel.Children.Add(name);
            panel.Children.Add(dogsInfo);
            panel.Children.Add(visitInfo);
            card.Child = panel;

            card.MouseLeftButtonUp += (s, e) =>
            {
                if (RoleAccess.IsGuest) return;
                var dialog = new CynologicalCenter.UI.Dialogs
                    .OwnerProfileDialog(owner.OwnerId);
                dialog.ShowDialog();
                _ = LoadDataAsync();
            };

            return card;
        }

        private string GetInitials(string fullName)
        {
            var parts = fullName.Split(' ');
            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[1][0]}".ToUpper();
            return fullName.Length > 0
                ? fullName[0].ToString().ToUpper() : "?";
        }

        private void TxtSearch_TextChanged(object sender,
            TextChangedEventArgs e)
        {
            string q = TxtSearch.Text.ToLower();
            var filtered = string.IsNullOrWhiteSpace(q)
                ? _allOwners
                : _allOwners.Where(o =>
                    o.FullName.ToLower().Contains(q)).ToList();
            RenderCards(filtered);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CynologicalCenter.UI.Dialogs.OwnerEditDialog();
            if (dialog.ShowDialog() == true)
                _ = LoadDataAsync();
        }
    }
}
