using System.Windows;
using System.Windows.Controls;
using Pokedex.viewmodels;

namespace Pokedex.views
{
    public partial class PokemonRegisterView : UserControl
    {
        private PokemonRegisterViewModel _viewModel;

        public PokemonRegisterView()
        {
            InitializeComponent();
            _viewModel = new PokemonRegisterViewModel();
            this.DataContext = _viewModel;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SavePokemon())
            {
                MessageBox.Show("Pokémon registered!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                _viewModel = new PokemonRegisterViewModel();
                this.DataContext = _viewModel;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _viewModel = new PokemonRegisterViewModel();
            this.DataContext = _viewModel;
        }
    }
}