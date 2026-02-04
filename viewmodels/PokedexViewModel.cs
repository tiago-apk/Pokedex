using System.Collections.ObjectModel;
using System.Linq;
using Pokedex.data;
using Pokedex.models;

namespace Pokedex.viewmodels
{
    public class PokedexViewModel
    {
        // Esta coleção avisa a tela quando os dados mudam (como um Stream no Flutter)
        public ObservableCollection<PokedexEntry> Pokemons { get; set; }

        public PokedexViewModel()
        {
            using (var db = new AppDbContext())
            {
                // Busca todos os Pokémons do SQLite e converte para a lista da tela
                var list = db.PokedexEntries.OrderBy(p => p.Dex).ToList();
                Pokemons = new ObservableCollection<PokedexEntry>(list);
            }
        }
    }
}