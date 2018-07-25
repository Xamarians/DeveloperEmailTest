using DeveloperTest.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace DeveloperTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainPageViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewModel = new MainPageViewModel();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            viewModel?.Cancel();
        }

        private void OnPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            viewModel.Password = (sender as System.Windows.Controls.PasswordBox).Password;
        }
    }
}
