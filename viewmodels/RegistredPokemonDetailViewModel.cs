using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Pokedex.data;
using Pokedex.models;

namespace Pokedex.viewmodels
{
    public enum StatDisplayMode { Calculated, IV, EV }

    public class DexInfoWrapper
    {
        public string Dex { get; set; }
    }

    public class RegisteredPokemonDetailViewModel : INotifyPropertyChanged
    {
        private StatDisplayMode _currentMode = StatDisplayMode.Calculated;
        private int _currentPokemonId;
        private string _trainerId;
        private Pokemon _basePokemon;

        public int? PreviousId { get; private set; }
        public int? NextId { get; private set; }
        public bool HasPrevious => PreviousId.HasValue;
        public bool HasNext => NextId.HasValue;

        private RegisteredPokemon _pokemon;
        public RegisteredPokemon Pokemon
        {
            get => _pokemon;
            set
            {
                _pokemon = value;
                OnPropertyChanged();
                RefreshAll();
            }
        }

        public DexInfoWrapper BaseDex => new DexInfoWrapper { Dex = _basePokemon?.NationalDex?.ToString("D4") ?? "???" };

        public string ImagePath
        {
            get
            {
                if (Pokemon == null || _basePokemon == null) return null;
                return (Pokemon.IsShiny && !string.IsNullOrWhiteSpace(_basePokemon.ImageShiny))
                        ? _basePokemon.ImageShiny
                        : _basePokemon.ImageNormal;
            }
        }

        public string DisplayNickname => !string.IsNullOrWhiteSpace(Pokemon?.Nickname) ? Pokemon.Nickname : Pokemon?.PokemonId;
        public string HeaderTitle => $"{DisplayNickname} (Nív. {Pokemon?.Level})";
        public string DisplayMove1 => Pokemon?.Move1 ?? "---";
        public string DisplayMove2 => Pokemon?.Move2 ?? "---";
        public string DisplayMove3 => Pokemon?.Move3 ?? "---";
        public string DisplayMove4 => Pokemon?.Move4 ?? "---";

        public ObservableCollection<StatItemViewModel> AllStats { get; set; } = new ObservableCollection<StatItemViewModel>();

        public string ChartTitle => _currentMode switch
        {
            StatDisplayMode.IV => "Individual Values (IVs)",
            StatDisplayMode.EV => "Effort Values (EVs)",
            _ => "Pokemon Stats"
        };

        public Brush ChartFillColor => _currentMode switch
        {
            StatDisplayMode.IV => new SolidColorBrush(Color.FromArgb(120, 156, 39, 176)),
            StatDisplayMode.EV => new SolidColorBrush(Color.FromArgb(120, 255, 152, 0)),
            _ => new SolidColorBrush(Color.FromArgb(120, 33, 150, 243))
        };

        public Brush ChartStrokeColor => _currentMode switch
        {
            StatDisplayMode.IV => Brushes.Purple,
            StatDisplayMode.EV => Brushes.DarkOrange,
            _ => Brushes.DodgerBlue
        };

        public PointCollection HexagonPointsString
        {
            get
            {
                var points = new PointCollection();
                if (AllStats == null || AllStats.Count < 6) return points;


                double centerX = 140;
                double centerY = 140;
                double maxRadius = 100;
                double minRadius = 0;

                double maxVal = _currentMode switch
                {
                    StatDisplayMode.IV => 31,
                    StatDisplayMode.EV => 252,
                    _ => Math.Max(150, AllStats.Max(s => s.NumericValue))
                };
                int[] drawOrder = { 0, 1, 2, 5, 4, 3 };
                double[] angles = { 270, 330, 30, 90, 150, 210 };

                for (int i = 0; i < 6; i++)
                {
                    int statIndex = drawOrder[i];
                    double val = AllStats[statIndex].NumericValue;

                    double r = (val / maxVal) * maxRadius;
                    if (r > maxRadius) r = maxRadius;
                    if (r < minRadius) r = minRadius;

                    // Matemática do WPF para encontrar os pontos X,Y
                    double rad = angles[i] * Math.PI / 180.0;
                    double x = centerX + r * Math.Cos(rad);
                    double y = centerY + r * Math.Sin(rad);

                    points.Add(new Point(x, y));
                }
                return points;
            }
        }
        public RegisteredPokemonDetailViewModel(int id, string trainerId)
        {
            _currentPokemonId = id;
            _trainerId = trainerId;
            LoadPokemon(id);
        }

        public void LoadPokemon(int id)
        {
            using (var db = new AppDbContext())
            {
                Pokemon = db.RegisteredPokemons
                            .FirstOrDefault(p => p.Id == id);

                if (Pokemon == null) return;

                _basePokemon = db.Pokemons.FirstOrDefault(p => p.Id == Pokemon.PokemonId);

                var allForTrainer = db.RegisteredPokemons
                                      .Where(p => p.TrainerId == _trainerId)
                                      .OrderBy(p => p.Id)
                                      .Select(p => p.Id)
                                      .ToList();

                int index = allForTrainer.IndexOf(id);
                PreviousId = index > 0 ? allForTrainer[index - 1] : (int?)null;
                NextId = index < allForTrainer.Count - 1 ? allForTrainer[index + 1] : (int?)null;

                RefreshAll();
            }
        }

        public void PreviousPokemon() { if (HasPrevious) LoadPokemon(PreviousId.Value); }
        public void NextPokemon() { if (HasNext) LoadPokemon(NextId.Value); }

        public void ToggleStatsMode()
        {
            if (_currentMode == StatDisplayMode.Calculated) SwitchMode(StatDisplayMode.IV);
            else if (_currentMode == StatDisplayMode.IV) SwitchMode(StatDisplayMode.EV);
            else SwitchMode(StatDisplayMode.Calculated);
        }

        public void SwitchMode(StatDisplayMode mode)
        {
            _currentMode = mode;
            UpdateStats();
        }

        private void RefreshAll()
        {
            OnPropertyChanged(nameof(DisplayNickname));
            OnPropertyChanged(nameof(HeaderTitle));
            OnPropertyChanged(nameof(DisplayMove1));
            OnPropertyChanged(nameof(DisplayMove2));
            OnPropertyChanged(nameof(DisplayMove3));
            OnPropertyChanged(nameof(DisplayMove4));
            OnPropertyChanged(nameof(HasPrevious));
            OnPropertyChanged(nameof(HasNext));
            OnPropertyChanged(nameof(BaseDex));
            OnPropertyChanged(nameof(ImagePath));

            UpdateStats();
        }

        private void UpdateStats()
        {
            if (Pokemon == null) return;

            using (var db = new AppDbContext())
            {
                var nature = db.Natures.FirstOrDefault(n => n.Name == Pokemon.Nature);

                AllStats.Clear();
                AllStats.Add(new StatItemViewModel("HP", _basePokemon?.Hp ?? 0, Pokemon.IvHp, Pokemon.EvHp, Pokemon.Level, "hp", nature, _currentMode));
                AllStats.Add(new StatItemViewModel("Atk", _basePokemon?.Attack ?? 0, Pokemon.IvAtk, Pokemon.EvAtk, Pokemon.Level, "attack", nature, _currentMode));
                AllStats.Add(new StatItemViewModel("Def", _basePokemon?.Defense ?? 0, Pokemon.IvDef, Pokemon.EvDef, Pokemon.Level, "defense", nature, _currentMode));
                AllStats.Add(new StatItemViewModel("Sp. Atk", _basePokemon?.SpAtk ?? 0, Pokemon.IvSpa, Pokemon.EvSpa, Pokemon.Level, "special attack", nature, _currentMode));
                AllStats.Add(new StatItemViewModel("Sp. Def", _basePokemon?.SpDef ?? 0, Pokemon.IvSpd, Pokemon.EvSpd, Pokemon.Level, "special defense", nature, _currentMode));
                AllStats.Add(new StatItemViewModel("Spe", _basePokemon?.Speed ?? 0, Pokemon.IvSpe, Pokemon.EvSpe, Pokemon.Level, "speed", nature, _currentMode));

                OnPropertyChanged(nameof(ChartTitle));
                OnPropertyChanged(nameof(ChartFillColor));
                OnPropertyChanged(nameof(ChartStrokeColor));
                OnPropertyChanged(nameof(HexagonPointsString));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class StatItemViewModel : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public int NumericValue { get; set; }
        public string DisplayValue => NumericValue.ToString();
        public string NatureSymbol { get; set; }
        public SolidColorBrush SymbolColor { get; set; }

        public StatItemViewModel(string name, int @base, int iv, int ev, int level, string target, Nature natureDb, StatDisplayMode mode)
        {
            Name = name;
            Calculate(@base, iv, ev, level, target, natureDb, mode);
            SetNatureSymbols(natureDb);
        }

        private void Calculate(int @base, int iv, int ev, int level, string target, Nature natureDb, StatDisplayMode mode)
        {
            if (mode == StatDisplayMode.IV) { NumericValue = iv; }
            else if (mode == StatDisplayMode.EV) { NumericValue = ev; }
            else
            {
                double natureMod = 1.0;
                int statBeforeNature;

                if (Name == "HP")
                {
                    statBeforeNature = (int)Math.Floor((double)((2 * @base + iv + (ev / 4)) * level) / 100) + level + 10;
                }
                else
                {
                    statBeforeNature = (int)Math.Floor((double)((2 * @base + iv + (ev / 4)) * level) / 100) + 5;
                    string inc = natureDb?.Increase?.ToLower() ?? "";
                    string dec = natureDb?.Decrease?.ToLower() ?? "";
                    if (inc.Contains(target)) natureMod = 1.1; else if (dec.Contains(target)) natureMod = 0.9;
                }
                NumericValue = (int)Math.Floor(statBeforeNature * natureMod);
            }
        }

        private void SetNatureSymbols(Nature natureDb)
        {
            if (natureDb == null || Name == "HP") { NatureSymbol = ""; return; }
            string target = Name.ToLower() switch
            {
                "sp. atk" => "special attack",
                "sp. def" => "special defense",
                _ => Name.ToLower()
            };
            string inc = natureDb.Increase?.ToLower() ?? "";
            string dec = natureDb.Decrease?.ToLower() ?? "";

            if (inc.Contains(target)) { NatureSymbol = "▲"; SymbolColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#304FFE")); }
            else if (dec.Contains(target)) { NatureSymbol = "▼"; SymbolColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D50000")); }
            else { NatureSymbol = ""; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}