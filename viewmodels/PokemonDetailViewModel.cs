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
            set { _isShiny = value; OnPropertyChanged(); UpdateDisplayedImage(); }
        }

        private bool _isFemale;
        public bool IsFemale
        {
            get => _isFemale;
            set { _isFemale = value; OnPropertyChanged(); ApplyGenderForm(); }
        }

        // ==========================================
        // NOVAS PROPRIEDADES: NAVEGAÇÃO DA POKÉDEX
        // ==========================================
        private bool _hasPreviousPokemon;
        public bool HasPreviousPokemon
        {
            get => _hasPreviousPokemon;
            set { _hasPreviousPokemon = value; OnPropertyChanged(); }
        }

        private bool _hasNextPokemon;
        public bool HasNextPokemon
        {
            get => _hasNextPokemon;
            set { _hasNextPokemon = value; OnPropertyChanged(); }
        }

        private PokedexEntry _previousPokemonEntry;
        private PokedexEntry _nextPokemonEntry;


        private bool _hasFemaleVariant;
        public bool HasFemaleVariant
        {
            get => _hasFemaleVariant;
            set { _hasFemaleVariant = value; OnPropertyChanged(); }
        }

        private bool _hasShinyVariant;
        public bool HasShinyVariant
        {
            get => _hasShinyVariant;
            set { _hasShinyVariant = value; OnPropertyChanged(); }
        }

        private bool _hasMultipleForms;
        public bool HasMultipleForms
        {
            get => _hasMultipleForms;
            set { _hasMultipleForms = value; OnPropertyChanged(); }
        }

        private List<PokedexEntry> _alternateForms;
        private List<PokedexEntry> _allEntriesForThisDex;
        private int _currentFormIndex = 0;

        private PokedexEntry _maleForm;
        private PokedexEntry _femaleForm;

        public PokemonDetailViewModel(PokedexEntry entry)
        {
            Pokemon = entry;
            InitializeData();
        }

        private void InitializeData()
        {
            LoadNavigationData();
            LoadFormsData();
            UpdateVariantsVisibilityForCurrent();
            ApplyGenderForm();
        }

        // ==========================================
        // MÁGICA DA NAVEGAÇÃO (PRÓXIMO/ANTERIOR)
        // ==========================================
        private int ParseDex(string dexStr)
        {
            if (string.IsNullOrEmpty(dexStr)) return 99999;
            string clean = dexStr.Replace("#", "").Trim().Split('.')[0].Split('_')[0].Split('-')[0];
            int.TryParse(clean, out int res);
            return res;
        }

        private void LoadNavigationData()
        {
            using (var db = new AppDbContext())
            {
                // Pega TODOS os Pokémons Base (Aqueles que NÃO possuem '.' no Dex)
                var basePokes = db.PokedexEntries
                    .ToList()
                    .Where(p => !string.IsNullOrEmpty(p.Dex) && !p.Dex.Contains("."))
                    .OrderBy(p => ParseDex(p.Dex)) // Ordena numericamente pelo número da Dex
                    .ToList();

                string currentBaseDex = ExtractBaseDex(Pokemon.Dex);
                var currentBase = basePokes.FirstOrDefault(p => ParseDex(p.Dex) == ParseDex(currentBaseDex));

                int idx = basePokes.IndexOf(currentBase);

                if (idx >= 0)
                {
                    HasPreviousPokemon = idx > 0;
                    HasNextPokemon = idx < basePokes.Count - 1;

                    _previousPokemonEntry = idx > 0 ? basePokes[idx - 1] : null;
                    _nextPokemonEntry = idx < basePokes.Count - 1 ? basePokes[idx + 1] : null;
                }
                else
                {
                    HasPreviousPokemon = false;
                    HasNextPokemon = false;
                }
            }
        }

        public void GoToNextPokemon()
        {
            if (_nextPokemonEntry != null) ChangePokemon(_nextPokemonEntry);
        }

        public void GoToPreviousPokemon()
        {
            if (_previousPokemonEntry != null) ChangePokemon(_previousPokemonEntry);
        }

        private void ChangePokemon(PokedexEntry newBase)
        {
            Pokemon = newBase;

            // Desliga os filtros visuais ao mudar de Pokémon
            _isShiny = false;
            OnPropertyChanged(nameof(IsShiny));
            _isFemale = false;
            OnPropertyChanged(nameof(IsFemale));
            _currentFormIndex = 0;

            InitializeData();
        }

        // ==========================================
        // RESTO DO CÓDIGO INTACTO
        // ==========================================
        private string ExtractBaseDex(string dexStr)
        {
            if (string.IsNullOrEmpty(dexStr)) return "";
            string clean = dexStr.Replace("#", "").Trim();
            string basePart = clean.Split('.')[0].Split('_')[0].Split('-')[0];
            return basePart.TrimStart('0');
        }

        private void CopyStats(PokedexEntry target, PokedexEntry source)
        {
            if (target == null || source == null) return;
            if (target.Hp == 0)
            {
                target.Hp = source.Hp; target.Atk = source.Atk; target.Def = source.Def;
                target.Spa = source.Spa; target.Spd = source.Spd; target.Spe = source.Spe; target.Bst = source.Bst;
            }
            if (string.IsNullOrWhiteSpace(target.Height)) target.Height = source.Height;
            if (string.IsNullOrWhiteSpace(target.Weight)) target.Weight = source.Weight;
            bool hasAnyType = target.Types != null && target.Types.Any(t => !string.IsNullOrWhiteSpace(t));
            if (!hasAnyType) target.Types = source.Types?.ToList() ?? new List<string>();
            bool hasAnyAbility = target.Abilities != null && target.Abilities.Any(a => !string.IsNullOrWhiteSpace(a));
            if (!hasAnyAbility) target.Abilities = source.Abilities?.ToList() ?? new List<string>();
        }

        private void LoadFormsData()
        {
            using (var db = new AppDbContext())
            {
                var allDbEntries = db.PokedexEntries.ToList();
                if (string.IsNullOrEmpty(Pokemon.Dex)) return;

                string myBaseDex = ExtractBaseDex(Pokemon.Dex);
                _allEntriesForThisDex = allDbEntries.Where(p => ExtractBaseDex(p.Dex) == myBaseDex).OrderBy(p => p.Id).ToList();

                var baseForm = _allEntriesForThisDex.FirstOrDefault(p => !p.Dex.Contains(".") && !p.Dex.Contains("_")) ?? _allEntriesForThisDex.FirstOrDefault();
                _maleForm = _allEntriesForThisDex.FirstOrDefault(p => p.Forms?.ToLower() == "male" || (p.Forms?.ToLower() == "gender" && p.Dex.EndsWith(".1")));
                _femaleForm = _allEntriesForThisDex.FirstOrDefault(p => p.Forms?.ToLower() == "female" || (p.Forms?.ToLower() == "gender" && p.Dex.EndsWith(".2")));

                if (baseForm != null && baseForm.Hp == 0)
                {
                    var filler = _maleForm ?? _allEntriesForThisDex.FirstOrDefault(p => p.Dex.EndsWith(".1"));
                    if (filler != null && filler.Hp > 0) CopyStats(baseForm, filler);
                }

                foreach (var form in _allEntriesForThisDex)
                {
                    if (baseForm != null && form != baseForm)
                    {
                        if (string.IsNullOrWhiteSpace(form.Name)) form.Name = baseForm.Name;
                        CopyStats(form, baseForm);
                    }
                    if (form.Types != null) form.Types = form.Types.Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
                    if (!string.IsNullOrEmpty(form.Dex)) form.Dex = form.Dex.Split('.')[0].Split('_')[0].Split('-')[0];
                }

                _alternateForms = new List<PokedexEntry>();
                var seenImages = new HashSet<string>();

                foreach (var p in _allEntriesForThisDex)
                {
                    string f = p.Forms?.ToLower() ?? "";
                    bool isGenderSpecific = f == "male" || f == "female" || f == "gender";
                    bool hasFemaleSuffix = p.Image != null && p.Image.ToLower().Contains("_female");

                    if (isGenderSpecific || hasFemaleSuffix) continue;

                    string img = Path.GetFileName(p.Image?.ToLower().Trim() ?? "");
                    if (!seenImages.Contains(img))
                    {
                        seenImages.Add(img);
                        _alternateForms.Add(p);
                    }
                }

                if (_alternateForms.Count == 0 && baseForm != null) _alternateForms.Add(baseForm);
                else if (baseForm != null && !_alternateForms.Contains(baseForm)) _alternateForms.Insert(0, baseForm);

                HasMultipleForms = _alternateForms.Count > 1;
                _currentFormIndex = _alternateForms.FindIndex(p => p.Id == Pokemon.Id);
                if (_currentFormIndex == -1) _currentFormIndex = 0;
            }
        }

        private bool FormHasShiny(PokedexEntry entry)
        {
            if (entry == null) return false;
            bool IsValidStr(string s)
            {
                if (string.IsNullOrWhiteSpace(s)) return false;
                string clean = s.Trim().ToLower();
                return clean != "null" && clean != "none" && clean != "false" && clean != "0";
            }
            return IsValidStr(entry.Shiny) || IsValidStr(entry.FemaleShiny);
        }

        private void UpdateVariantsVisibilityForCurrent()
        {
            if (_alternateForms == null || _alternateForms.Count == 0) return;
            var currentBase = _alternateForms[_currentFormIndex];

            bool hasShiny = FormHasShiny(currentBase);
            HasShinyVariant = hasShiny;
            if (!hasShiny && _isShiny) { _isShiny = false; OnPropertyChanged(nameof(IsShiny)); }

            bool hasFemaleCol = !string.IsNullOrWhiteSpace(currentBase.Female) && currentBase.Female.ToLower() != "false" && currentBase.Female != "0";
            bool hasFemaleRow = _allEntriesForThisDex.Any(p => p.Forms?.ToLower() == "female" || p.Forms?.ToLower() == "gender" || (p.Image?.ToLower().Contains("_female") ?? false));
            HasFemaleVariant = hasFemaleCol || _femaleForm != null || hasFemaleRow;

            if (!HasFemaleVariant && _isFemale) { _isFemale = false; OnPropertyChanged(nameof(IsFemale)); }
        }

        private void ApplyGenderForm()
        {
            if (_alternateForms == null || _alternateForms.Count == 0) return;
            var currentBase = _alternateForms[_currentFormIndex];

            if (IsFemale) Pokemon = (_femaleForm != null && _currentFormIndex == 0) ? _femaleForm : currentBase;
            else Pokemon = (_currentFormIndex == 0 && _maleForm != null) ? _maleForm : currentBase;

            UpdateDisplayAbilities();
            UpdateDisplayedImage();
        }

        private void UpdateDisplayAbilities()
        {
            var list = new List<string>();
            if (Pokemon != null && Pokemon.Abilities != null)
            {
                for (int i = 0; i < Pokemon.Abilities.Count; i++)
                {
                    string ab = Pokemon.Abilities[i];
                    if (!string.IsNullOrWhiteSpace(ab)) list.Add(i == 2 ? $"{ab} (HA)" : ab);
                }
            }
            DisplayAbilities = list;
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
}