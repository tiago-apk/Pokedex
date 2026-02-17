using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging; // ➔ ADICIONADO PARA LER A IMAGEM
using Pokedex.data;
using Pokedex.models;

namespace Pokedex.viewmodels
{
    public enum StatDisplayMode { Calculated, IV, EV }

    public class RegisteredPokemonDetailViewModel : INotifyPropertyChanged
    {
        private StatDisplayMode _currentMode = StatDisplayMode.Calculated;
        private int _currentPokemonId;
        private int _trainerId;

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
                // Avisamos a Janela que estas informações formatadas também mudaram!
                OnPropertyChanged(nameof(DisplayNickname));
                OnPropertyChanged(nameof(HeaderTitle));
                OnPropertyChanged(nameof(DisplayMove1));
                OnPropertyChanged(nameof(DisplayMove2));
                OnPropertyChanged(nameof(DisplayMove3));
                OnPropertyChanged(nameof(DisplayMove4));
            }
        }

        private PokedexEntry _baseDex;
        public PokedexEntry BaseDex
        {
            get => _baseDex;
            set { _baseDex = value; OnPropertyChanged(); }
        }

        private Nature _natureDb;

        // =========================================================
        // PROPRIEDADES FORMATADAS PARA A TELA (RESOLVEM OS ERROS)
        // =========================================================
        public string DisplayNickname => Pokemon != null && !string.IsNullOrWhiteSpace(Pokemon.Nickname) ? Pokemon.Nickname : Pokemon?.Form;

        public string HeaderTitle => Pokemon != null ? $"{Pokemon.TrainerName.ToUpper()}'S POKÉMON" : "P O K É M O N   S T A T U S";

        public string DisplayMove1 => string.IsNullOrWhiteSpace(Pokemon?.Move1) ? "-" : Pokemon.Move1;
        public string DisplayMove2 => string.IsNullOrWhiteSpace(Pokemon?.Move2) ? "-" : Pokemon.Move2;
        public string DisplayMove3 => string.IsNullOrWhiteSpace(Pokemon?.Move3) ? "-" : Pokemon.Move3;
        public string DisplayMove4 => string.IsNullOrWhiteSpace(Pokemon?.Move4) ? "-" : Pokemon.Move4;

        // Propriedade que envia a Imagem já renderizada para a Janela
        private ImageSource _pokemonImageSource;
        public ImageSource PokemonImageSource
        {
            get => _pokemonImageSource;
            set { _pokemonImageSource = value; OnPropertyChanged(); }
        }

        private string _chartTitle = "CALCULATED STATS";
        public string ChartTitle { get => _chartTitle; set { _chartTitle = value; OnPropertyChanged(); } }

        private Brush _chartFillColor;
        public Brush ChartFillColor { get => _chartFillColor; set { _chartFillColor = value; OnPropertyChanged(); } }

        private Brush _chartStrokeColor;
        public Brush ChartStrokeColor { get => _chartStrokeColor; set { _chartStrokeColor = value; OnPropertyChanged(); } }

        private string _hexagonPointsString;
        public string HexagonPointsString { get => _hexagonPointsString; set { _hexagonPointsString = value; OnPropertyChanged(); } }

        public ObservableCollection<StatPoint> AllStats { get; set; }

        public RegisteredPokemonDetailViewModel(int pokemonId, int trainerId)
        {
            _currentPokemonId = pokemonId;
            _trainerId = trainerId;
            AllStats = new ObservableCollection<StatPoint>();

            LoadPokemonData();
        }

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set { _imagePath = value; OnPropertyChanged(); }
        }

        public void LoadPokemonData()
        {
            using (var db = new AppDbContext())
            {
                Pokemon = db.RegisteredPokemons.FirstOrDefault(p => p.Id == _currentPokemonId);
                if (Pokemon == null) return;

                BaseDex = db.PokedexEntries.FirstOrDefault(d => d.Name == Pokemon.Form);
                _natureDb = db.Natures.FirstOrDefault(n => n.Name == Pokemon.Nature);

                LoadNavigationIds(db);
            }

            // ➔ CORREÇÃO DA IMAGEM: Apenas enviamos a string do caminho!
            if (BaseDex != null)
            {
                ImagePath = Pokemon.IsShiny && !string.IsNullOrWhiteSpace(BaseDex.Shiny) ? BaseDex.Shiny : BaseDex.Image;
            }

            AllStats.Clear();
            AllStats.Add(new StatPoint("HP", BaseDex?.Hp ?? 0, Pokemon.IvHp, Pokemon.EvHp, Pokemon.Level, _natureDb, _currentMode));
            AllStats.Add(new StatPoint("Attack", BaseDex?.Atk ?? 0, Pokemon.IvAtk, Pokemon.EvAtk, Pokemon.Level, _natureDb, _currentMode));
            AllStats.Add(new StatPoint("Defense", BaseDex?.Def ?? 0, Pokemon.IvDef, Pokemon.EvDef, Pokemon.Level, _natureDb, _currentMode));
            AllStats.Add(new StatPoint("Sp. Atk", BaseDex?.Spa ?? 0, Pokemon.IvSpa, Pokemon.EvSpa, Pokemon.Level, _natureDb, _currentMode));
            AllStats.Add(new StatPoint("Sp. Def", BaseDex?.Spd ?? 0, Pokemon.IvSpd, Pokemon.EvSpd, Pokemon.Level, _natureDb, _currentMode));
            AllStats.Add(new StatPoint("Speed", BaseDex?.Spe ?? 0, Pokemon.IvSpe, Pokemon.EvSpe, Pokemon.Level, _natureDb, _currentMode));

            UpdateColors();
            CalculatePolygonPoints();
        }

        private void LoadNavigationIds(AppDbContext db)
        {
            var trainerPokemonIds = db.RegisteredPokemons
                                      .Where(p => p.TrainerId == _trainerId)
                                      .OrderBy(p => p.Id)
                                      .Select(p => p.Id)
                                      .ToList();

            int currentIndex = trainerPokemonIds.IndexOf(_currentPokemonId);
            PreviousId = currentIndex > 0 ? trainerPokemonIds[currentIndex - 1] : (int?)null;
            NextId = currentIndex >= 0 && currentIndex < trainerPokemonIds.Count - 1 ? trainerPokemonIds[currentIndex + 1] : (int?)null;
        }

        public void ToggleStatsMode()
        {
            _currentMode = _currentMode switch
            {
                StatDisplayMode.Calculated => StatDisplayMode.EV,
                StatDisplayMode.EV => StatDisplayMode.IV,
                StatDisplayMode.IV => StatDisplayMode.Calculated,
                _ => StatDisplayMode.Calculated
            };

            ChartTitle = _currentMode switch
            {
                StatDisplayMode.Calculated => "CALCULATED STATS",
                StatDisplayMode.IV => "INDIVIDUAL VALUES (IVs)",
                StatDisplayMode.EV => "EFFORT VALUES (EVs)",
                _ => "STATS"
            };

            foreach (var stat in AllStats) stat.Mode = _currentMode;

            UpdateColors();
            CalculatePolygonPoints();
        }

        private void UpdateColors()
        {
            var converter = new BrushConverter();
            switch (_currentMode)
            {
                case StatDisplayMode.Calculated:
                    ChartFillColor = (SolidColorBrush)converter.ConvertFromString("#B39DDB"); // Deep Purple 200
                    ChartStrokeColor = (SolidColorBrush)converter.ConvertFromString("#651FFF"); // Deep Purple A400
                    break;
                case StatDisplayMode.EV:
                    ChartFillColor = (SolidColorBrush)converter.ConvertFromString("#80DEEA"); // Cyan 200
                    ChartStrokeColor = (SolidColorBrush)converter.ConvertFromString("#00E5FF"); // Cyan A400
                    break;
                case StatDisplayMode.IV:
                    ChartFillColor = (SolidColorBrush)converter.ConvertFromString("#FFCC80"); // Orange 200
                    ChartStrokeColor = (SolidColorBrush)converter.ConvertFromString("#FF9100"); // Orange A400
                    break;
            }
        }

        private void CalculatePolygonPoints()
        {
            double centerX = 140, centerY = 140, maxRadius = 100;
            double maxVal = _currentMode switch { StatDisplayMode.Calculated => 500.0, StatDisplayMode.EV => 255.0, StatDisplayMode.IV => 31.0, _ => 100.0 };

            var points = new List<System.Windows.Point>();
            double[] angles = { 270, 330, 30, 90, 150, 210 };

            for (int i = 0; i < 6; i++)
            {
                double val = _currentMode switch { StatDisplayMode.Calculated => AllStats[i].FinalStat, StatDisplayMode.EV => AllStats[i].EV, StatDisplayMode.IV => AllStats[i].IV, _ => 0 };
                double ratio = Math.Max(0, Math.Min(1.0, val / maxVal));

                double r = maxRadius * ratio;
                double rad = angles[i] * Math.PI / 180.0;
                points.Add(new System.Windows.Point(centerX + r * Math.Cos(rad), centerY + r * Math.Sin(rad)));
            }
            HexagonPointsString = string.Join(" ", points.Select(p => $"{p.X.ToString(System.Globalization.CultureInfo.InvariantCulture)},{p.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)}"));
        }

        public void NextPokemon() { if (HasNext) { _currentPokemonId = NextId.Value; LoadPokemonData(); } }
        public void PreviousPokemon() { if (HasPrevious) { _currentPokemonId = PreviousId.Value; LoadPokemonData(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class StatPoint : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public int BaseStat { get; set; }
        public int IV { get; set; }
        public int EV { get; set; }
        public int FinalStat { get; set; }
        public string NatureSymbol { get; set; }
        public Brush SymbolColor { get; set; }

        private StatDisplayMode _mode;
        public StatDisplayMode Mode
        {
            get => _mode;
            set { _mode = value; OnPropertyChanged(nameof(DisplayValue)); }
        }

        public string DisplayValue => _mode switch { StatDisplayMode.Calculated => FinalStat.ToString(), StatDisplayMode.IV => IV.ToString(), StatDisplayMode.EV => EV.ToString(), _ => "" };

        public StatPoint(string name, int baseStat, int iv, int ev, int level, Nature natureDb, StatDisplayMode initialMode)
        {
            Name = name; BaseStat = baseStat; IV = iv; EV = ev; _mode = initialMode;
            CalculateFinalStat(level, natureDb);
            SetNatureSymbols(natureDb);
        }

        private void CalculateFinalStat(int level, Nature natureDb)
        {
            if (Name == "HP") FinalStat = (int)Math.Floor((2 * BaseStat + IV + Math.Floor(EV / 4.0)) * level / 100.0) + level + 10;
            else
            {
                double statBeforeNature = Math.Floor((2 * BaseStat + IV + Math.Floor(EV / 4.0)) * level / 100.0) + 5;
                double natureMod = 1.0;
                if (natureDb != null)
                {
                    string target = Name.ToLower() == "sp. atk" ? "special attack" : Name.ToLower() == "sp. def" ? "special defense" : Name.ToLower();
                    string inc = natureDb.Increase?.ToLower() ?? "";
                    string dec = natureDb.Decrease?.ToLower() ?? "";
                    if (inc.Contains(target)) natureMod = 1.1; else if (dec.Contains(target)) natureMod = 0.9;
                }
                FinalStat = (int)Math.Floor(statBeforeNature * natureMod);
            }
        }

        private void SetNatureSymbols(Nature natureDb)
        {
            if (natureDb == null || Name == "HP") { NatureSymbol = ""; return; }
            string target = Name.ToLower() == "sp. atk" ? "special attack" : Name.ToLower() == "sp. def" ? "special defense" : Name.ToLower();
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