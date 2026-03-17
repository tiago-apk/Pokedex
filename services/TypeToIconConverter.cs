using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Pokedex.services
{
    public class TypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string typeName = value as string;
            if (string.IsNullOrWhiteSpace(typeName)) return null;

            string iconRelativePath = Path.Combine("assets", "type_images", "icon", $"{typeName.ToUpper()}.png");
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, iconRelativePath);

            return File.Exists(fullPath) ? fullPath : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}