using System.Collections.ObjectModel;
using System.Windows;
using Pokedex.data;
using Pokedex.models;

namespace Pokedex.viewmodels
{
    public class TrainerRegisterViewModel : BaseViewModel
    {
        private string _name;
        private string _trainerId;
        private string _selectedRegion;
        private string _selectedGeneration;
        private string _selectedGame;

        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        public string TrainerId { get => _trainerId; set { _trainerId = value; OnPropertyChanged(); } }

        // Adicionado "Hisui" conforme sua lista
        public ObservableCollection<string> Regions { get; } = new ObservableCollection<string>
        { "Kanto", "Johto", "Hoenn", "Sinnoh", "Unova", "Kalos", "Alola", "Galar", "Hisui", "Paldea" };

        public ObservableCollection<string> Generations { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> Games { get; } = new ObservableCollection<string>();

        public string SelectedRegion
        {
            get => _selectedRegion;
            set
            {
                _selectedRegion = value;
                OnPropertyChanged();
                UpdateGenerations();
            }
        }

        public string SelectedGeneration
        {
            get => _selectedGeneration;
            set
            {
                _selectedGeneration = value;
                OnPropertyChanged();
                UpdateGames();
            }
        }

        public string SelectedGame { get => _selectedGame; set { _selectedGame = value; OnPropertyChanged(); } }

        private void UpdateGenerations()
        {
            Generations.Clear();
            SelectedGeneration = null;
            if (string.IsNullOrEmpty(SelectedRegion)) return;

            switch (SelectedRegion)
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

        private void AddGens(params string[] gens) { foreach (var g in gens) Generations.Add(g); }

        private void UpdateGames()
        {
            Games.Clear();
            SelectedGame = null;
            if (string.IsNullOrEmpty(SelectedGeneration) || string.IsNullOrEmpty(SelectedRegion)) return;

            // Lógica baseada na sua regra exata (Região + Geração)
            if (SelectedRegion == "Kanto")
            {
                if (SelectedGeneration == "I") AddGames("Red", "Green", "Blue", "Yellow");
                else if (SelectedGeneration == "III") AddGames("FireRed", "LeafGreen");
                else if (SelectedGeneration == "VII") AddGames("Let's Go Pikachu", "Let's Go Eevee");
            }
            else if (SelectedRegion == "Johto")
            {
                if (SelectedGeneration == "II") AddGames("Gold", "Silver", "Crystal");
                else if (SelectedGeneration == "IV") AddGames("HeartGold", "SoulSilver");
            }
            else if (SelectedRegion == "Hoenn")
            {
                if (SelectedGeneration == "III") AddGames("Ruby", "Sapphire", "Emerald");
                else if (SelectedGeneration == "VI") AddGames("Omega Ruby", "Alpha Sapphire");
            }
            else if (SelectedRegion == "Sinnoh")
            {
                if (SelectedGeneration == "IV") AddGames("Diamond", "Pearl", "Platinum");
                else if (SelectedGeneration == "VIII") AddGames("Brilliant Diamond", "Shining Pearl");
            }
            else if (SelectedRegion == "Unova" && SelectedGeneration == "V")
            {
                AddGames("Black", "White", "Black 2", "White 2");
            }
            else if (SelectedRegion == "Kalos")
            {
                if (SelectedGeneration == "VI") AddGames("X", "Y");
                else if (SelectedGeneration == "IX") AddGames("Legends Z-A");
            }
            else if (SelectedRegion == "Alola" && SelectedGeneration == "VII")
            {
                AddGames("Sun", "Moon", "Ultra Sun", "Ultra Moon");
            }
            else if (SelectedRegion == "Galar" && SelectedGeneration == "VIII")
            {
                AddGames("Sword", "Shield");
            }
            else if (SelectedRegion == "Hisui" && SelectedGeneration == "VIII")
            {
                AddGames("Legends Arceus");
            }
            else if (SelectedRegion == "Paldea" && SelectedGeneration == "IX")
            {
                AddGames("Scarlet", "Violet");
            }
        }

        private void AddGames(params string[] gs) { foreach (var g in gs) Games.Add(g); }

        public void Save()
        {
            // Adicionei a verificação de Geração também, por segurança
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrEmpty(SelectedGame) || string.IsNullOrEmpty(SelectedGeneration))
            {
                MessageBox.Show("Preencha o Nome, a Geração e escolha o Jogo!");
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    db.Trainers.Add(new Trainer
                    {
                        Name = Name,
                        TrainerId = TrainerId,
                        Region = SelectedRegion,
                        Generation = SelectedGeneration, // <-- ESTAVA FALTANDO ESTA LINHA!
                        Game = SelectedGame
                    });

                    db.SaveChanges();
                }
                MessageBox.Show("Treinador salvo com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                Clear();
            }
            catch (System.Exception ex)
            {
                // Adicionei um bloco try-catch para capturar exatamente se o banco de dados der erro na próxima vez
                MessageBox.Show($"Erro ao salvar no banco: {ex.Message}", "Erro de Banco de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Clear() { Name = ""; TrainerId = ""; SelectedRegion = null; }
    }
}