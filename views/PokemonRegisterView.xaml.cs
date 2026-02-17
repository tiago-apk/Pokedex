using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Pokedex.data;
using Pokedex.models;

namespace Pokedex.views
{
    public partial class PokemonRegisterView : UserControl
    {
        public PokemonRegisterView()
        {
            InitializeComponent();

            this.Loaded += PokemonRegisterView_Loaded;

            cmbTrainer.SelectionChanged += CmbTrainer_SelectionChanged;
            cmbPokemon.SelectionChanged += CmbPokemon_SelectionChanged;
            cmbForm.SelectionChanged += CmbForm_SelectionChanged;
        }

        private void PokemonRegisterView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var trainers = db.Trainers.ToList();
                    cmbTrainer.ItemsSource = trainers;
                    cmbTrainer.DisplayMemberPath = "Name";

                    var natures = db.Natures.Select(n => n.Name).OrderBy(n => n).ToList();
                    cmbNature.ItemsSource = natures;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar banco de dados: " + ex.Message);
            }
        }

        private void CmbTrainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTrainer.SelectedItem == null)
            {
                cmbPokemon.ItemsSource = null;
                cmbForm.ItemsSource = null;
                cmbMove1.ItemsSource = null; cmbMove2.ItemsSource = null;
                cmbMove3.ItemsSource = null; cmbMove4.ItemsSource = null;
                return;
            }

            var trainer = (Trainer)cmbTrainer.SelectedItem;
            int maxGen = ConvertRomanToGeneration(trainer.Generation);
            int maxDex = GetMaxDexLimit(trainer.Generation, trainer.Game);

            using (var db = new AppDbContext())
            {
                var allPokemon = db.PokedexEntries.ToList();

                var legalBasePokemon = allPokemon
                    .Where(p => ExtractDexNumber(p.Dex) <= maxDex && !p.Dex.Contains("."))
                    .ToList();

                cmbPokemon.ItemsSource = legalBasePokemon;
                cmbPokemon.DisplayMemberPath = "Name";

                var allMoves = db.Moves.ToList();
                var legalMoves = allMoves
                    .Where(m => ConvertRomanToGeneration(m.Gen) <= maxGen)
                    .Select(m => m.Name)
                    .OrderBy(name => name)
                    .ToList();

                cmbMove1.ItemsSource = legalMoves;
                cmbMove2.ItemsSource = legalMoves;
                cmbMove3.ItemsSource = legalMoves;
                cmbMove4.ItemsSource = legalMoves;
            }
        }

        private void CmbPokemon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbPokemon.SelectedItem == null)
            {
                cmbForm.ItemsSource = null;
                cmbAbility.ItemsSource = null;
                return;
            }

            var selectedBase = (PokedexEntry)cmbPokemon.SelectedItem;
            string baseNum = ExtractBaseDexString(selectedBase.Dex);

            using (var db = new AppDbContext())
            {
                var allPokemon = db.PokedexEntries.ToList();

                var allForms = allPokemon
                    .Where(p => ExtractBaseDexString(p.Dex) == baseNum)
                    .OrderBy(p => p.Id)
                    .ToList();

                cmbForm.ItemsSource = allForms;
                cmbForm.DisplayMemberPath = "Name";

                if (allForms.Count > 0)
                    cmbForm.SelectedIndex = 0;
            }
        }

        private void CmbForm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbForm.SelectedItem == null)
            {
                cmbAbility.ItemsSource = null;
                return;
            }

            var selectedForm = (PokedexEntry)cmbForm.SelectedItem;
            var validAbilities = new List<string>();

            if (selectedForm.Abilities != null)
            {
                for (int i = 0; i < selectedForm.Abilities.Count; i++)
                {
                    string ab = selectedForm.Abilities[i];
                    if (!string.IsNullOrWhiteSpace(ab))
                    {
                        if (i == 2)
                            validAbilities.Add($"{ab} (HA)");
                        else
                            validAbilities.Add(ab);
                    }
                }
            }

            cmbAbility.ItemsSource = validAbilities;
            if (validAbilities.Count > 0)
                cmbAbility.SelectedIndex = 0;
        }

        private string ExtractBaseDexString(string dexString)
        {
            if (string.IsNullOrWhiteSpace(dexString)) return "";
            return dexString.Replace("#", "").Trim().Split('.')[0].Split('_')[0].Split('-')[0];
        }

        private int ExtractDexNumber(string dexString)
        {
            string clean = ExtractBaseDexString(dexString);
            if (int.TryParse(clean, out int res)) return res;
            return 9999;
        }

        private int ConvertRomanToGeneration(string roman)
        {
            if (string.IsNullOrWhiteSpace(roman)) return 99;
            if (int.TryParse(roman, out int intGen)) return intGen;

            string upper = roman.Trim().ToUpper();
            switch (upper)
            {
                case "I": return 1;
                case "II": return 2;
                case "III": return 3;
                case "IV": return 4;
                case "V": return 5;
                case "VI": return 6;
                case "VII": return 7;
                case "VIII": return 8;
                case "IX": return 9;
                default: return 99;
            }
        }

        private int GetMaxDexLimit(string gen, string game)
        {
            if (gen == "I") return 151;
            if (gen == "II") return 251;
            if (gen == "III") return 386;
            if (gen == "IV") return 493;
            if (gen == "V") return 649;
            if (gen == "VI") return 721;

            if (gen == "VII")
            {
                if (game == "Sun" || game == "Moon") return 802;
                if (game == "UltraSun" || game == "UltraMoon") return 807;
                if (game == "Let's Go Pikachu" || game == "Let's Go Eevee") return 809;
                return 809;
            }

            if (gen == "VIII")
            {
                if (game == "Sword" || game == "Shield") return 898;
                if (game == "Legends Arceus") return 905;
                return 898;
            }

            if (gen == "IX") return 1025;

            return 1025;
        }

        // ================================================================
        // TRAVA DE SEGURANÇA DOS EVs (MÁXIMO 510 EM TEMPO REAL)
        // ================================================================
        private bool _isAdjustingEvs = false;

        private void EvSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isAdjustingEvs) return;

            // Os Sliders disparam esse evento ao carregar a tela, então prevenimos erros
            if (sldEvHp == null || sldEvAtk == null || sldEvDef == null ||
                sldEvSpa == null || sldEvSpd == null || sldEvSpe == null) return;

            // Calcula o total atual
            double total = sldEvHp.Value + sldEvAtk.Value + sldEvDef.Value +
                           sldEvSpa.Value + sldEvSpd.Value + sldEvSpe.Value;

            // Se o utilizador tentou passar de 510...
            if (total > 510)
            {
                _isAdjustingEvs = true; // Liga a trava

                Slider activeSlider = sender as Slider;
                if (activeSlider != null)
                {
                    double excess = total - 510;
                    // Empurra a barra de volta para o limite permitido de forma invisível e imediata
                    activeSlider.Value -= excess;
                }

                _isAdjustingEvs = false; // Desliga a trava
            }
        }

        // ================================================================
        // BOTÕES: VALIDAÇÃO DOS EVs E GUARDAR NO BANCO DE DADOS
        // ================================================================
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // 1. Validação de Campos Obrigatórios
            if (cmbTrainer.SelectedItem == null || cmbPokemon.SelectedItem == null || cmbForm.SelectedItem == null)
            {
                MessageBox.Show("Please select a Trainer, Pokémon and Form!", "Missing Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. VALIDAÇÃO COMPETITIVA: A SOMA DOS EVs NÃO PODE PASSAR DE 510
            double evTotal = sldEvHp.Value + sldEvAtk.Value + sldEvDef.Value +
                             sldEvSpa.Value + sldEvSpd.Value + sldEvSpe.Value;

            if (evTotal > 510)
            {
                MessageBox.Show($"The total EVs cannot exceed 510! Your current total is {evTotal}.\nPlease reduce some EVs.",
                                "Invalid EVs", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // 3. Preparar os dados para o Banco
                var selectedTrainer = (Trainer)cmbTrainer.SelectedItem;
                var basePoke = (PokedexEntry)cmbPokemon.SelectedItem;
                var formPoke = (PokedexEntry)cmbForm.SelectedItem;

                using (var db = new AppDbContext())
                {
                    var newPokemon = new RegisteredPokemon
                    {
                        TrainerId = selectedTrainer.Id,
                        TrainerName = selectedTrainer.Name,

                        BasePokemon = basePoke.Name,
                        Form = formPoke.Name,
                        Nickname = txtNickname.Text.Trim(),
                        IsShiny = tglShiny.IsChecked ?? false,
                        Level = (int)sldLevel.Value,
                        Nature = cmbNature.SelectedItem?.ToString() ?? "",
                        Ability = cmbAbility.SelectedItem?.ToString() ?? "",

                        Move1 = cmbMove1.SelectedItem?.ToString() ?? "",
                        Move2 = cmbMove2.SelectedItem?.ToString() ?? "",
                        Move3 = cmbMove3.SelectedItem?.ToString() ?? "",
                        Move4 = cmbMove4.SelectedItem?.ToString() ?? "",

                        IvHp = (int)sldIvHp.Value,
                        IvAtk = (int)sldIvAtk.Value,
                        IvDef = (int)sldIvDef.Value,
                        IvSpa = (int)sldIvSpa.Value,
                        IvSpd = (int)sldIvSpd.Value,
                        IvSpe = (int)sldIvSpe.Value,

                        EvHp = (int)sldEvHp.Value,
                        EvAtk = (int)sldEvAtk.Value,
                        EvDef = (int)sldEvDef.Value,
                        EvSpa = (int)sldEvSpa.Value,
                        EvSpd = (int)sldEvSpd.Value,
                        EvSpe = (int)sldEvSpe.Value
                    };

                    // 4. Salvar no Banco!
                    db.RegisteredPokemons.Add(newPokemon);
                    db.SaveChanges();
                }

                MessageBox.Show($"Pokémon '{formPoke.Name}' successfully registered for Trainer '{selectedTrainer.Name}'!",
                                "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Limpa o formulário após gravar
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving to database: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtNickname.Text = string.Empty;
            cmbTrainer.SelectedIndex = -1;
            cmbForm.SelectedIndex = -1;
            cmbAbility.SelectedIndex = -1;
            cmbNature.SelectedIndex = -1;

            tglShiny.IsChecked = false;

            sldLevel.Value = 50;

            // ATUALIZADO: IVs agora limpam para 0
            sldIvHp.Value = 0; sldIvAtk.Value = 0; sldIvDef.Value = 0;
            sldIvSpa.Value = 0; sldIvSpd.Value = 0; sldIvSpe.Value = 0;

            // EVs limpam para 0
            sldEvHp.Value = 0; sldEvAtk.Value = 0; sldEvDef.Value = 0;
            sldEvSpa.Value = 0; sldEvSpd.Value = 0; sldEvSpe.Value = 0;
        }
    }
}