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
    public partial class AuditReportDialog : Window
    {
        public AuditReportDialog(DataSet ds)
        {
            InitializeComponent();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var row = ds.Tables[0].Rows[0];
                var panel = new WrapPanel { Margin = new Thickness(0) };

                foreach (DataColumn col in ds.Tables[0].Columns)
                {
                    var sp = new StackPanel
                    {
                        Margin = new Thickness(0, 0, 24, 0)
                    };
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
                        FontSize = 20,
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(
                            Color.FromRgb(99, 102, 241))
                    });
                    panel.Children.Add(sp);
                }

                GridStats.ItemsSource = null;
                var container = GridStats.Parent as System.Windows.Controls.StackPanel;
                if (container != null)
                {
                }

                GridStats.ItemsSource = ds.Tables[0].DefaultView;
            }

            if (ds.Tables.Count > 1)
                GridByTable.ItemsSource = ds.Tables[1].DefaultView;

            if (ds.Tables.Count > 2)
                GridDetail.ItemsSource = ds.Tables[2].DefaultView;
        }
    }
}
