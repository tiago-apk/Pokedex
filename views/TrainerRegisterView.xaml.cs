using Pokedex.viewmodels;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pokedex.views
{
    public partial class TrainerRegisterView : UserControl
    {
        // Referência privada para a nossa ViewModel
        private TrainerRegisterViewModel _viewModel;

        public TrainerRegisterView()
        {
            InitializeComponent();

            // Instanciamos a ViewModel
            _viewModel = new TrainerRegisterViewModel();

            // O DataContext é a chave do MVVM. 
            // Ele diz ao XAML: "Tudo o que você vir em {Binding ...}, procure nesta classe."
            this.DataContext = _viewModel;
        }

        /// <summary>
        /// O evento de clique agora apenas chama o método de salvar da ViewModel.
        /// Toda a validação e acesso ao banco de dados estão lá.
        /// </summary>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Save();
        }

        /// <summary>
        /// O botão cancelar apenas limpa as propriedades da ViewModel.
        /// Graças ao Binding, a tela limpa automaticamente.
        /// </summary>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Clear();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // A Regex verifica se o que foi digitado NÃO é um número (0 a 9)
            Regex regex = new Regex("[^0-9]+");
            // Se não for número, marcamos o evento como "Handled" (ou seja, bloqueamos a digitação)
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}