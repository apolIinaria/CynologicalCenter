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

            if (ds.Tables.Count > 0)
                GridStats.ItemsSource = ds.Tables[0].DefaultView;
            if (ds.Tables.Count > 1)
                GridByTable.ItemsSource = ds.Tables[1].DefaultView;
            if (ds.Tables.Count > 2)
                GridDetail.ItemsSource = ds.Tables[2].DefaultView;
        }
    }
}
