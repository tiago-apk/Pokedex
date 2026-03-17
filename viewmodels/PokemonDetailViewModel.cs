using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Pokedex.data;
using Pokedex.models;

namespace Pokedex.viewmodels
{
    public class EvolutionNode
    {
        public string Species { get; set; }
        public string ImagePath { get; set; }
        public string TargetId { get; set; }
        public bool IsCurrent { get; set; }
        public bool ShowIncomingArrow { get; set; }
        public string EvoConditionText { get; set; }
        public string EvoToolTip { get; set; }
    }

    public class UIDexEntry
    {
        public string Game { get; set; }
        public string Description { get; set; }
    }

    public class UIForm
    {
        public string FormId { get; set; }
        public bool IsCosmetic { get; set; }
        public CosmeticForm CosmeticData { get; set; }
    }

    public class PokemonDetailViewModel : BaseViewModel
    {
        private Pokemon _pokemon;
        public Pokemon Pokemon { get => _pokemon; set { _pokemon = value; OnPropertyChanged(); } }

        private string _displayedImage;
        public string DisplayedImage { get => _displayedImage; set { _displayedImage = value; OnPropertyChanged(); } }

        private bool _isShiny;
        public bool IsShiny { get => _isShiny; set { _isShiny = value; UpdateDisplayedImage(); } }

        private bool _isFemale;
        public bool IsFemale { get => _isFemale; set { _isFemale = value; UpdateDisplayedImage(); } }

        public bool HasFemaleVariant => !string.IsNullOrEmpty(_currentImageFemale);
        public bool HasMultipleForms => _availableForms.Count > 1;
        public string CurrentFormName { get; set; }

        public IEnumerable<string> DisplayAbilities => Pokemon?.Abilities?
            .OrderBy(a => a.Slot)
            .Select(a => (a.IsHidden ?? false) ? $"{a.AbilityName} (Hidden)" : a.AbilityName);

        private List<UIDexEntry> _currentEntries = new List<UIDexEntry>();
        private int _descriptionIndex = 0;
        public UIDexEntry CurrentEntry => _currentEntries.ElementAtOrDefault(_descriptionIndex);

        private List<UIForm> _availableForms = new List<UIForm>();
        private int _currentFormIndex = 0;
        private string _currentImageNormal;
        private string _currentImageShiny;
        private string _currentImageFemale;
        private string _currentImageFemaleShiny;

        private List<List<Pokemon>> _allPossiblePaths = new List<List<Pokemon>>();
        private int _currentPathIndex = 0;
        private ObservableCollection<EvolutionNode> _currentEvolutionLine;
        public ObservableCollection<EvolutionNode> CurrentEvolutionLine { get => _currentEvolutionLine; set { _currentEvolutionLine = value; OnPropertyChanged(); } }
        public bool HasMultipleEvolutionPaths => _allPossiblePaths.Count > 1;

        public ICommand NextPokemonCommand { get; }
        public ICommand PrevPokemonCommand { get; }
        public ICommand NextFormCommand { get; }
        public ICommand PrevFormCommand { get; }
        public ICommand NextDescriptionCommand { get; }
        public ICommand PrevDescriptionCommand { get; }
        public ICommand NextEvoPathCommand { get; }
        public ICommand PrevEvoPathCommand { get; }
        public ICommand NavigateToPokemonCommand { get; }

        public PokemonDetailViewModel(string pokemonId)
        {
            NextPokemonCommand = new RelayCommand(GoToNextPokemon);
            PrevPokemonCommand = new RelayCommand(GoToPrevPokemon);
            NextFormCommand = new RelayCommand(NextForm);
            PrevFormCommand = new RelayCommand(PrevForm);
            NextDescriptionCommand = new RelayCommand(NextDescription);
            PrevDescriptionCommand = new RelayCommand(PrevDescription);
            NextEvoPathCommand = new RelayCommand(NextPath);
            PrevEvoPathCommand = new RelayCommand(PrevPath);
            NavigateToPokemonCommand = new RelayCommand<string>(id => LoadPokemonFull(id));

            LoadPokemonFull(pokemonId);
        }

        private void LoadPokemonFull(string id)
        {
            if (string.IsNullOrEmpty(id)) return;

            using (var db = new AppDbContext())
            {
                var p = db.Pokemons
                    .Include(x => x.Types)
                    .Include(x => x.Abilities)
                    .Include(x => x.Descriptions)
                    .Include(x => x.EggGroups)
                    .Include(x => x.LocalDexes)
                    .Include(x => x.CosmeticForms).ThenInclude(cf => cf.Descriptions)
                    .Include(x => x.EvolvesFrom)
                    .Include(x => x.EvolvesTo)
                    .FirstOrDefault(x => x.Id == id);

                if (p != null)
                {
                    Pokemon = p;
                    BuildFormsList(db);
                    BuildEvolutionChain();
                    OnPropertyChanged(nameof(DisplayAbilities));
                }
            }
        }

        private void BuildFormsList(AppDbContext db)
        {
            _availableForms.Clear();

            var structuralForms = db.Pokemons
                .Include(x => x.CosmeticForms).ThenInclude(cf => cf.Descriptions)
                .Where(x => x.NationalDex == Pokemon.NationalDex)
                .OrderBy(x => x.Id).ToList();

            foreach (var sf in structuralForms)
            {
                _availableForms.Add(new UIForm { FormId = sf.Id, IsCosmetic = false });
                foreach (var cf in sf.CosmeticForms)
                {
                    _availableForms.Add(new UIForm { FormId = sf.Id, IsCosmetic = true, CosmeticData = cf });
                }
            }

            _currentFormIndex = _availableForms.FindIndex(f => f.FormId == Pokemon.Id && !f.IsCosmetic);
            if (_currentFormIndex == -1) _currentFormIndex = 0;

            OnPropertyChanged(nameof(HasMultipleForms));
            ApplyCurrentForm();
        }

        private void ApplyCurrentForm()
        {
            if (_availableForms.Count == 0) return;
            var form = _availableForms[_currentFormIndex];

            if (form.IsCosmetic)
            {
                _currentImageNormal = form.CosmeticData.ImageNormal;
                _currentImageShiny = form.CosmeticData.ImageShiny;
                _currentImageFemale = form.CosmeticData.ImageFemale;
                _currentImageFemaleShiny = form.CosmeticData.ImageFemaleShiny;
                CurrentFormName = form.CosmeticData.FormName;

                _currentEntries = form.CosmeticData.Descriptions
                    .Select(d => new UIDexEntry { Game = d.Game, Description = d.Description })
                    .ToList();
            }
            else
            {
                _currentImageNormal = Pokemon.ImageNormal;
                _currentImageShiny = Pokemon.ImageShiny;
                _currentImageFemale = Pokemon.ImageFemale;
                _currentImageFemaleShiny = Pokemon.ImageFemaleShiny;
                CurrentFormName = string.Empty;

                _currentEntries = Pokemon.Descriptions
                    .Select(d => new UIDexEntry { Game = d.Game, Description = d.Description })
                    .ToList();
            }

            if (!_currentEntries.Any())
                _currentEntries.Add(new UIDexEntry { Game = "Unknown", Description = "No description available." });

            _descriptionIndex = 0;
            IsFemale = false;

            OnPropertyChanged(nameof(CurrentFormName));
            OnPropertyChanged(nameof(HasFemaleVariant));
            OnPropertyChanged(nameof(CurrentEntry));
            UpdateDisplayedImage();
        }

        private void UpdateDisplayedImage()
        {
            if (IsShiny && IsFemale && !string.IsNullOrEmpty(_currentImageFemaleShiny))
                DisplayedImage = _currentImageFemaleShiny;
            else if (IsShiny && !string.IsNullOrEmpty(_currentImageShiny))
                DisplayedImage = _currentImageShiny;
            else if (IsFemale && !string.IsNullOrEmpty(_currentImageFemale))
                DisplayedImage = _currentImageFemale;
            else
                DisplayedImage = _currentImageNormal;
        }

        private void NextForm()
        {
            if (!HasMultipleForms) return;
            _currentFormIndex = (_currentFormIndex + 1) % _availableForms.Count;
            if (_availableForms[_currentFormIndex].FormId != Pokemon.Id) LoadPokemonFull(_availableForms[_currentFormIndex].FormId);
            else ApplyCurrentForm();
        }

        private void PrevForm()
        {
            if (!HasMultipleForms) return;
            _currentFormIndex = (_currentFormIndex - 1 + _availableForms.Count) % _availableForms.Count;
            if (_availableForms[_currentFormIndex].FormId != Pokemon.Id) LoadPokemonFull(_availableForms[_currentFormIndex].FormId);
            else ApplyCurrentForm();
        }

        private void NextDescription() { if (_currentEntries.Count > 1) { _descriptionIndex = (_descriptionIndex + 1) % _currentEntries.Count; OnPropertyChanged(nameof(CurrentEntry)); } }
        private void PrevDescription() { if (_currentEntries.Count > 1) { _descriptionIndex = (_descriptionIndex - 1 + _currentEntries.Count) % _currentEntries.Count; OnPropertyChanged(nameof(CurrentEntry)); } }

        private void GoToNextPokemon()
        {
            using (var db = new AppDbContext())
            {
                var next = db.Pokemons.Where(p => p.NationalDex > Pokemon.NationalDex && p.Form == "Base").OrderBy(p => p.NationalDex).FirstOrDefault();
                if (next != null) LoadPokemonFull(next.Id);
            }
        }

        private void GoToPrevPokemon()
        {
            using (var db = new AppDbContext())
            {
                var prev = db.Pokemons.Where(p => p.NationalDex < Pokemon.NationalDex && p.Form == "Base").OrderByDescending(p => p.NationalDex).FirstOrDefault();
                if (prev != null) LoadPokemonFull(prev.Id);
            }
        }

        private void BuildEvolutionChain()
        {
            using (var db = new AppDbContext())
            {
                var root = Pokemon;
                while (root.EvolvesFrom?.Any() == true)
                {
                    var parentId = root.EvolvesFrom.First().FromPokemonId;
                    root = db.Pokemons.Include(p => p.EvolvesFrom).Include(p => p.EvolvesTo).FirstOrDefault(p => p.Id == parentId);
                }

                _allPossiblePaths.Clear();
                FindPathsRecursive(root, new List<Pokemon>(), db);

                _currentPathIndex = _allPossiblePaths.FindIndex(path => path.Any(p => p.Id == Pokemon.Id));
                if (_currentPathIndex == -1) _currentPathIndex = 0;
                UpdateEvolutionDisplay();
            }
        }

        private void FindPathsRecursive(Pokemon current, List<Pokemon> currentPath, AppDbContext db)
        {
            var newPath = new List<Pokemon>(currentPath) { current };
            var evos = db.Evolutions.Where(e => e.FromPokemonId == current.Id).ToList();

            if (!evos.Any()) _allPossiblePaths.Add(newPath);
            else
            {
                foreach (var e in evos)
                {
                    var next = db.Pokemons.Include(p => p.EvolvesTo).FirstOrDefault(p => p.Id == e.ToPokemonId);
                    if (next != null) FindPathsRecursive(next, newPath, db);
                }
            }
        }

        private void UpdateEvolutionDisplay()
        {
            var nodes = new List<EvolutionNode>();
            if (_allPossiblePaths.Count > 0)
            {
                var path = _allPossiblePaths[_currentPathIndex];
                using (var db = new AppDbContext())
                {
                    for (int i = 0; i < path.Count; i++)
                    {
                        var p = path[i];
                        var node = new EvolutionNode
                        {
                            Species = p.Name,
                            ImagePath = p.ImageNormal,
                            TargetId = p.Id,
                            IsCurrent = p.Id == Pokemon.Id,
                            ShowIncomingArrow = i > 0
                        };
                        if (i > 0)
                        {
                            var evo = db.Evolutions.FirstOrDefault(e => e.FromPokemonId == path[i - 1].Id && e.ToPokemonId == p.Id);
                            if (evo != null)
                            {
                                node.EvoConditionText = evo.MinLevel > 0 ? $"Lv. {evo.MinLevel}" : evo.Method;
                                node.EvoToolTip = evo.Item ?? evo.Method;
                            }
                        }
                        nodes.Add(node);
                    }
                }
            }
            CurrentEvolutionLine = new ObservableCollection<EvolutionNode>(nodes);
            OnPropertyChanged(nameof(HasMultipleEvolutionPaths));
        }

        private void NextPath() { if (HasMultipleEvolutionPaths) { _currentPathIndex = (_currentPathIndex + 1) % _allPossiblePaths.Count; UpdateEvolutionDisplay(); } }
        private void PrevPath() { if (HasMultipleEvolutionPaths) { _currentPathIndex = (_currentPathIndex - 1 + _allPossiblePaths.Count) % _allPossiblePaths.Count; UpdateEvolutionDisplay(); } }
    }
}