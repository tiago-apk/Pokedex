using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input; // Necessário para ICommand
using Pokedex.data;
using Pokedex.models;
using Pokedex.services;
using Pokedex.views; // Para abrir a DetailView

namespace Pokedex.viewmodels
{
    // Idealmente, herde de uma classe base que implemente INotifyPropertyChanged
    public class PokedexViewModel
    {
        public ObservableCollection<PokedexEntry> Pokemons { get; set; }

        // Este é o comando que o botão no XAML vai chamar
        public ICommand OpenDetailCommand { get; }

        public PokedexViewModel()
        {
            using (var db = new AppDbContext())
            {
                var list = db.PokedexEntries.OrderBy(p => p.Dex).ToList();
                Pokemons = new ObservableCollection<PokedexEntry>(list);
            }

            // Inicializamos o comando passando o método que será executado
            OpenDetailCommand = new RelayCommand<PokedexEntry>(ExecuteOpenDetail);
        }

        private void ExecuteOpenDetail(PokedexEntry entry)
        {
            if (entry == null) return;

            // Instancia a nova tela passando o Pokémon selecionado no construtor
            var detailWindow = new PokemonDetailView(entry);
            detailWindow.ShowDialog(); // Abre como janela modal
        }

    }
}