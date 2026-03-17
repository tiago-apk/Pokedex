using System.Windows;
using Pokedex.viewmodels;

namespace Pokedex.views
{
    public partial class PokemonDetailView : Window
    {
        public PokemonDetailView(string pokemonId)
        {
            InitializeComponent();
            this.DataContext = new PokemonDetailViewModel(pokemonId);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}