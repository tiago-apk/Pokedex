using System.Collections.ObjectModel;
using System.Linq;
using Pokedex.data;
using Pokedex.models;
using Pokedex.viewmodels; // Certifica-te que o namespace está correto

namespace Pokedex.viewmodels
{
    public class TrainerListViewModel : BaseViewModel
    {
        // Lista de treinadores (Coluna Esquerda)
        public ObservableCollection<Trainer> Trainers { get; set; } = new ObservableCollection<Trainer>();

        // Lista de Pokémons do time (Coluna Direita)
        private ObservableCollection<DisplayPokemon> _pokemons;
        public ObservableCollection<DisplayPokemon> Pokemons
        {
            get => _pokemons;
            set { _pokemons = value; OnPropertyChanged(); }
        }

        private Trainer _selectedTrainer;
        public Trainer SelectedTrainer
        {
            get => _selectedTrainer;
            set
            {
                _selectedTrainer = value;
                OnPropertyChanged();
                // Sempre que mudar o treinador, carregamos o time dele
                LoadPokemonsForTrainer(value?.Id ?? 0);
            }
        }

        public TrainerListViewModel()
        {
            LoadTrainers();
        }

        public void LoadTrainers()
        {
            using (var db = new AppDbContext())
            {
                var list = db.Trainers.OrderByDescending(t => t.Id).ToList();
                Trainers.Clear();
                foreach (var t in list) Trainers.Add(t);
            }
        }

        private void LoadPokemonsForTrainer(int trainerId)
        {
            if (trainerId == 0) { Pokemons = new ObservableCollection<DisplayPokemon>(); return; }

            using (var db = new AppDbContext())
            {
                var registered = db.RegisteredPokemons
                                   .Where(p => p.TrainerId == trainerId)
                                   .ToList();

                var newList = new ObservableCollection<DisplayPokemon>();

                foreach (var p in registered)
                {
                    var baseDex = db.PokedexEntries.FirstOrDefault(dex => dex.Name == p.Form);
                    string img = (p.IsShiny && !string.IsNullOrEmpty(baseDex?.Shiny)) ? baseDex.Shiny : baseDex?.Image;

                    newList.Add(new DisplayPokemon
                    {
                        Id = p.Id,
                        Dex = baseDex?.Dex ?? "???",
                        Species = p.BasePokemon,
                        DisplayName = !string.IsNullOrEmpty(p.Nickname) ? p.Nickname : p.Form,
                        Level = p.Level,
                        ImagePath = img,
                        IsShiny = p.IsShiny
                    });
                }
                Pokemons = newList;
            }
        }
    }
}