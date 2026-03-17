using Pokedex.viewmodels;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pokedex.views
{
    public partial class TrainerRegisterView : UserControl
    {
        // Referência privada para a nossa ViewModel
        private TrainerRegisterViewModel _viewModel;

        public TrainerRegisterView()
        {
            InitializeComponent();
            _viewModel = new TrainerRegisterViewModel();
            this.DataContext = _viewModel;
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Save();
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Clear();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}