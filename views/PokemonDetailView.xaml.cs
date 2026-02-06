using System.Windows;
using Pokedex.models;
using Pokedex.viewmodels;

namespace Pokedex.views
{
    public partial class PokemonDetailView : Window
    {
        public PokemonDetailViewModel ViewModel => DataContext as PokemonDetailViewModel;

        public PokemonDetailView(PokedexEntry entry)
        {
            InitializeComponent();
            this.DataContext = new PokemonDetailViewModel(entry);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Nomes traduzidos para seguir o Clean Code
        private void NextForm_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.NextForm();
        }

        private void PreviousForm_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.PreviousForm();
        }
    }
}