using System;
using System.Collections.ObjectModel;
using System.Windows;
using Pokedex.data;
using Pokedex.models;
// Certifique-se de que tem o namespace do seu RelayCommand aqui. Ex: using Pokedex.services;

namespace Pokedex.viewmodels
{
    public class TrainerRegisterViewModel : BaseViewModel
    {
        private string _name;
        private string _trainerId;
        private string _selectedRegion;
        private string _selectedGeneration;
        private string _selectedGame;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string TrainerId
        {
            get => _trainerId;
            set { _trainerId = value; OnPropertyChanged(); }
        }

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


        public string SelectedGame
        {
            get => _selectedGame;
            set
            {
                _selectedGame = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Regions { get; } = new ObservableCollection<string>
        {
            "Kanto", "Johto", "Hoenn", "Sinnoh", "Unova",
            "Kalos", "Alola", "Galar", "Hisui", "Paldea"
        };

        public ObservableCollection<string> Generations { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> Games { get; } = new ObservableCollection<string>();

        private services.RelayCommand _saveCommand;
        public services.RelayCommand SaveCommand => _saveCommand ??= new services.RelayCommand(() => Save());

        private services.RelayCommand _clearCommand;
        public services.RelayCommand ClearCommand => _clearCommand ??= new services.RelayCommand(() => Clear());

        public TrainerRegisterViewModel()
        { }

        public void Save()
        {
            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(TrainerId) ||
                string.IsNullOrWhiteSpace(SelectedRegion) ||
                string.IsNullOrWhiteSpace(SelectedGeneration) ||
                string.IsNullOrWhiteSpace(SelectedGame))
            {
                MessageBox.Show("Por favor, preencha todos os campos obrigatórios (Nome, ID do Treinador, Região, Geração e Jogo)!",
                                "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    var novoTreinador = new Trainer
                    {
                        Name = Name.Trim(),
                        TrainerId = TrainerId.Trim(),
                        Region = SelectedRegion,
                        Generation = SelectedGeneration,
                        Game = SelectedGame
                    };

                    db.Trainers.Add(novoTreinador);
                    db.SaveChanges();
                }
                MessageBox.Show("Treinador salvo com sucesso no banco de dados!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar no banco:\n{ex.Message}\n\nDetalhes:\n{ex.InnerException?.Message}",
                                "Erro Grave", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Clear()
        {
            Name = string.Empty;
            TrainerId = string.Empty;
            SelectedRegion = null;
            SelectedGeneration = null;
            SelectedGame = null;
        }

        private void UpdateGenerations()
        {
            Generations.Clear();
            Games.Clear();
            SelectedGeneration = null;
            SelectedGame = null;

            if (string.IsNullOrEmpty(SelectedRegion)) return;

            switch (SelectedRegion)
            {
                case "Kanto":
                    Generations.Add("Generation I"); Generations.Add("Generation III"); Generations.Add("Generation VII");
                    break;
                case "Johto":
                    Generations.Add("Generation II"); Generations.Add("Generation IV");
                    break;
                case "Hoenn":
                    Generations.Add("Generation III"); Generations.Add("Generation VI");
                    break;
                case "Sinnoh":
                    Generations.Add("Generation IV"); Generations.Add("Generation VIII");
                    break;
                case "Unova":
                    Generations.Add("Generation V");
                    break;
                case "Kalos":
                    Generations.Add("Generation VI");
                    break;
                case "Alola":
                    Generations.Add("Generation VII");
                    break;
                case "Galar":
                case "Hisui":
                    Generations.Add("Generation VIII");
                    break;
                case "Paldea":
                    Generations.Add("Generation IX");
                    break;
            }
        }

        private void UpdateGames()
        {
            Games.Clear();
            SelectedGame = null;

            if (string.IsNullOrEmpty(SelectedGeneration)) return;

            switch (SelectedGeneration)
            {
                case "Generation I":
                    Games.Add("Red"); Games.Add("Blue"); Games.Add("Yellow"); break;
                case "Generation II":
                    Games.Add("Gold"); Games.Add("Silver"); Games.Add("Crystal"); break;
                case "Generation III":
                    Games.Add("Ruby"); Games.Add("Sapphire"); Games.Add("Emerald"); Games.Add("FireRed"); Games.Add("LeafGreen"); break;
                case "Generation IV":
                    Games.Add("Diamond"); Games.Add("Pearl"); Games.Add("Platinum"); Games.Add("HeartGold"); Games.Add("SoulSilver"); break;
                case "Generation V":
                    Games.Add("Black"); Games.Add("White"); Games.Add("Black 2"); Games.Add("White 2"); break;
                case "Generation VI":
                    Games.Add("X"); Games.Add("Y"); Games.Add("Omega Ruby"); Games.Add("Alpha Sapphire"); break;
                case "Generation VII":
                    Games.Add("Sun"); Games.Add("Moon"); Games.Add("Ultra Sun"); Games.Add("Ultra Moon"); Games.Add("Let's Go Pikachu"); Games.Add("Let's Go Eevee"); break;
                case "Generation VIII":
                    Games.Add("Sword"); Games.Add("Shield"); Games.Add("Brilliant Diamond"); Games.Add("Shining Pearl"); Games.Add("Legends: Arceus"); break;
                case "Generation IX":
                    Games.Add("Scarlet"); Games.Add("Violet"); break;
            }
        }
    }
}