using System.Windows;

namespace CynologicalCenter.UI
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            // Open main window if available
            try
            {
                var main = new Views.MainWindow();
                main.Show();
            }
            catch
            {
                // If MainWindow is not available, simply close login
            }
            this.Close();
        }
    }
}