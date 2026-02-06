using System.Collections.Generic;
using System.Linq;
using Pokedex.models;
using Pokedex.data;

namespace Pokedex.viewmodels
{
    public class PokemonDetailViewModel : BaseViewModel
    {
        private PokedexEntry _pokemon;
        private bool _isShiny;
        private bool _isFemale;

        public PokedexEntry Pokemon
        {
            get => _pokemon;
            set
            {
                _pokemon = value;
                OnPropertyChanged();
                NotifyAllPropertiesChanged();
            }
        }

        public bool IsShiny
        {
            get => _isShiny;
            set { _isShiny = value; OnPropertyChanged(); OnPropertyChanged(nameof(DisplayedImage)); }
        }

        public bool IsFemale
        {
            get => _isFemale;
            set { _isFemale = value; OnPropertyChanged(); OnPropertyChanged(nameof(DisplayedImage)); }
        }

        public string DisplayedImage
        {
            get
            {
                if (Pokemon == null) return string.Empty;

                if (IsFemale && IsShiny && !string.IsNullOrEmpty(Pokemon.FemaleShiny))
                    return Pokemon.FemaleShiny;

                if (IsFemale && !string.IsNullOrEmpty(Pokemon.Female))
                    return Pokemon.Female;

                if (IsShiny && !string.IsNullOrEmpty(Pokemon.Shiny))
                    return Pokemon.Shiny;

                return Pokemon.Image;
            }
        }

        // No PokemonDetailViewModel.cs
        public List<string> DisplayTypes => Pokemon?.Types?
            .Where(t => !string.IsNullOrWhiteSpace(t)) // Remove "" ou null
            .ToList() ?? new List<string>();
        public bool HasFemaleVariant => !string.IsNullOrEmpty(Pokemon?.Female);
        public bool HasGMaxMove => !string.IsNullOrEmpty(Pokemon?.GMaxMove);

        public PokemonDetailViewModel(PokedexEntry pokemon)
        {
            Pokemon = pokemon;
        }

        // Métodos de navegação renomeados para Inglês
        public void NextForm()
        {
            using (var db = new AppDbContext())
            {
                var forms = db.PokedexEntries.Where(p => p.Dex == Pokemon.Dex).ToList();
                if (forms.Count <= 1) return;

                int index = forms.FindIndex(p => p.Id == Pokemon.Id);
                int nextIndex = (index + 1) % forms.Count;
                Pokemon = forms[nextIndex];
            }
        }

        public void PreviousForm()
        {
            using (var db = new AppDbContext())
            {
                var forms = db.PokedexEntries.Where(p => p.Dex == Pokemon.Dex).ToList();
                if (forms.Count <= 1) return;

                int index = forms.FindIndex(p => p.Id == Pokemon.Id);
                int prevIndex = (index - 1 + forms.Count) % forms.Count;
                Pokemon = forms[prevIndex];
            }
        }

        private void NotifyAllPropertiesChanged()
        {
            OnPropertyChanged(nameof(DisplayTypes));
            OnPropertyChanged(nameof(DisplayedImage));
            OnPropertyChanged(nameof(HasFemaleVariant));
            OnPropertyChanged(nameof(HasGMaxMove));
        }
    }
}