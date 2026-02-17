using Pokedex.data;
using Pokedex.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Pokedex.viewmodels
{
    public class PokemonRegisterViewModel : BaseViewModel
    {
        // ==========================================
        // LISTAS E COMBOBOXES
        // ==========================================
        public ObservableCollection<Trainer> Trainers { get; set; }
        public ObservableCollection<Nature> Natures { get; set; }

        private ObservableCollection<PokedexEntry> _filteredSpecies;
        public ObservableCollection<PokedexEntry> FilteredSpecies { get => _filteredSpecies; set { _filteredSpecies = value; OnPropertyChanged(); } }

        private ObservableCollection<PokedexEntry> _availableForms;
        public ObservableCollection<PokedexEntry> AvailableForms { get => _availableForms; set { _availableForms = value; OnPropertyChanged(); } }

        private ObservableCollection<string> _availableAbilities;
        public ObservableCollection<string> AvailableAbilities { get => _availableAbilities; set { _availableAbilities = value; OnPropertyChanged(); } }

        // ==========================================
        // LISTAS DE MOVES INTELIGENTES (Anti-Duplicação)
        // ==========================================
        private List<string> _baseTrainerMoves = new List<string>(); // Guarda todos os golpes permitidos para a Geração do Treinador

        private ObservableCollection<string> _availableMoves1;
        public ObservableCollection<string> AvailableMoves1 { get => _availableMoves1; set { _availableMoves1 = value; OnPropertyChanged(); } }

        private ObservableCollection<string> _availableMoves2;
        public ObservableCollection<string> AvailableMoves2 { get => _availableMoves2; set { _availableMoves2 = value; OnPropertyChanged(); } }

        private ObservableCollection<string> _availableMoves3;
        public ObservableCollection<string> AvailableMoves3 { get => _availableMoves3; set { _availableMoves3 = value; OnPropertyChanged(); } }

        private ObservableCollection<string> _availableMoves4;
        public ObservableCollection<string> AvailableMoves4 { get => _availableMoves4; set { _availableMoves4 = value; OnPropertyChanged(); } }

        // ==========================================
        // PROPRIEDADES DE SELEÇÃO
        // ==========================================
        private Trainer _selectedTrainer;
        public Trainer SelectedTrainer
        {
            get => _selectedTrainer;
            set { _selectedTrainer = value; OnPropertyChanged(); ApplyTrainerRules(); }
        }

        private PokedexEntry _selectedBasePokemon;
        public PokedexEntry SelectedBasePokemon
        {
            get => _selectedBasePokemon;
            set { _selectedBasePokemon = value; OnPropertyChanged(); FilterForms(); }
        }

        private PokedexEntry _selectedForm;
        public PokedexEntry SelectedForm
        {
            get => _selectedForm;
            set
            {
                _selectedForm = value;
                OnPropertyChanged();
                LoadBaseStats();
                LoadAbilities();
            }
        }

        private Nature _selectedNature;
        public Nature SelectedNature
        {
            get => _selectedNature;
            set { _selectedNature = value; OnPropertyChanged(); RecalculateAll(); }
        }

        // ==========================================
        // CAMPOS DE TEXTO E CONFIGURAÇÕES BÁSICAS
        // ==========================================
        private string _nickname, _ability;
        public string Nickname { get => _nickname; set { _nickname = value; OnPropertyChanged(); } }
        public string Ability { get => _ability; set { _ability = value; OnPropertyChanged(); } }

        private bool _isShiny;
        public bool IsShiny { get => _isShiny; set { _isShiny = value; OnPropertyChanged(); } }

        private int _level = 50;
        public int Level { get => _level; set { _level = value; OnPropertyChanged(); RecalculateAll(); } }

        // Se o valor de um move mudar, recalculamos as opções dos outros
        private string _move1, _move2, _move3, _move4;
        public string Move1
        {
            get => _move1;
            set
            {
                if (_move1 == value || _isUpdatingMoves) return; // Se for igual ou estiver atualizando, ignora!
                _move1 = value;
                OnPropertyChanged();
                UpdateMoveLists();
            }
        }

        public string Move2
        {
            get => _move2;
            set
            {
                if (_move2 == value || _isUpdatingMoves) return;
                _move2 = value;
                OnPropertyChanged();
                UpdateMoveLists();
            }
        }

        public string Move3
        {
            get => _move3;
            set
            {
                if (_move3 == value || _isUpdatingMoves) return;
                _move3 = value;
                OnPropertyChanged();
                UpdateMoveLists();
            }
        }

        public string Move4
        {
            get => _move4;
            set
            {
                if (_move4 == value || _isUpdatingMoves) return;
                _move4 = value;
                OnPropertyChanged();
                UpdateMoveLists();
            }
        }
        // ==========================================
        // STATS BASE E IVS
        // ==========================================
        private int bHp, bAtk, bDef, bSpa, bSpd, bSpe;

        private int _ivHp = 31, _ivAtk = 31, _ivDef = 31, _ivSpa = 31, _ivSpd = 31, _ivSpe = 31;
        public int IvHp { get => _ivHp; set { _ivHp = value; OnPropertyChanged(); RecalculateAll(); } }
        public int IvAtk { get => _ivAtk; set { _ivAtk = value; OnPropertyChanged(); RecalculateAll(); } }
        public int IvDef { get => _ivDef; set { _ivDef = value; OnPropertyChanged(); RecalculateAll(); } }
        public int IvSpa { get => _ivSpa; set { _ivSpa = value; OnPropertyChanged(); RecalculateAll(); } }
        public int IvSpd { get => _ivSpd; set { _ivSpd = value; OnPropertyChanged(); RecalculateAll(); } }
        public int IvSpe { get => _ivSpe; set { _ivSpe = value; OnPropertyChanged(); RecalculateAll(); } }

        // ==========================================
        // EVS COM BLOQUEIO INTELIGENTE (Max 510 total / 255 por stat)
        // ==========================================
        private int _evHp, _evAtk, _evDef, _evSpa, _evSpd, _evSpe;

        public int EvHp { get => _evHp; set { SetEv(ref _evHp, value, nameof(EvHp)); } }
        public int EvAtk { get => _evAtk; set { SetEv(ref _evAtk, value, nameof(EvAtk)); } }
        public int EvDef { get => _evDef; set { SetEv(ref _evDef, value, nameof(EvDef)); } }
        public int EvSpa { get => _evSpa; set { SetEv(ref _evSpa, value, nameof(EvSpa)); } }
        public int EvSpd { get => _evSpd; set { SetEv(ref _evSpd, value, nameof(EvSpd)); } }
        public int EvSpe { get => _evSpe; set { SetEv(ref _evSpe, value, nameof(EvSpe)); } }

        public int MaxEvHp => Math.Min(255, 510 - (_evAtk + _evDef + _evSpa + _evSpd + _evSpe));
        public int MaxEvAtk => Math.Min(255, 510 - (_evHp + _evDef + _evSpa + _evSpd + _evSpe));
        public int MaxEvDef => Math.Min(255, 510 - (_evHp + _evAtk + _evSpa + _evSpd + _evSpe));
        public int MaxEvSpa => Math.Min(255, 510 - (_evHp + _evAtk + _evDef + _evSpd + _evSpe));
        public int MaxEvSpd => Math.Min(255, 510 - (_evHp + _evAtk + _evDef + _evSpa + _evSpe));
        public int MaxEvSpe => Math.Min(255, 510 - (_evHp + _evAtk + _evDef + _evSpa + _evSpd));

        private void SetEv(ref int field, int value, string propertyName)
        {
            int currentTotalWithoutField = (_evHp + _evAtk + _evDef + _evSpa + _evSpd + _evSpe) - field;
            int remainingPoints = 510 - currentTotalWithoutField;

            int cappedValue = Math.Max(0, Math.Min(value, remainingPoints));
            cappedValue = Math.Min(cappedValue, 255);

            if (field != cappedValue)
            {
                field = cappedValue;
                OnPropertyChanged(propertyName);

                OnPropertyChanged(nameof(MaxEvHp));
                OnPropertyChanged(nameof(MaxEvAtk));
                OnPropertyChanged(nameof(MaxEvDef));
                OnPropertyChanged(nameof(MaxEvSpa));
                OnPropertyChanged(nameof(MaxEvSpd));
                OnPropertyChanged(nameof(MaxEvSpe));

                RecalculateAll();
            }
        }

        // ==========================================
        // STATS FINAIS (Para a UI)
        // ==========================================
        private int _fHp, _fAtk, _fDef, _fSpa, _fSpd, _fSpe;
        public int FinalHp { get => _fHp; set { _fHp = value; OnPropertyChanged(); } }
        public int FinalAtk { get => _fAtk; set { _fAtk = value; OnPropertyChanged(); } }
        public int FinalDef { get => _fDef; set { _fDef = value; OnPropertyChanged(); } }
        public int FinalSpa { get => _fSpa; set { _fSpa = value; OnPropertyChanged(); } }
        public int FinalSpd { get => _fSpd; set { _fSpd = value; OnPropertyChanged(); } }
        public int FinalSpe { get => _fSpe; set { _fSpe = value; OnPropertyChanged(); } }

        // ==========================================
        // CONSTRUTOR
        // ==========================================
        public PokemonRegisterViewModel()
        {
            using (var db = new AppDbContext())
            {
                Trainers = new ObservableCollection<Trainer>(db.Trainers.ToList());
                Natures = new ObservableCollection<Nature>(db.Natures.ToList());
            }
        }

        // ==========================================
        // REGRAS DE NEGÓCIO E FILTRAGEM
        // ==========================================
        private void ApplyTrainerRules()
        {
            if (SelectedTrainer == null) return;

            int trainerGen = RomanToInt(SelectedTrainer.Generation);
            string game = SelectedTrainer.Game;

            using (var db = new AppDbContext())
            {
                // Carrega todos os golpes possíveis uma única vez
                _baseTrainerMoves = db.Moves.AsEnumerable()
                                      .Where(m => RomanToInt(m.Gen) <= trainerGen)
                                      .Select(m => m.Name).OrderBy(n => n).ToList();

                int dexLimit = GetDexLimit(trainerGen, game);
                var species = db.PokedexEntries.AsEnumerable()
                                .Where(p => !p.Dex.Contains(".") && int.Parse(p.Dex) <= dexLimit)
                                .OrderBy(p => int.Parse(p.Dex)).ToList();
                FilteredSpecies = new ObservableCollection<PokedexEntry>(species);
            }

            // Reseta seleções
            SelectedBasePokemon = null;
            SelectedForm = null;
            Move1 = Move2 = Move3 = Move4 = null;

            // Constrói as 4 listas de golpes
            UpdateMoveLists();
        }

        private void FilterForms()
        {
            if (SelectedBasePokemon == null)
            {
                AvailableForms = new ObservableCollection<PokedexEntry>();
                return;
            }

            using (var db = new AppDbContext())
            {
                string baseDex = SelectedBasePokemon.Dex;
                var forms = db.PokedexEntries.AsEnumerable()
                              .Where(p => p.Dex == baseDex || p.Dex.StartsWith(baseDex + "."))
                              .ToList();
                AvailableForms = new ObservableCollection<PokedexEntry>(forms);
            }
            SelectedForm = AvailableForms.FirstOrDefault();
        }

        private void LoadAbilities()
        {
            AvailableAbilities = new ObservableCollection<string>();
            if (SelectedForm == null || SelectedForm.Abilities == null) return;

            // Varre a lista de habilidades do Pokémon e adiciona ao ComboBox
            foreach (var ab in SelectedForm.Abilities)
            {
                if (!string.IsNullOrWhiteSpace(ab))
                {
                    AvailableAbilities.Add(ab);
                }
            }

            Ability = AvailableAbilities.FirstOrDefault();
        }



        // Esta é a regra de Ouro Anti-Duplicação:
        // ==========================================
        // PROPRIEDADES DE MOVES COM TRAVA ANTI-LOOP
        // ==========================================
        private bool _isUpdatingMoves = false; // <-- NOSSO CADEADO DE SEGURANÇA



        // ==========================================
        // REGRA DE OURO ANTI-DUPLICAÇÃO
        // ==========================================
        private void UpdateMoveLists()
        {
            if (_baseTrainerMoves == null || !_baseTrainerMoves.Any()) return;

            // FECHA O CADEADO: Impede que as ComboBoxes disparem novos ciclos enquanto recriamos as listas
            _isUpdatingMoves = true;

            AvailableMoves1 = new ObservableCollection<string>(_baseTrainerMoves.Where(m => m == Move1 || (m != Move2 && m != Move3 && m != Move4)));
            AvailableMoves2 = new ObservableCollection<string>(_baseTrainerMoves.Where(m => m == Move2 || (m != Move1 && m != Move3 && m != Move4)));
            AvailableMoves3 = new ObservableCollection<string>(_baseTrainerMoves.Where(m => m == Move3 || (m != Move1 && m != Move2 && m != Move4)));
            AvailableMoves4 = new ObservableCollection<string>(_baseTrainerMoves.Where(m => m == Move4 || (m != Move1 && m != Move2 && m != Move3)));

            // ABRE O CADEADO: As listas já foram atualizadas com segurança
            _isUpdatingMoves = false;
        }

        private int GetDexLimit(int gen, string game)
        {
            return gen switch
            {
                1 => 151,
                2 => 251,
                3 => 386,
                4 => 493,
                5 => 649,
                6 => 721,
                7 => game.Contains("Ultra") ? 807 : (game.Contains("Let's Go") ? 809 : 802),
                8 => game.Contains("Arceus") ? 905 : 898,
                9 => 1025,
                _ => 1025
            };
        }

        private int RomanToInt(string roman)
        {
            return roman switch { "I" => 1, "II" => 2, "III" => 3, "IV" => 4, "V" => 5, "VI" => 6, "VII" => 7, "VIII" => 8, "IX" => 9, _ => 9 };
        }

        // ==========================================
        // CÁLCULO DE STATUS
        // ==========================================
        private void LoadBaseStats()
        {
            if (SelectedForm == null) return;

            bHp = SelectedForm.Hp; bAtk = SelectedForm.Atk; bDef = SelectedForm.Def;
            bSpa = SelectedForm.Spa; bSpd = SelectedForm.Spd; bSpe = SelectedForm.Spe;

            RecalculateAll();
        }

        private void RecalculateAll()
        {
            if (SelectedForm == null) return;

            FinalHp = (int)(Math.Floor(((2 * bHp + IvHp + (EvHp / 4.0)) * Level) / 100.0) + Level + 10);
            if (bHp == 1) FinalHp = 1;

            FinalAtk = CalcStat(bAtk, IvAtk, EvAtk, "Attack");
            FinalDef = CalcStat(bDef, IvDef, EvDef, "Defense");
            FinalSpa = CalcStat(bSpa, IvSpa, EvSpa, "Special Attack");
            FinalSpd = CalcStat(bSpd, IvSpd, EvSpd, "Special Defense");
            FinalSpe = CalcStat(bSpe, IvSpe, EvSpe, "Speed");
        }

        private int CalcStat(int @base, int iv, int ev, string statName)
        {
            double core = Math.Floor(((2 * @base + iv + (ev / 4.0)) * Level) / 100.0) + 5;
            double mult = 1.0;

            if (SelectedNature != null)
            {
                if (SelectedNature.Increase == statName) mult = 1.1;
                else if (SelectedNature.Decrease == statName) mult = 0.9;
            }

            return (int)Math.Floor(core * mult);
        }

        // ==========================================
        // GRAVAÇÃO NO BANCO
        // ==========================================
        public bool SavePokemon()
        {
            if (SelectedTrainer == null || SelectedForm == null) return false;

            try
            {
                using (var db = new AppDbContext())
                {
                    db.RegisteredPokemons.Add(new RegisteredPokemon
                    {
                        TrainerId = SelectedTrainer.Id,
                        TrainerName = SelectedTrainer.Name,
                        BasePokemon = SelectedBasePokemon.Name,
                        Form = SelectedForm.Name,
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
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar no banco: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}