using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Pokedex.viewmodels;

namespace Pokedex
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // VINCULA O VIEWMODEL À JANELA
            this.DataContext = new MainViewModel();
        }

        // Fecha a aplicação completamente
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Permite arrastar a janela clicando no fundo (já que removemos a barra do topo)
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}