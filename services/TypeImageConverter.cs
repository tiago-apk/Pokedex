using System;
using System.Globalization;
using System.Windows.Data;

namespace Pokedex.services
{
    public class TypeImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string typeName = value as string;
            if (string.IsNullOrEmpty(typeName)) return null;
            return $"/assets/type_images/names/{typeName.ToUpper()}.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}