using System;
using System.Windows;
using System.Windows.Controls;

namespace Pokedex.views
{
    public partial class TrainerRegisterView : UserControl
    {
        public TrainerRegisterView()
        {
            InitializeComponent();
        }

        // ==========================================
        // LÓGICA 1: QUANDO ESCOLHER A REGIÃO
        // ==========================================
        private void cmbRegion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Sempre que trocar a região, limpa e desativa os debaixo
            cmbGeneration.Items.Clear();
            cmbGame.Items.Clear();
            cmbGame.IsEnabled = false;

            if (cmbRegion.SelectedItem is ComboBoxItem selectedRegion)
            {
                cmbGeneration.IsEnabled = true; // Libera a Geração
                string region = selectedRegion.Content.ToString();

                // Preenche as Gerações dependendo da Região escolhida
                switch (region)
                {
                    case "Kanto": AddGens("I", "III", "VII"); break;
                    case "Johto": AddGens("II", "IV"); break;
                    case "Hoenn": AddGens("III", "VI"); break;
                    case "Sinnoh": AddGens("IV", "VIII"); break;
                    case "Unova": AddGens("V"); break;
                    case "Kalos": AddGens("VI", "IX"); break;
                    case "Alola": AddGens("VII"); break;
                    case "Galar": AddGens("VIII"); break;
                    case "Hisui": AddGens("VIII"); break;
                    case "Paldea": AddGens("IX"); break;
                }
            }
            else
            {
                cmbGeneration.IsEnabled = false;
            }
        }

        // ==========================================
        // LÓGICA 2: QUANDO ESCOLHER A GERAÇÃO
        // ==========================================
        private void cmbGeneration_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbGame.Items.Clear();

            // Só continua se tiver uma Região E uma Geração escolhidas
            if (cmbGeneration.SelectedItem is ComboBoxItem selectedGen &&
                cmbRegion.SelectedItem is ComboBoxItem selectedRegion)
            {
                cmbGame.IsEnabled = true; // Libera os Jogos
                string region = selectedRegion.Content.ToString();
                string gen = selectedGen.Content.ToString();

                // Regras cruzadas (Região + Geração = Jogo)
                if (region == "Kanto" && gen == "I") AddGames("Red", "Green", "Blue", "Yellow");
                else if (region == "Kanto" && gen == "III") AddGames("FireRed", "LeafGreen");
                else if (region == "Kanto" && gen == "VII") AddGames("Let's Go Pikachu", "Let's Go Eevee");

                else if (region == "Johto" && gen == "II") AddGames("Gold", "Silver", "Crystal");
                else if (region == "Johto" && gen == "IV") AddGames("HeartGold", "SoulSilver");

                else if (region == "Hoenn" && gen == "III") AddGames("Ruby", "Sapphire", "Emerald");
                else if (region == "Hoenn" && gen == "VI") AddGames("OmegaRuby", "AlphaSapphire");

                else if (region == "Sinnoh" && gen == "IV") AddGames("Diamond", "Pearl", "Platinum");
                else if (region == "Sinnoh" && gen == "VIII") AddGames("BrilliantDiamond", "ShiningPearl");

                else if (region == "Unova" && gen == "V") AddGames("Black", "White", "Black 2", "White 2");

                else if (region == "Kalos" && gen == "VI") AddGames("X", "Y");
                else if (region == "Kalos" && gen == "IX") AddGames("Legends ZA");

                else if (region == "Alola" && gen == "VII") AddGames("Sun", "Moon", "UltraSun", "UltraMoon");

                else if (region == "Galar" && gen == "VIII") AddGames("Sword", "Shield");

                else if (region == "Hisui" && gen == "VIII") AddGames("Legends Arceus");

                else if (region == "Paldea" && gen == "IX") AddGames("Scarlet", "Violet");
            }
            else
            {
                cmbGame.IsEnabled = false;
            }
        }

        // ==========================================
        // FUNÇÕES AUXILIARES PARA ADICIONAR OS ITENS
        // ==========================================
        private void AddGens(params string[] gens)
        {
            foreach (var g in gens)
                cmbGeneration.Items.Add(new ComboBoxItem { Content = g });
        }

        private void AddGames(params string[] games)
        {
            foreach (var g in games)
                cmbGame.Items.Add(new ComboBoxItem { Content = g });
        }

        // ==========================================
        // BOTÕES SAVE E CANCEL
        // ==========================================
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // 1. Validação de Segurança
            if (string.IsNullOrWhiteSpace(txtName.Text) || cmbGame.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill all the required fields (Name, Region, Generation and Game)!",
                                "Missing Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // 2. Abre a conexão com o Banco de Dados
                using (var db = new Pokedex.data.AppDbContext())
                {
                    // 3. Monta o Treinador com os dados da tela
                    var newTrainer = new Pokedex.models.Trainer
                    {
                        Name = txtName.Text.Trim(),
                        TrainerId = txtTrainerId.Text.Trim(), // Pode ser vazio, não há problema
                        Region = ((ComboBoxItem)cmbRegion.SelectedItem).Content.ToString(),
                        Generation = ((ComboBoxItem)cmbGeneration.SelectedItem).Content.ToString(),
                        Game = ((ComboBoxItem)cmbGame.SelectedItem).Content.ToString()
                    };

                    // 4. Adiciona ao banco e Salva!
                    db.Trainers.Add(newTrainer);
                    db.SaveChanges();
                }

                // 5. Sucesso e Limpeza
                MessageBox.Show($"Trainer {txtName.Text.Trim()} registered successfully!",
                                "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
            }
            catch (Exception ex)
            {
                // Caso algo dê errado (ex: banco bloqueado), ele avisa sem fechar o app
                MessageBox.Show($"Error saving to database: {ex.Message}",
                                "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            txtName.Text = string.Empty;
            txtTrainerId.Text = string.Empty;

            // Ao desmarcar a Região, a cascata fecha as caixas seguintes automaticamente
            cmbRegion.SelectedIndex = -1;
        }
    }
}