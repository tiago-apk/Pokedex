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

        public ICommand ShowHomeCommand { get; }
        public ICommand ShowPokedexCommand { get; }
        public ICommand ShowRegisterPokemonCommand { get; }
        public ICommand ShowRegisterTrainerCommand { get; }
        public ICommand ShowTrainerListCommand { get; }

        public MainViewModel()
        {
            ShowHomeCommand = new RelayCommand(() => CurrentView = new HomeViewModel());
            ShowPokedexCommand = new RelayCommand(() => CurrentView = new PokedexViewModel());

            ShowRegisterPokemonCommand = new RelayCommand(() => CurrentView = new PokemonRegisterViewModel());
            ShowRegisterTrainerCommand = new RelayCommand(() => CurrentView = new TrainerRegisterViewModel());
            ShowTrainerListCommand = new RelayCommand(() => CurrentView = new TrainerListViewModel());

            CurrentView = new HomeViewModel();
        }
    }
}