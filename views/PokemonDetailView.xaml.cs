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
using Pokedex.models;

namespace Pokedex.views
{
    /// <summary>
    /// Interaction logic for PokemonDetailView.xaml
    /// </summary>
    public partial class PokemonDetailView : Window
    {
        public PokemonDetailView()
        {
            InitializeComponent();
        }

        private void chkFemale_Checked(object sender, RoutedEventArgs e)
        {

        }
        public PokemonDetailView(PokedexEntry entry) : this()
        {
            this.DataContext = entry;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ProximaForma_Click(object sender, RoutedEventArgs e)
        {
            // Puxa o ViewModel do DataContext e chama o método
            if (DataContext is PokemonDetailViewModel vm) vm.NextForm();
        }

        private void FormaAnterior_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PokemonDetailViewModel vm) vm.PreviousForm();
        }
    }
}
