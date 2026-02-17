using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Pokedex.data;
using Pokedex.models;

namespace Pokedex.views
{
    public partial class RegisteredPokemonDetailView : Window, INotifyPropertyChanged
    {
        // =========================================================
        // MÁGICA DO WINDOWS API PARA ESCONDER BOTÕES NATIVOS
        // =========================================================
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        // =========================================================
        // VARIÁVEIS DE ESTADO E NAVEGAÇÃO
        // =========================================================
        private StatDisplayMode _currentMode = StatDisplayMode.Calculated;
        private RegisteredPokemon _pokemon;
        private PokedexEntry _baseDex;
        private Nature _natureDb;

        private int _currentPokemonId;
        private int _trainerId;
        private List<int> _trainerPokemonIds;

        // =========================================================
        // PROPRIEDADES QUE O XAML VAI LER (BINDING)
        // =========================================================
        public List<CalculatedStat> AllStats { get; private set; }

        private string _chartTitle;
        public string ChartTitle { get => _chartTitle; set { _chartTitle = value; OnPropertyChanged(); } }

        private string _hexagonPointsString;
        public string HexagonPointsString { get => _hexagonPointsString; set { _hexagonPointsString = value; OnPropertyChanged(); } }

        private Brush _chartFillColor;
        public Brush ChartFillColor { get => _chartFillColor; set { _chartFillColor = value; OnPropertyChanged(); } }

        private Brush _chartStrokeColor;
        public Brush ChartStrokeColor { get => _chartStrokeColor; set { _chartStrokeColor = value; OnPropertyChanged(); } }

        // =========================================================
        // CONSTRUTOR
        // =========================================================
        public RegisteredPokemonDetailView(int registeredPokemonId, int trainerId)
        {
            InitializeComponent();

            // Define o DataContext para a própria janela (importante para os Bindings)
            this.DataContext = this;

            this.Loaded += (s, e) =>
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
            };

            _currentPokemonId = registeredPokemonId;
            _trainerId = trainerId;

            LoadTrainerPokemonList();
            LoadPokemonData(_currentPokemonId);
        }

        // =========================================================
        // NAVEGAÇÃO (SETAS E FECHAR)
        // =========================================================
        private void btnClose_Click(object sender, RoutedEventArgs e) => this.Close();

        private void LoadTrainerPokemonList()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    _trainerPokemonIds = db.RegisteredPokemons
                                           .Where(p => p.TrainerId == _trainerId)
                                           .OrderBy(p => p.Id)
                                           .Select(p => p.Id)
                                           .ToList();
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro ao buscar a equipa: " + ex.Message); }
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (_trainerPokemonIds == null || !_trainerPokemonIds.Contains(_currentPokemonId)) return;

            int currentIndex = _trainerPokemonIds.IndexOf(_currentPokemonId);
            if (currentIndex > 0)
            {
                _currentPokemonId = _trainerPokemonIds[currentIndex - 1];
                _currentMode = StatDisplayMode.Calculated;
                LoadPokemonData(_currentPokemonId);
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (_trainerPokemonIds == null || !_trainerPokemonIds.Contains(_currentPokemonId)) return;

            int currentIndex = _trainerPokemonIds.IndexOf(_currentPokemonId);
            if (currentIndex < _trainerPokemonIds.Count - 1)
            {
                _currentPokemonId = _trainerPokemonIds[currentIndex + 1];
                _currentMode = StatDisplayMode.Calculated;
                LoadPokemonData(_currentPokemonId);
            }
        }

        private void UpdateNavigationButtons()
        {
            if (_trainerPokemonIds == null || _trainerPokemonIds.Count == 0)
            {
                btnPrev.Visibility = Visibility.Collapsed;
                btnNext.Visibility = Visibility.Collapsed;
                return;
            }

            int currentIndex = _trainerPokemonIds.IndexOf(_currentPokemonId);
            btnPrev.Visibility = currentIndex > 0 ? Visibility.Visible : Visibility.Collapsed;
            btnNext.Visibility = currentIndex < _trainerPokemonIds.Count - 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        // =========================================================
        // INTERAÇÃO COM O GRÁFICO
        // =========================================================
        private void Stats_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _currentMode = _currentMode switch
            {
                StatDisplayMode.Calculated => StatDisplayMode.IV,
                StatDisplayMode.IV => StatDisplayMode.EV,
                StatDisplayMode.EV => StatDisplayMode.Calculated,
                _ => StatDisplayMode.Calculated
            };
            UpdateStatsDisplay();
        }

        // =========================================================
        // CARREGAMENTO DOS DADOS (BANCO E TELA)
        // =========================================================
        private void LoadPokemonData(int id)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    _pokemon = db.RegisteredPokemons.FirstOrDefault(p => p.Id == id);
                    if (_pokemon == null) return;

                    _baseDex = db.PokedexEntries.FirstOrDefault(p => p.Name == _pokemon.Form);
                    _natureDb = db.Natures.FirstOrDefault(n => n.Name == _pokemon.Nature);

                    txtHeaderTitle.Text = $"{_pokemon.TrainerName.ToUpper()}'S POKÉMON";
                    txtDex.Text = _baseDex != null ? $"#{_baseDex.Dex}" : "#???";
                    txtLevel.Text = $"Lv. {_pokemon.Level}";
                    txtNickname.Text = !string.IsNullOrWhiteSpace(_pokemon.Nickname) ? _pokemon.Nickname : _pokemon.Form;
                    txtSpecies.Text = _pokemon.BasePokemon;
                    txtNature.Text = _pokemon.Nature;
                    txtAbility.Text = _pokemon.Ability;

                    txtShiny.Visibility = _pokemon.IsShiny ? Visibility.Visible : Visibility.Collapsed;

                    if (_baseDex != null)
                    {
                        string imgPath = _pokemon.IsShiny && !string.IsNullOrWhiteSpace(_baseDex.Shiny) ? _baseDex.Shiny : _baseDex.Image;
                        string appDir = AppDomain.CurrentDomain.BaseDirectory;
                        string absolutePath = System.IO.Path.Combine(appDir, imgPath);
                        if (System.IO.File.Exists(absolutePath)) imgPokemon.Source = new BitmapImage(new Uri(absolutePath));
                    }

                    txtMove1.Text = string.IsNullOrWhiteSpace(_pokemon.Move1) ? "-" : _pokemon.Move1;
                    txtMove2.Text = string.IsNullOrWhiteSpace(_pokemon.Move2) ? "-" : _pokemon.Move2;
                    txtMove3.Text = string.IsNullOrWhiteSpace(_pokemon.Move3) ? "-" : _pokemon.Move3;
                    txtMove4.Text = string.IsNullOrWhiteSpace(_pokemon.Move4) ? "-" : _pokemon.Move4;

                    if (_baseDex != null)
                    {
                        CalculateAllStats();
                        UpdateStatsDisplay();
                    }

                    UpdateNavigationButtons();
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro ao carregar dados: " + ex.Message); }
        }

        // =========================================================
        // MATEMÁTICA E LÓGICA DO GRÁFICO
        // =========================================================
        private void CalculateAllStats()
        {
            int lvl = _pokemon.Level;

            // Ordem garantida para o Gráfico: HP, Atk, Def, SpAtk, SpDef, Speed
            AllStats = new List<CalculatedStat>
            {
                new CalculatedStat("HP", _baseDex.Hp, _pokemon.IvHp, _pokemon.EvHp, CalculateHP(_baseDex.Hp, _pokemon.IvHp, _pokemon.EvHp, lvl), _natureDb),
                new CalculatedStat("Attack", _baseDex.Atk, _pokemon.IvAtk, _pokemon.EvAtk, CalculateStat(_baseDex.Atk, _pokemon.IvAtk, _pokemon.EvAtk, lvl, GetMultiplier("Attack", _natureDb)), _natureDb),
                new CalculatedStat("Defense", _baseDex.Def, _pokemon.IvDef, _pokemon.EvDef, CalculateStat(_baseDex.Def, _pokemon.IvDef, _pokemon.EvDef, lvl, GetMultiplier("Defense", _natureDb)), _natureDb),
                new CalculatedStat("Sp. Atk", _baseDex.Spa, _pokemon.IvSpa, _pokemon.EvSpa, CalculateStat(_baseDex.Spa, _pokemon.IvSpa, _pokemon.EvSpa, lvl, GetMultiplier("Sp. Atk", _natureDb)), _natureDb),
                new CalculatedStat("Sp. Def", _baseDex.Spd, _pokemon.IvSpd, _pokemon.EvSpd, CalculateStat(_baseDex.Spd, _pokemon.IvSpd, _pokemon.EvSpd, lvl, GetMultiplier("Sp. Def", _natureDb)), _natureDb),
                new CalculatedStat("Speed", _baseDex.Spe, _pokemon.IvSpe, _pokemon.EvSpe, CalculateStat(_baseDex.Spe, _pokemon.IvSpe, _pokemon.EvSpe, lvl, GetMultiplier("Speed", _natureDb)), _natureDb)
            };

            OnPropertyChanged(nameof(AllStats));
        }

        private void UpdateStatsDisplay()
        {
            if (AllStats == null) return;

            switch (_currentMode)
            {
                case StatDisplayMode.Calculated:
                    ChartTitle = "C A L C U L A T E D   S T A T S  (Click to Switch)";
                    ChartFillColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#455A64")) { Opacity = 0.7 };
                    ChartStrokeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#304FFE"));
                    break;
                case StatDisplayMode.IV:
                    ChartTitle = "I N D I V I D U A L   V A L U E S  (IVs)";
                    ChartFillColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#64DD17")) { Opacity = 0.7 };
                    ChartStrokeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00C853"));
                    break;
                case StatDisplayMode.EV:
                    ChartTitle = "E F F O R T   V A L U E S  (EVs)";
                    ChartFillColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD600")) { Opacity = 0.7 };
                    ChartStrokeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFAB00"));
                    break;
            }

            foreach (var stat in AllStats) { stat.UpdateDisplay(_currentMode); }
            CalculateHexagonPoints();
        }

        private void CalculateHexagonPoints()
        {
            if (AllStats == null || AllStats.Count < 6) return;

            double centerX = 140;
            double centerY = 140;
            double maxRadius = 100;

            int[] circularOrderIndices = { 0, 1, 2, 5, 4, 3 };
            double[] angles = { -90, -30, 30, 90, 150, 210 };

            double normalizationFactor = _currentMode switch
            {
                StatDisplayMode.IV => 31.0,
                StatDisplayMode.EV => 252.0,
                _ => 500.0
            };

            var points = new List<string>();

            for (int i = 0; i < 6; i++)
            {
                int statIndex = circularOrderIndices[i];
                double value = _currentMode switch
                {
                    StatDisplayMode.IV => AllStats[statIndex].IV,
                    StatDisplayMode.EV => AllStats[statIndex].EV,
                    _ => AllStats[statIndex].FinalStat
                };

                double normalizedValue = (value / normalizationFactor) * maxRadius;
                double radius = Math.Min(normalizedValue, maxRadius);
                if (radius < 2) radius = 2;

                double angleRad = angles[i] * Math.PI / 180.0;
                double x = centerX + radius * Math.Cos(angleRad);
                double y = centerY + radius * Math.Sin(angleRad);

                points.Add($"{x.ToString(System.Globalization.CultureInfo.InvariantCulture)},{y.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            }

            HexagonPointsString = string.Join(" ", points);
        }

        // =========================================================
        // FÓRMULAS OFICIAIS
        // =========================================================
        private int CalculateHP(int baseStat, int iv, int ev, int level)
        {
            if (baseStat == 1) return 1;
            double core = ((2 * baseStat + iv + (ev / 4.0)) * level) / 100.0;
            return (int)Math.Floor(core) + level + 10;
        }

        private int CalculateStat(int baseStat, int iv, int ev, int level, double natureMultiplier)
        {
            double core = ((2 * baseStat + iv + (ev / 4.0)) * level) / 100.0;
            int preNature = (int)Math.Floor(core) + 5;
            return (int)Math.Floor(preNature * natureMultiplier);
        }

        private double GetMultiplier(string statName, Nature nature)
        {
            if (nature == null) return 1.0;
            string inc = nature.Increase?.Trim().ToLower() ?? "";
            string dec = nature.Decrease?.Trim().ToLower() ?? "";
            string target = statName.Trim().ToLower();

            if (target == "sp. atk") target = "special attack";
            if (target == "sp. def") target = "special defense";

            if (inc.Contains(target) || (target == "special attack" && inc.Contains("sp. atk"))) return 1.1;
            if (dec.Contains(target) || (target == "special attack" && dec.Contains("sp. atk"))) return 0.9;

            return 1.0;
        }

        // =========================================================
        // IMPLEMENTAÇÃO INOTIFYPROPERTYCHANGED
        // =========================================================
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public enum StatDisplayMode { Calculated, IV, EV }

    public class CalculatedStat : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public int Base { get; private set; }
        public int IV { get; private set; }
        public int EV { get; private set; }
        public int FinalStat { get; private set; }

        private string _displayValue;
        public string DisplayValue { get => _displayValue; set { _displayValue = value; OnPropertyChanged(); } }

        private string _natureSymbol;
        public string NatureSymbol { get => _natureSymbol; set { _natureSymbol = value; OnPropertyChanged(); } }

        private Brush _symbolColor;
        public Brush SymbolColor { get => _symbolColor; set { _symbolColor = value; OnPropertyChanged(); } }

        private Nature _natureDb;

        public CalculatedStat(string name, int @base, int iv, int ev, int final, Nature natureDb)
        {
            Name = name; Base = @base; IV = iv; EV = ev; FinalStat = final; _natureDb = natureDb;
        }

        public void UpdateDisplay(StatDisplayMode mode)
        {
            if (mode == StatDisplayMode.Calculated) SetNatureSymbols();
            else { NatureSymbol = ""; SymbolColor = Brushes.Transparent; }

            DisplayValue = mode switch
            {
                StatDisplayMode.Calculated => FinalStat.ToString(),
                StatDisplayMode.IV => IV.ToString(),
                StatDisplayMode.EV => EV.ToString(),
                _ => ""
            };
        }

        private void SetNatureSymbols()
        {
            if (_natureDb == null || Name == "HP") { NatureSymbol = ""; return; }
            string target = Name.ToLower();

            if (target == "sp. atk") target = "special attack";
            if (target == "sp. def") target = "special defense";

            string inc = _natureDb.Increase?.ToLower() ?? "";
            string dec = _natureDb.Decrease?.ToLower() ?? "";

            if (inc.Contains(target) || (target == "special attack" && inc.Contains("sp. atk")))
            {
                NatureSymbol = "▲";
                SymbolColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#304FFE"));
            }
            else if (dec.Contains(target) || (target == "special attack" && dec.Contains("sp. atk")))
            {
                NatureSymbol = "▼";
                SymbolColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D50000"));
            }
            else { NatureSymbol = ""; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}