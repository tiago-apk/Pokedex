using Microsoft.EntityFrameworkCore;
using Pokedex.data;
using Pokedex.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Diagnostics;

namespace Pokedex.viewmodels
{
    public class MoveOption
    {
        public string Name { get; set; }
        public bool IsLearned { get; set; }
    }

    public class PokemonRegisterViewModel : BaseViewModel
    {
        // ==========================================
        // LISTAS (ITEMS SOURCE)
        // ==========================================
        public ObservableCollection<Trainer> Trainers { get; set; } = new ObservableCollection<Trainer>();
        public ObservableCollection<Nature> Natures { get; set; } = new ObservableCollection<Nature>();
        public ObservableCollection<Pokemon> FilteredSpecies { get; set; } = new ObservableCollection<Pokemon>();
        public ObservableCollection<Pokemon> AvailableForms { get; set; } = new ObservableCollection<Pokemon>();
        public ObservableCollection<string> AvailableAbilities { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<MoveOption> AvailableMoves1 { get; set; } = new ObservableCollection<MoveOption>();
        public ObservableCollection<MoveOption> AvailableMoves2 { get; set; } = new ObservableCollection<MoveOption>();
        public ObservableCollection<MoveOption> AvailableMoves3 { get; set; } = new ObservableCollection<MoveOption>();
        public ObservableCollection<MoveOption> AvailableMoves4 { get; set; } = new ObservableCollection<MoveOption>();

        // ==========================================
        // PROPRIEDADES SELECIONADAS (BINDINGS)
        // ==========================================
        private Trainer _selectedTrainer;
        public Trainer SelectedTrainer { get => _selectedTrainer; set { _selectedTrainer = value; OnPropertyChanged(); ApplyTrainerRules(); } }

        private Pokemon _selectedBasePokemon;
        public Pokemon SelectedBasePokemon { get => _selectedBasePokemon; set { _selectedBasePokemon = value; OnPropertyChanged(); UpdateForms(); } }

        private Pokemon _selectedForm;
        public Pokemon SelectedForm { get => _selectedForm; set { _selectedForm = value; OnPropertyChanged(); UpdateAbilitiesAndMoves(); } }

        private string _ability;
        public string Ability { get => _ability; set { _ability = value; OnPropertyChanged(); } }

        private Nature _selectedNature;
        public Nature SelectedNature { get => _selectedNature; set { _selectedNature = value; OnPropertyChanged(); } }

        // ==========================================
        // DADOS DO POKEMON
        // ==========================================
        private string _nickname; public string Nickname { get => _nickname; set { _nickname = value; OnPropertyChanged(); } }
        private int _level = 1; public int Level { get => _level; set { _level = value; OnPropertyChanged(); } }
        private bool _isShiny; public bool IsShiny { get => _isShiny; set { _isShiny = value; OnPropertyChanged(); } }

        public string Move1 { get; set; }
        public string Move2 { get; set; }
        public string Move3 { get; set; }
        public string Move4 { get; set; }

        // ==========================================
        // IVs (0 a 31)
        // ==========================================
        private int _ivHp; public int IvHp { get => _ivHp; set { _ivHp = value; OnPropertyChanged(); } }
        private int _ivAtk; public int IvAtk { get => _ivAtk; set { _ivAtk = value; OnPropertyChanged(); } }
        private int _ivDef; public int IvDef { get => _ivDef; set { _ivDef = value; OnPropertyChanged(); } }
        private int _ivSpa; public int IvSpa { get => _ivSpa; set { _ivSpa = value; OnPropertyChanged(); } }
        private int _ivSpd; public int IvSpd { get => _ivSpd; set { _ivSpd = value; OnPropertyChanged(); } }
        private int _ivSpe; public int IvSpe { get => _ivSpe; set { _ivSpe = value; OnPropertyChanged(); } }

        // ==========================================
        // LÓGICA DE EVs (MÁX 255 POR CAMPO, SOMA MÁX 510)
        // ==========================================
        private const int MAX_INDIVIDUAL_EV = 255;
        private const int MAX_TOTAL_EV = 510;

        private int _evHp; public int EvHp { get => _evHp; set { _evHp = EnforceEvLimits(value, _evHp); OnPropertyChanged(); RefreshMaxEvs(); } }
        private int _evAtk; public int EvAtk { get => _evAtk; set { _evAtk = EnforceEvLimits(value, _evAtk); OnPropertyChanged(); RefreshMaxEvs(); } }
        private int _evDef; public int EvDef { get => _evDef; set { _evDef = EnforceEvLimits(value, _evDef); OnPropertyChanged(); RefreshMaxEvs(); } }
        private int _evSpa; public int EvSpa { get => _evSpa; set { _evSpa = EnforceEvLimits(value, _evSpa); OnPropertyChanged(); RefreshMaxEvs(); } }
        private int _evSpd; public int EvSpd { get => _evSpd; set { _evSpd = EnforceEvLimits(value, _evSpd); OnPropertyChanged(); RefreshMaxEvs(); } }
        private int _evSpe; public int EvSpe { get => _evSpe; set { _evSpe = EnforceEvLimits(value, _evSpe); OnPropertyChanged(); RefreshMaxEvs(); } }

        // Propriedades dinâmicas para o MAXIMUM dos Sliders no XAML
        public int MaxEvHp => GetDynamicMax(_evHp);
        public int MaxEvAtk => GetDynamicMax(_evAtk);
        public int MaxEvDef => GetDynamicMax(_evDef);
        public int MaxEvSpa => GetDynamicMax(_evSpa);
        public int MaxEvSpd => GetDynamicMax(_evSpd);
        public int MaxEvSpe => GetDynamicMax(_evSpe);

        private int EnforceEvLimits(int newValue, int oldValue)
        {
            int currentTotal = _evHp + _evAtk + _evDef + _evSpa + _evSpd + _evSpe;
            int totalWithoutCurrent = currentTotal - oldValue;
            int remainingBudget = MAX_TOTAL_EV - totalWithoutCurrent;

            // O novo valor não pode passar do orçamento restante E nem de 255
            int allowedValue = Math.Min(newValue, remainingBudget);
            return Math.Clamp(allowedValue, 0, MAX_INDIVIDUAL_EV);
        }

        private int GetDynamicMax(int currentValue)
        {
            int totalUsed = _evHp + _evAtk + _evDef + _evSpa + _evSpd + _evSpe;
            int available = MAX_TOTAL_EV - totalUsed;
            return Math.Min(MAX_INDIVIDUAL_EV, currentValue + available);
        }

        private void RefreshMaxEvs()
        {
            OnPropertyChanged(nameof(MaxEvHp)); OnPropertyChanged(nameof(MaxEvAtk)); OnPropertyChanged(nameof(MaxEvDef));
            OnPropertyChanged(nameof(MaxEvSpa)); OnPropertyChanged(nameof(MaxEvSpd)); OnPropertyChanged(nameof(MaxEvSpe));
        }

        // ==========================================
        // COMANDOS E CARREGAMENTO
        // ==========================================
        public services.RelayCommand SaveCommand { get; }

        public PokemonRegisterViewModel()
        {
            SaveCommand = new services.RelayCommand(() => SavePokemon());
            LoadInitialData();
        }

        private void LoadInitialData()
        {
            using (var db = new AppDbContext())
            {
                Trainers.Clear();
                foreach (var t in db.Trainers.ToList()) Trainers.Add(t);
                Natures.Clear();
                foreach (var n in db.Natures.ToList()) Natures.Add(n);

                var allPokes = db.Pokemons.AsNoTracking().ToList();
                var baseSpecies = allPokes.GroupBy(p => p.NationalDex)
                                          .Select(g => g.OrderBy(x => x.Id).First())
                                          .OrderBy(p => p.NationalDex).ToList();

                FilteredSpecies.Clear();
                foreach (var s in baseSpecies) FilteredSpecies.Add(s);
            }
        }

        private void UpdateForms()
        {
            AvailableForms.Clear();
            if (SelectedBasePokemon == null) return;
            using (var db = new AppDbContext())
            {
                var forms = db.Pokemons.AsNoTracking().Where(p => p.NationalDex == SelectedBasePokemon.NationalDex).ToList();
                foreach (var f in forms) AvailableForms.Add(f);
            }
            if (AvailableForms.Count > 0) SelectedForm = AvailableForms[0];
        }

        private void UpdateAbilitiesAndMoves()
        {
            AvailableAbilities.Clear();
            if (SelectedForm == null) return;
            using (var db = new AppDbContext())
            {
                string pid = SelectedForm.Id;
                var abs = db.PokemonAbilities.AsNoTracking().Where(a => a.PokemonId == pid).Select(a => a.AbilityName).Distinct().ToList();
                foreach (var a in abs) AvailableAbilities.Add(a);
                if (AvailableAbilities.Count > 0) Ability = AvailableAbilities[0];
                ApplyTrainerRules();
            }
        }

        private void ApplyTrainerRules()
        {
            if (SelectedTrainer == null || SelectedForm == null) return;
            int trainerGen = RomanToInt(SelectedTrainer.Generation);
            using (var db = new AppDbContext())
            {
                string bid = SelectedForm.Id;
                if (bid.Contains("_Mega_") || bid.Contains("_Gigantamax_"))
                {
                    var pts = bid.Split('_');
                    if (pts.Length >= 2) bid = $"{pts[0]}_{pts[1]}";
                }
                var moves = db.PokemonMoves.AsNoTracking().Where(m => m.PokemonId == bid).ToList();
                var filtered = moves.Where(m => {
                    string c = m.Generation?.ToLower().Replace("gen", "").Replace("generation", "").Trim();
                    return RomanToInt(c) <= trainerGen;
                }).Select(m => m.MoveName).Distinct().OrderBy(n => n).ToList();

                AvailableMoves1.Clear(); AvailableMoves2.Clear(); AvailableMoves3.Clear(); AvailableMoves4.Clear();
                foreach (var moveName in filtered)
                {
                    var opt = new MoveOption { Name = moveName, IsLearned = true };
                    AvailableMoves1.Add(opt); AvailableMoves2.Add(opt); AvailableMoves3.Add(opt); AvailableMoves4.Add(opt);
                }
            }
        }

        public bool SavePokemon()
        {
            if (SelectedTrainer == null || SelectedForm == null || string.IsNullOrEmpty(Ability))
            {
                MessageBox.Show("Preencha Treinador, Pokémon e Habilidade!");
                return false;
            }
            try
            {
                using (var db = new AppDbContext())
                {
                    db.RegisteredPokemons.Add(new RegisteredPokemon
                    {
                        TrainerId = SelectedTrainer.TrainerId,
                        PokemonId = SelectedForm.Id,
                        Nickname = Nickname,
                        Level = Level,
                        Ability = Ability,
                        Nature = SelectedNature?.Name,
                        IsShiny = IsShiny,
                        IvHp = IvHp,
                        IvAtk = IvAtk,
                        IvDef = IvDef,
                        IvSpa = IvSpa,
                        IvSpd = IvSpd,
                        IvSpe = IvSpe,
                        EvHp = EvHp,
                        EvAtk = EvAtk,
                        EvDef = EvDef,
                        EvSpa = EvSpa,
                        EvSpd = EvSpd,
                        EvSpe = EvSpe,
                        Move1 = Move1,
                        Move2 = Move2,
                        Move3 = Move3,
                        Move4 = Move4
                    });
                    db.SaveChanges();
                }
                MessageBox.Show("Registado com sucesso!");
                return true;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return false; }
        }

        private int RomanToInt(string r)
        {
            if (string.IsNullOrEmpty(r)) return 9;
            string v = r.ToUpper().Replace("GENERATION", "").Replace("GEN", "").Trim();
            if (int.TryParse(v, out int res)) return res;
            return v switch { "I" => 1, "II" => 2, "III" => 3, "IV" => 4, "V" => 5, "VI" => 6, "VII" => 7, "VIII" => 8, "IX" => 9, _ => 9 };
        }
    }
}