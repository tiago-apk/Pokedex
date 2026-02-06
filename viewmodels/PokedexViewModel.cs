using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Pokedex.data;
using Pokedex.models; // Importante para usar CustomIcons

namespace Pokedex.viewmodels
{
    public class RegionFilter
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
    }

    public class PokedexViewModel : BaseViewModel
    {
        // ... (Propriedades Pokemons, _allPokemons e Comandos mantêm-se iguais) ...

        // Cópia das propriedades para contexto
        private ObservableCollection<PokedexEntry> _pokemons;
        private List<PokedexEntry> _allPokemons;
        private string _currentRegionName;

        public string CurrentRegionName
        {
            get => _currentRegionName;
            set { _currentRegionName = value; OnPropertyChanged(); }
        }

        public ObservableCollection<PokedexEntry> Pokemons
        {
            get => _pokemons;
            set { _pokemons = value; OnPropertyChanged(); }
        }

        public List<RegionFilter> Regions { get; private set; }
        public ICommand OpenDetailCommand { get; }
        public ICommand FilterByRegionCommand { get; }

        public PokedexViewModel()
        {
            OpenDetailCommand = new RelayCommand<PokedexEntry>(OpenDetail);
            FilterByRegionCommand = new RelayCommand<RegionFilter>(ApplyFilter);

            InitializeRegions();
            LoadPokemons();
        }

        private void InitializeRegions()
        {
            // AGORA USANDO A CLASSE CustomIcons
            Regions = new List<RegionFilter>
            {
                new RegionFilter { Name = "All", Icon = CustomIcons.Pokeball, Start = 0, End = 9999 },

                new RegionFilter { Name = "Kanto", Icon = CustomIcons.Kanto, Start = 1, End = 151 },
                new RegionFilter { Name = "Johto", Icon = CustomIcons.Johto, Start = 152, End = 251 },
                new RegionFilter { Name = "Hoenn", Icon = CustomIcons.Hoenn, Start = 252, End = 386 },
                new RegionFilter { Name = "Sinnoh", Icon = CustomIcons.Sinnoh, Start = 387, End = 493 },
                new RegionFilter { Name = "Unova", Icon = CustomIcons.Unova, Start = 494, End = 649 },
                new RegionFilter { Name = "Kalos", Icon = CustomIcons.Kalos, Start = 650, End = 721 },
                new RegionFilter { Name = "Alola", Icon = CustomIcons.Alola, Start = 722, End = 807 },
                new RegionFilter { Name = "Unknown", Icon = CustomIcons.Unknown, Start = 808, End = 809 },
                new RegionFilter { Name = "Galar", Icon = CustomIcons.Galar, Start = 810, End = 898 },
                new RegionFilter { Name = "Hisui", Icon = CustomIcons.Hisui, Start = 899, End = 905 },
                new RegionFilter { Name = "Paldea", Icon = CustomIcons.Paldea, Start = 906, End = 1025 }
            };
        }

        // ... (Métodos LoadPokemons, ApplyFilter e OpenDetail mantêm-se iguais) ...
        private void LoadPokemons()
        {
            using (var db = new AppDbContext())
            {
                _allPokemons = db.PokedexEntries
                    .AsEnumerable()
                    .Where(p => string.IsNullOrEmpty(p.Forms) || p.Forms == "Altered")
                    .OrderBy(p => p.Id)
                    .ToList();
                ApplyFilter(Regions[0]);
            }
        }

        private void ApplyFilter(RegionFilter region)
        {
            if (region == null) return;
            CurrentRegionName = region.Name == "All" ? "Pokedex" : region.Name;
            var filtered = _allPokemons.Where(p =>
            {
                if (int.TryParse(p.Dex.Replace("#", ""), out int dexNum))
                    return dexNum >= region.Start && dexNum <= region.End;
                return false;
            }).ToList();
            Pokemons = new ObservableCollection<PokedexEntry>(filtered);
        }

        private void OpenDetail(PokedexEntry pokemon)
        {
            if (pokemon == null) return;
            var detailView = new views.PokemonDetailView(pokemon);
            detailView.ShowDialog();
        }
    }
}