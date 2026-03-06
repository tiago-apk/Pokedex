using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using Pokedex.viewmodels;

namespace Pokedex.views
{
    /// <summary>
    /// Lógica de interação para PokedexView.xaml
    /// </summary>
    public partial class PokedexView : UserControl
    {
        public PokedexView()
        {
            InitializeComponent();

            // Liga a View à ViewModel refatorada para usar o banco de dados
            this.DataContext = new PokedexViewModel();
        }
    }

    // Conversor genérico que podes usar em toda a aplicação se precisares de esconder/mostrar elementos
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string) ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}