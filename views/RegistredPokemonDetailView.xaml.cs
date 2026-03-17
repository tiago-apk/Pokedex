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
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private RegisteredPokemonDetailViewModel _viewModel;

        public RegisteredPokemonDetailView(int pokemonId, string trainerId)
        {
            InitializeComponent();
            _viewModel = new RegisteredPokemonDetailViewModel(pokemonId, trainerId);
            this.DataContext = _viewModel;
            UpdateNavButtons();
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }
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
        private void UpdateNavButtons()
        {
            btnPrev.Visibility = _viewModel.HasPrevious ? Visibility.Visible : Visibility.Hidden;
            btnNext.Visibility = _viewModel.HasNext ? Visibility.Visible : Visibility.Hidden;
        }
    }
}