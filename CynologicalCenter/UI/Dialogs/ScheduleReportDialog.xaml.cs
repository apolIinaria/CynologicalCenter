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
    public partial class ScheduleReportDialog : Window
    {
        public ScheduleReportDialog(DataSet ds)
        {
            InitializeComponent();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var row = ds.Tables[0].Rows[0];
                TxtHeader.Text = $"{row[0]}  |  {row[1]}";
            }

            if (ds.Tables.Count > 1)
                GridSchedule.ItemsSource = ds.Tables[1].DefaultView;
        }
    }
}
