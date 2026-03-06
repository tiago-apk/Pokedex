using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Pokedex.viewmodels;

namespace Pokedex.views
{
    public partial class RegisteredPokemonDetailView : Window
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

        // 🧠 Referência à nossa nova ViewModel
        private RegisteredPokemonDetailViewModel _viewModel;

        public RegisteredPokemonDetailView(int pokemonId, string trainerId)
        {
            InitializeComponent();

            // Instanciamos o "Cérebro" e passamos os IDs
            _viewModel = new RegisteredPokemonDetailViewModel(pokemonId, trainerId);

            // O DataContext liga o teu XAML à ViewModel (Isto é o coração do MVVM)
            this.DataContext = _viewModel;

            // Verifica logo no início se devemos mostrar as setinhas
            UpdateNavButtons();
        }

        // Continua a esconder a barra do Windows
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        // =========================================================
        // EVENTOS DE CLIQUE (A View só repassa a ordem)
        // =========================================================
        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.PreviousPokemon();
            UpdateNavButtons();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.NextPokemon();
            UpdateNavButtons();
        }

        private void Stats_Click(object sender, MouseButtonEventArgs e)
        {
            _viewModel.ToggleStatsMode();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Lógica puramente visual (Ocultar/Mostrar botões se houver ou não Pokémon na fila)
        private void UpdateNavButtons()
        {
            btnPrev.Visibility = _viewModel.HasPrevious ? Visibility.Visible : Visibility.Hidden;
            btnNext.Visibility = _viewModel.HasNext ? Visibility.Visible : Visibility.Hidden;
        }
    }
}