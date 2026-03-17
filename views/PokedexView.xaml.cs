using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using Pokedex.viewmodels;

namespace Pokedex.views
{
    public partial class PokedexView : UserControl
    {
        public PokedexView()
        {
            InitializeComponent();
            this.DataContext = new PokedexViewModel();
        }
    }
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