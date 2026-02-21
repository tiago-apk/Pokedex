using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Pokedex.viewmodels;

namespace Pokedex.views
{
    public partial class PokemonDetailView : Window
    {
        // =========================================================
        // MÁGICA DO WINDOWS API PARA ESCONDER O BOTÃO "X"
        // =========================================================
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000; // Representa os botões da barra (Ícone, Min, Max, Fechar)

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        // =========================================================

        public PokemonDetailView(models.PokedexEntry pokemon)
        {
            InitializeComponent();
            DataContext = new PokemonDetailViewModel(pokemon);

            // Adiciona um evento para rodar o truque assim que a janela carregar
            this.Loaded += PokemonDetailView_Loaded;
        }

        private void PokemonDetailView_Loaded(object sender, RoutedEventArgs e)
        {
            // Captura a janela atual e remove os botões de ação nativos da barra superior,
            // deixando a barra intacta apenas com o Título.
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // =========================================================
        // NAVEGAÇÃO DE POKÉMONS (Setinhas no topo ao lado do nome)
        // =========================================================
        private void PrevPoke_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PokemonDetailViewModel vm)
                vm.GoToPreviousPokemon();
        }

        private void NextPoke_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PokemonDetailViewModel vm)
                vm.GoToNextPokemon();
        }

        // =========================================================
        // NAVEGAÇÃO DE FORMAS (Setinhas na imagem para Megas, Alola, etc)
        // =========================================================
        private void PreviousForm_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PokemonDetailViewModel vm)
                vm.PreviousForm();
        }

        private void NextForm_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PokemonDetailViewModel vm)
                vm.NextForm();
        }

        // =========================================================
        // NAVEGAÇÃO DE EVOLUÇÕES (Imagens e Setas)
        // =========================================================

        private void PrevEvoPath_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PokemonDetailViewModel vm)
                vm.PreviousEvolutionPath();
        }

        private void NextEvoPath_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PokemonDetailViewModel vm)
                vm.NextEvolutionPath();
        }

        // =========================================================
        // CLIQUE NA IMAGEM DA LINHA EVOLUTIVA
        // =========================================================
        private void EvolutionNode_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.Tag is string targetDex)
            {
                if (DataContext is PokemonDetailViewModel vm)
                {
                    vm.NavigateToPokemonByDex(targetDex);
                }
            }
        }
    }
}