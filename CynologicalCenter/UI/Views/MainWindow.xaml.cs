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
using CynologicalCenter.Data;
using MySql.Data.MySqlClient;

namespace CynologicalCenter.UI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private  async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var factory = new DbConnectionFactory();
                using var conn = factory.CreateConnection();
                await conn.OpenAsync();

                MessageBox.Show("Підключення до БД успішне!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка підключення:\n{ex.Message}");
            }
        }
    }
}
