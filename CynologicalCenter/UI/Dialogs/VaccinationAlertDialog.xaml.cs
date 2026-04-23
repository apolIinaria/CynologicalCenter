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
using CynologicalCenter.Models.ViewModels;

namespace CynologicalCenter.UI.Dialogs
{
    public partial class VaccinationAlertDialog : Window
    {
        public VaccinationAlertDialog(List<ExpiredVaccinationViewModel> dogs)
        {
            InitializeComponent();
            TxtCount.Text = $"{dogs.Count} собак";
            GridExpired.ItemsSource = dogs;
        }
    }
}
