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
    public partial class SecurityReportDialog : Window
    {
        public SecurityReportDialog(DataSet ds, int total)
        {
            InitializeComponent();
            TxtTotal.Text = $"Знайдено інцидентів: {total}";
            if (ds.Tables.Count > 0) GridDelete.ItemsSource = ds.Tables[0].DefaultView;
            if (ds.Tables.Count > 1) GridErrors.ItemsSource = ds.Tables[1].DefaultView;
            if (ds.Tables.Count > 2) GridNight.ItemsSource = ds.Tables[2].DefaultView;
            if (ds.Tables.Count > 3) GridSummary.ItemsSource = ds.Tables[3].DefaultView;
        }
    }
}
