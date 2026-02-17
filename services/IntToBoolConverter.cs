using System;
using System.Globalization;
using System.Windows.Data;

namespace Pokedex.services // Certifique-se que o namespace condiz com sua pasta de serviços/conversores
{
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Se o contador da lista for maior que 0, retorna TRUE (habilita o campo)
            if (value is int count)
            {
                return count > 0;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}