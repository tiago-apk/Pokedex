using System;
using System.Globalization;
using System.Windows.Data;

namespace Pokedex.services // <--- ADICIONA ESTA LINHA
{
    public class IntToVisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int numero)
            {
                return numero > 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
            return System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
} // <--- FECHA A CHAVE AQUI