using System.Windows.Controls;
using System.Windows.Input;
using Pokedex.models;
using Pokedex.viewmodels;

namespace Pokedex.views
{
    public partial class TrainerListView : UserControl
    {
        private TrainerListViewModel _viewModel;

        public TrainerListView()
        {
            InitializeComponent();
            _viewModel = new TrainerListViewModel();
            this.DataContext = _viewModel;
        }

        private void PokemonCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is DisplayPokemon clickedPoke)
            {
                // Pegamos o ID do treinador diretamente da ViewModel refatorada
                if (_viewModel.SelectedTrainer != null)
                {
                    var detailWindow = new RegisteredPokemonDetailView(clickedPoke.Id, _viewModel.SelectedTrainer.Id);
                    detailWindow.ShowDialog();
                }
            }
        }
    }
}