using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Pokedex.data;
using Pokedex.models;

namespace Pokedex.viewmodels
{
    public class PokemonDetailViewModel : BaseViewModel
    {
        private PokedexEntry _pokemon;
        public PokedexEntry Pokemon
        {
            get => _pokemon;
            set { _pokemon = value; OnPropertyChanged(); }
        }

        private string _displayedImage;
        public string DisplayedImage
        {
            get => _displayedImage;
            set { _displayedImage = value; OnPropertyChanged(); }
        }

        private List<string> _displayAbilities;
        public List<string> DisplayAbilities
        {
            get => _displayAbilities;
            set { _displayAbilities = value; OnPropertyChanged(); }
        }

        private bool _isShiny;
        public bool IsShiny
        {
            get => _isShiny;
            set
            {
                if (_isShiny == value) return;
                _isShiny = value;
                OnPropertyChanged();
                UpdateDisplayedImage();
                LoadEvolutionData();
            }
        }

        private bool _isFemale;
        public bool IsFemale
        {
            get => _isFemale;
            set
            {
                if (_isFemale == value) return;
                _isFemale = value;
                OnPropertyChanged();
                ApplyGenderForm();
            }
        }

        private bool _hasPreviousPokemon;
        public bool HasPreviousPokemon { get => _hasPreviousPokemon; set { _hasPreviousPokemon = value; OnPropertyChanged(); } }

        private bool _hasNextPokemon;
        public bool HasNextPokemon { get => _hasNextPokemon; set { _hasNextPokemon = value; OnPropertyChanged(); } }

        private bool _hasMultipleForms;
        public bool HasMultipleForms { get => _hasMultipleForms; set { _hasMultipleForms = value; OnPropertyChanged(); } }

        private bool _hasShinyVariant;
        public bool HasShinyVariant { get => _hasShinyVariant; set { _hasShinyVariant = value; OnPropertyChanged(); } }

        private bool _hasFemaleVariant;
        public bool HasFemaleVariant { get => _hasFemaleVariant; set { _hasFemaleVariant = value; OnPropertyChanged(); } }

        // PROPRIEDADES: MEGA E GIGANTAMAX
        private bool _hasMegaEvolution;
        public bool HasMegaEvolution { get => _hasMegaEvolution; set { _hasMegaEvolution = value; OnPropertyChanged(); } }

        private bool _hasGigantamax;
        public bool HasGigantamax { get => _hasGigantamax; set { _hasGigantamax = value; OnPropertyChanged(); } }

        public string MegaIconPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "type_images", "stats", "Megaevolution_icon.png");
        public string DynamaxIconPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "type_images", "stats", "Dynamax_icon.png");

        private List<PokedexEntry> _alternateForms;
        private int _currentFormIndex = 0;

        // ==========================================
        // PROPRIEDADES: LINHA EVOLUTIVA
        // ==========================================
        private List<List<EvolutionNode>> _allEvolutionPaths;
        private int _currentPathIndex = 0;

        private List<EvolutionNode> _currentEvolutionLine;
        public List<EvolutionNode> CurrentEvolutionLine
        {
            get => _currentEvolutionLine;
            set { _currentEvolutionLine = value; OnPropertyChanged(); }
        }

        private bool _hasMultipleEvolutionPaths;
        public bool HasMultipleEvolutionPaths { get => _hasMultipleEvolutionPaths; set { _hasMultipleEvolutionPaths = value; OnPropertyChanged(); } }

        private bool _hasEvolution;
        public bool HasEvolution { get => _hasEvolution; set { _hasEvolution = value; OnPropertyChanged(); } }
        // ==========================================

        public PokemonDetailViewModel(PokedexEntry pokemon)
        {
            Pokemon = pokemon;
            _isShiny = false;
            _isFemale = false;
            InitializeData();
        }

        private void InitializeData(string targetDex = null)
        {
            LoadAlternateForms();
            LoadAdjacentPokemonPresence();
            LoadEvolutionData();

            if (targetDex != null)
            {
                int foundIndex = _alternateForms.FindIndex(f => f.Dex == targetDex);
                _currentFormIndex = foundIndex >= 0 ? foundIndex : 0;
            }
            else
            {
                _currentFormIndex = 0;
            }

            ApplyGenderForm();
            UpdateVariantsVisibilityForCurrent();
        }

        private void LoadAlternateForms()
        {
            using (var context = new AppDbContext())
            {
                string baseDex = Pokemon.Dex.Split('.')[0];
                _alternateForms = context.PokedexEntries
                    .Where(p => p.Dex == baseDex || p.Dex.StartsWith(baseDex + "."))
                    .OrderBy(p => p.Dex)
                    .ToList();
            }
            HasMultipleForms = _alternateForms.Count > 1;
        }

        private void LoadAdjacentPokemonPresence()
        {
            using (var context = new AppDbContext())
            {
                string currentDexStr = Pokemon.Dex;
                var allDexes = context.PokedexEntries.Select(p => p.Dex).Distinct().ToList();
                var sortedDexes = allDexes.OrderBy(d => { if (double.TryParse(d, out double num)) return num; return 99999; }).ToList();

                int currentIndex = sortedDexes.IndexOf(currentDexStr);
                HasPreviousPokemon = currentIndex > 0;
                HasNextPokemon = currentIndex < sortedDexes.Count - 1 && currentIndex >= 0;
            }
        }

        private void LoadEvolutionData()
        {
            using (var context = new AppDbContext())
            {
                var root = Pokemon;
                HashSet<string> visitedRoots = new HashSet<string>();
                visitedRoots.Add(root.Dex);

                while (root != null && root.Evolution != null && !string.IsNullOrEmpty(root.Evolution.EvolvesFrom))
                {
                    var parent = context.PokedexEntries.FirstOrDefault(p => p.Name.ToLower() == root.Evolution.EvolvesFrom.ToLower());
                    if (parent == null) break;

                    if (visitedRoots.Contains(parent.Dex)) break;
                    visitedRoots.Add(parent.Dex);

                    root = parent;
                }

                if (root == null) root = Pokemon;

                _allEvolutionPaths = new List<List<EvolutionNode>>();

                void BuildPaths(PokedexEntry current, List<EvolutionNode> currentPath, EvolutionDetail incomingEvo)
                {
                    var node = new EvolutionNode
                    {
                        Species = current.Name,
                        ImagePath = GetFormattedImagePathForEvolution(current.Image),
                        TargetDex = current.Dex,
                        IsCurrent = current.Dex == Pokemon.Dex,
                        ShowIncomingArrow = incomingEvo != null,
                        MinLevel = incomingEvo?.MinLevel?.ToString(),
                        Item = incomingEvo?.Item,
                        Method = incomingEvo?.Method
                    };

                    if (incomingEvo != null)
                    {
                        string conditionVal = "";
                        var conditionProp = incomingEvo.GetType().GetProperty("Condition");
                        if (conditionProp != null)
                            conditionVal = conditionProp.GetValue(incomingEvo)?.ToString() ?? "";

                        node.ProcessarRegrasDeEvolucao(incomingEvo.Method, incomingEvo.Item, conditionVal, incomingEvo.MinLevel);
                    }

                    var newPath = new List<EvolutionNode>(currentPath) { node };

                    if (current.Evolution?.EvolvesTo != null && current.Evolution.EvolvesTo.Count > 0)
                    {
                        foreach (var evo in current.Evolution.EvolvesTo)
                        {
                            var next = context.PokedexEntries.FirstOrDefault(p => p.Name.ToLower() == evo.Species.ToLower());
                            if (next != null)
                            {
                                if (currentPath.Any(n => n.TargetDex == next.Dex)) continue;
                                BuildPaths(next, newPath, evo);
                            }
                        }
                    }
                    else
                    {
                        _allEvolutionPaths.Add(newPath);
                    }
                }

                BuildPaths(root, new List<EvolutionNode>(), null);

                if (Pokemon.Dex != root.Dex)
                    _allEvolutionPaths = _allEvolutionPaths.Where(path => path.Any(n => n.IsCurrent)).ToList();

                if (_allEvolutionPaths.Count == 0)
                {
                    HasEvolution = false;
                    HasMultipleEvolutionPaths = false;
                }
                else
                {
                    _currentPathIndex = _allEvolutionPaths.FindIndex(path => path.Any(n => n.IsCurrent));
                    if (_currentPathIndex == -1) _currentPathIndex = 0;

                    CurrentEvolutionLine = _allEvolutionPaths[_currentPathIndex];
                    HasEvolution = CurrentEvolutionLine.Count > 1;
                    HasMultipleEvolutionPaths = _allEvolutionPaths.Count > 1;
                }
            }
        }

        private string GetFormattedImagePathForEvolution(string rawPath)
        {
            if (string.IsNullOrWhiteSpace(rawPath)) return "";
            if (IsShiny && !rawPath.Contains("/shiny/"))
                return rawPath.Replace("/normal/", "/shiny/");
            return rawPath;
        }

        public void NavigateToPokemonByDex(string dex)
        {
            if (string.IsNullOrEmpty(dex) || Pokemon.Dex == dex) return;

            using (var context = new AppDbContext())
            {
                var targetPokemon = context.PokedexEntries.FirstOrDefault(p => p.Dex == dex);
                if (targetPokemon != null)
                {
                    Pokemon = targetPokemon;
                    InitializeData(dex);
                }
            }
        }

        public void NextEvolutionPath()
        {
            if (_allEvolutionPaths == null || _allEvolutionPaths.Count <= 1) return;
            _currentPathIndex = (_currentPathIndex + 1) % _allEvolutionPaths.Count;
            CurrentEvolutionLine = _allEvolutionPaths[_currentPathIndex];
        }

        public void PreviousEvolutionPath()
        {
            if (_allEvolutionPaths == null || _allEvolutionPaths.Count <= 1) return;
            _currentPathIndex = (_currentPathIndex - 1 + _allEvolutionPaths.Count) % _allEvolutionPaths.Count;
            CurrentEvolutionLine = _allEvolutionPaths[_currentPathIndex];
        }

        private void ApplyGenderForm()
        {
            if (_alternateForms == null || _alternateForms.Count == 0) return;

            var currentForm = _alternateForms[_currentFormIndex];
            Pokemon = currentForm;

            DisplayAbilities = new List<string>(Pokemon.Abilities.Where(a => !string.IsNullOrWhiteSpace(a)));
            UpdateDisplayedImage();
            LoadEvolutionData();
        }

        private bool FormHasShiny(PokedexEntry form) => !string.IsNullOrWhiteSpace(form.Shiny);
        private bool FormHasFemale(PokedexEntry form) => !string.IsNullOrWhiteSpace(form.Female);

        private void UpdateVariantsVisibilityForCurrent()
        {
            if (_alternateForms == null || _alternateForms.Count == 0) return;

            var currentForm = _alternateForms[_currentFormIndex];
            HasShinyVariant = FormHasShiny(currentForm);
            HasFemaleVariant = FormHasFemale(currentForm);

            if (!HasShinyVariant) IsShiny = false;
            if (!HasFemaleVariant) IsFemale = false;

            HasMegaEvolution = currentForm.Name.Contains("Mega ", StringComparison.OrdinalIgnoreCase) ||
                               currentForm.Name.Contains("Primal ", StringComparison.OrdinalIgnoreCase);

            HasGigantamax = currentForm.Name.Contains("Gigantamax", StringComparison.OrdinalIgnoreCase) ||
                            currentForm.Name.Contains("G-Max", StringComparison.OrdinalIgnoreCase);
        }

        public void GoToNextPokemon()
        {
            using (var context = new AppDbContext())
            {
                string currentDexStr = Pokemon.Dex;
                var allDexes = context.PokedexEntries.Select(p => p.Dex).Distinct().ToList();
                var sortedDexes = allDexes.OrderBy(d => { if (double.TryParse(d, out double num)) return num; return 99999; }).ToList();

                int currentIndex = sortedDexes.IndexOf(currentDexStr);
                if (currentIndex < sortedDexes.Count - 1)
                {
                    string nextDexStr = sortedDexes[currentIndex + 1];
                    var nextPokemon = context.PokedexEntries.FirstOrDefault(p => p.Dex == nextDexStr && (p.Forms == "" || p.Forms == null)) ?? context.PokedexEntries.FirstOrDefault(p => p.Dex == nextDexStr);
                    if (nextPokemon != null)
                    {
                        Pokemon = nextPokemon;
                        _isShiny = false;
                        _isFemale = false;
                        InitializeData();
                    }
                }
            }
        }

        public void GoToPreviousPokemon()
        {
            using (var context = new AppDbContext())
            {
                string currentDexStr = Pokemon.Dex;
                var allDexes = context.PokedexEntries.Select(p => p.Dex).Distinct().ToList();
                var sortedDexes = allDexes.OrderBy(d => { if (double.TryParse(d, out double num)) return num; return 99999; }).ToList();

                int currentIndex = sortedDexes.IndexOf(currentDexStr);
                if (currentIndex > 0)
                {
                    string prevDexStr = sortedDexes[currentIndex - 1];
                    var prevPokemon = context.PokedexEntries.FirstOrDefault(p => p.Dex == prevDexStr && (p.Forms == "" || p.Forms == null)) ?? context.PokedexEntries.FirstOrDefault(p => p.Dex == prevDexStr);
                    if (prevPokemon != null)
                    {
                        Pokemon = prevPokemon;
                        _isShiny = false;
                        _isFemale = false;
                        InitializeData();
                    }
                }
            }
        }

        public void NextForm()
        {
            if (_alternateForms == null || _alternateForms.Count <= 1) return;
            int nextIndex = (_currentFormIndex + 1) % _alternateForms.Count;
            if (IsShiny)
            {
                int startIndex = _currentFormIndex;
                while (!FormHasShiny(_alternateForms[nextIndex]))
                {
                    nextIndex = (nextIndex + 1) % _alternateForms.Count;
                    if (nextIndex == startIndex) return;
                }
            }
            _currentFormIndex = nextIndex;
            UpdateVariantsVisibilityForCurrent();
            ApplyGenderForm();
        }

        public void PreviousForm()
        {
            if (_alternateForms == null || _alternateForms.Count <= 1) return;
            int prevIndex = (_currentFormIndex - 1 + _alternateForms.Count) % _alternateForms.Count;
            if (IsShiny)
            {
                int startIndex = _currentFormIndex;
                while (!FormHasShiny(_alternateForms[prevIndex]))
                {
                    prevIndex = (prevIndex - 1 + _alternateForms.Count) % _alternateForms.Count;
                    if (prevIndex == startIndex) return;
                }
            }
            _currentFormIndex = prevIndex;
            UpdateVariantsVisibilityForCurrent();
            ApplyGenderForm();
        }

        private void UpdateDisplayedImage()
        {
            if (Pokemon == null) return;
            string targetImage = Pokemon.Image ?? "";
            if (string.IsNullOrWhiteSpace(targetImage)) targetImage = Pokemon.Image ?? "";
            if (IsShiny && !string.IsNullOrWhiteSpace(targetImage) && !targetImage.Contains("/shiny/"))
                targetImage = targetImage.Replace("/normal/", "/shiny/");

            string fileName = Path.GetFileName(targetImage);
            string nameOnly = Path.GetFileNameWithoutExtension(fileName).Replace("_female", "", StringComparison.OrdinalIgnoreCase).Replace("_shiny", "", StringComparison.OrdinalIgnoreCase);
            string ext = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(ext)) ext = ".png";

            string suffix = IsFemale ? "_female" : "";
            DisplayedImage = $"{(IsShiny ? "assets/pokemon_images/shiny" : "assets/pokemon_images/normal")}/{nameOnly}{suffix}{ext}";
        }
    }

    public class EvolutionNode
    {
        public string Species { get; set; }
        public string ImagePath { get; set; }
        public string TargetDex { get; set; }
        public bool IsCurrent { get; set; }
        public bool ShowIncomingArrow { get; set; }
        public string MinLevel { get; set; }
        public string Item { get; set; }
        public string Method { get; set; }

        public string EvoItemImagePath { get; set; }
        public string EvoConditionText { get; set; }
        public string EvoToolTip { get; set; }

        public void ProcessarRegrasDeEvolucao(string methodRaw, string item, string condition, int? minLevel)
        {
            string imgName = "Lvl_up";
            string text = "";
            string toolTip = "";

            string method = (methodRaw ?? "").ToLower();
            string originalMethod = methodRaw ?? "";
            item = item ?? "";
            condition = condition ?? "";

            string extraFromMethod = "";
            int plusIndex = originalMethod.IndexOf('+');
            if (plusIndex >= 0 && plusIndex < originalMethod.Length - 1)
            {
                extraFromMethod = originalMethod.Substring(plusIndex + 1).Trim();
            }

            string finalCondition = !string.IsNullOrWhiteSpace(condition) ? condition : extraFromMethod;

            if (method.Contains("use item") || method.Contains("stone"))
            {
                imgName = !string.IsNullOrWhiteSpace(item) ? item : "Lvl_up";
                text = finalCondition;
            }
            // ==========================================
            // NOVA REGRA DE TRADE
            // ==========================================
            else if (method.Contains("trade"))
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    imgName = item;
                    text = "Trade"; // Força a dizer Trade
                }
                else if (method.Contains("holding"))
                {
                    // Tenta caçar o nome do item mágico no meio da frase "holding [ItemName]"
                    int holdingIndex = originalMethod.ToLower().IndexOf("holding");
                    imgName = originalMethod.Substring(holdingIndex + "holding".Length).Trim();
                    text = "Trade"; // Força a dizer Trade
                }
                else
                {
                    imgName = "Linking_Cord";
                    // Se não tiver item, mas tiver outra condição
                    text = string.IsNullOrWhiteSpace(finalCondition) ? "Trade" : finalCondition;
                }
            }
            // ==========================================
            else if (method.Contains("level-up") || method.Contains("level up"))
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    imgName = item;
                    text = string.IsNullOrWhiteSpace(finalCondition) ? "Lv. Up" : finalCondition;
                }
                else
                {
                    imgName = "Lvl_up";
                    if (!string.IsNullOrWhiteSpace(finalCondition))
                        text = finalCondition;
                    else if (minLevel.HasValue && minLevel.Value > 1)
                        text = $"Lv. {minLevel.Value}";
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(finalCondition))
                    text = finalCondition;
                else if (minLevel.HasValue && minLevel.Value > 1)
                    text = $"Lv. {minLevel.Value}";
            }

            toolTip = originalMethod;
            if (!string.IsNullOrWhiteSpace(item) && !originalMethod.Contains(item, StringComparison.OrdinalIgnoreCase))
                toolTip += $" ({item})";
            if (minLevel.HasValue && minLevel.Value > 1 && !toolTip.Contains(minLevel.Value.ToString()))
                toolTip += $" (Lv. {minLevel.Value})";

            if (!string.IsNullOrWhiteSpace(text) && text.Length > 0)
                text = char.ToUpper(text[0]) + text.Substring(1);

            imgName = imgName.Replace(" ", "_").Replace("'", "").Replace("é", "e").Replace(":", "") + ".png";

            EvoItemImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "evolution_itens", imgName);
            EvoConditionText = text;
            EvoToolTip = toolTip.Trim();
        }
    }
}