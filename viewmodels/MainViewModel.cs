using System.Windows.Input;
using Pokedex.models;
using Pokedex.services;


namespace Pokedex.viewmodels
{
    public class MainViewModel : BaseViewModel
    {
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        // Comandos de navegação (ICommand aceita RelayCommand)
        public ICommand ShowPokedexCommand { get; }
        public ICommand ShowRegisterPokemonCommand { get; }
        public ICommand ShowRegisterTrainerCommand { get; }
        public ICommand ShowTrainerListCommand { get; }

        public MainViewModel()
        {
            // Atribuição usando a versão não-genérica do RelayCommand
            ShowPokedexCommand = new RelayCommand(() => CurrentView = new PokedexViewModel());
            ShowRegisterPokemonCommand = new RelayCommand(() => CurrentView = new RegisterPokemonViewModel());
            ShowRegisterTrainerCommand = new RelayCommand(() => CurrentView = new TrainerRegisterViewModel());
            ShowTrainerListCommand = new RelayCommand(() => CurrentView = new TrainerListViewModel());

            // Tela inicial
            CurrentView = new PokedexViewModel();
        }
    }
}