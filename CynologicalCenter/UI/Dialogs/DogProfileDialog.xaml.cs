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
using System.Data;

namespace CynologicalCenter.UI.Dialogs
{
    public partial class DogProfileDialog : Window
    {
        private readonly int _dogId;

        public DogProfileDialog(int dogId)
        {
            InitializeComponent();
            _dogId = dogId;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var ds = await App.ReportService.GetDogReportAsync(_dogId);

                if (ds.Tables.Count == 0) return;

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var row = ds.Tables[0].Rows[0];
                    TxtTitle.Text = $"🐕 {row["nickname"]}";

                    var panel = new WrapPanel { Margin = new Thickness(0) };
                    foreach (DataColumn col in ds.Tables[0].Columns)
                    {
                        panel.Children.Add(new TextBlock
                        {
                            Text = $"{col.ColumnName}: {row[col]}   ",
                            Margin = new Thickness(0, 0, 16, 4),
                            FontSize = 13
                        });
                    }
                    GridInfo.Children.Add(panel);
                }

                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    var row = ds.Tables[1].Rows[0];
                    var panel = new WrapPanel();
                    foreach (DataColumn col in ds.Tables[1].Columns)
                    {
                        panel.Children.Add(new TextBlock
                        {
                            Text = $"{col.ColumnName}: {row[col]}   ",
                            Margin = new Thickness(0, 0, 16, 4),
                            FontSize = 13,
                            Foreground = System.Windows.Media.Brushes.DarkBlue
                        });
                    }
                    GridStats.Children.Add(panel);
                }

                if (ds.Tables.Count > 2)
                    GridHistory.ItemsSource = ds.Tables[2].DefaultView;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }
    }
}
