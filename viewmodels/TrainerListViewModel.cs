using System.Collections.ObjectModel;
using System.Linq;
using Pokedex.data;
using Pokedex.models;

namespace Pokedex.viewmodels
{
    public class TrainerListViewModel : BaseViewModel
    {
        public ObservableCollection<Trainer> Trainers { get; set; } = new ObservableCollection<Trainer>();

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
                LoadPokemonsForTrainer(value?.TrainerId);
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

        private void LoadPokemonsForTrainer(string trainerId)
        {
            if (string.IsNullOrEmpty(trainerId)) return;

            using (var db = new AppDbContext())
            {
                var registered = db.RegisteredPokemons
                                   .Where(p => p.TrainerId == trainerId)
                                   .ToList();

                var newList = new ObservableCollection<DisplayPokemon>();

                foreach (var p in registered)
                {
                    var baseDex = db.Pokemons.FirstOrDefault(dex => dex.Id == p.PokemonId);

                    // 1. Assumimos a imagem normal por defeito
                    string img = baseDex?.ImageNormal;

                    // 2. Trocamos para shiny SE estiver marcado como shiny no BD e existir o caminho
                    if (p.IsShiny && baseDex != null && !string.IsNullOrWhiteSpace(baseDex.ImageShiny))
                    {
                        img = baseDex.ImageShiny;
                    }

                    // 3. O DETETIVE: Lê a tua janela "Output" (Saída) no Visual Studio após correr isto!
                    System.Diagnostics.Debug.WriteLine($"[SHINY-DEBUG] Pkm: {p.PokemonId} | Shiny no Banco? {p.IsShiny} | Imagem Escolhida: {img}");

                    newList.Add(new DisplayPokemon
                    {
                        Id = p.Id,
                        Dex = (baseDex != null && baseDex.NationalDex.HasValue) ? baseDex.NationalDex.Value.ToString("D4") : "???",
                        Species = baseDex?.Name ?? p.BasePokemon,
                        DisplayName = !string.IsNullOrWhiteSpace(p.Nickname) ? p.Nickname : (baseDex?.Name ?? p.BasePokemon),
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