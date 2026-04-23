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
    public partial class DogReportDialog : Window
    {
        public DogReportDialog(DataSet ds)
        {
            InitializeComponent();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var row = ds.Tables[0].Rows[0];
                var panel = new StackPanel();
                foreach (DataColumn col in ds.Tables[0].Columns)
                {
                    var rowPanel = new StackPanel
                    {
                        Margin = new Thickness(0, 0, 0, 8)
                    };
                    rowPanel.Children.Add(new TextBlock
                    {
                        Text = col.ColumnName,
                        FontSize = 11,
                        FontWeight = FontWeights.SemiBold,
                        Foreground = new SolidColorBrush(
                            Color.FromRgb(107, 114, 128))
                    });
                    rowPanel.Children.Add(new TextBlock
                    {
                        Text = row[col]?.ToString() ?? "—",
                        FontSize = 14,
                        Foreground = new SolidColorBrush(
                            Color.FromRgb(26, 29, 46))
                    });
                    panel.Children.Add(rowPanel);
                }
                GridInfoContent.Children.Add(panel);
            }

            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                var row = ds.Tables[1].Rows[0];
                var panel = new WrapPanel();
                foreach (DataColumn col in ds.Tables[1].Columns)
                {
                    var block = new Border
                    {
                        Margin = new Thickness(0, 0, 16, 12),
                        Padding = new Thickness(0)
                    };
                    var sp = new StackPanel();
                    sp.Children.Add(new TextBlock
                    {
                        Text = col.ColumnName,
                        FontSize = 11,
                        FontWeight = FontWeights.SemiBold,
                        Foreground = new SolidColorBrush(
                            Color.FromRgb(107, 114, 128))
                    });
                    sp.Children.Add(new TextBlock
                    {
                        Text = row[col]?.ToString() ?? "—",
                        FontSize = 16,
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(
                            Color.FromRgb(99, 102, 241))
                    });
                    block.Child = sp;
                    panel.Children.Add(block);
                }
                GridStatsContent.Children.Add(panel);
            }

            if (ds.Tables.Count > 2)
                GridHistory.ItemsSource = ds.Tables[2].DefaultView;
        }
    }
}
