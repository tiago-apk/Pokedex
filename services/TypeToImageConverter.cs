using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Pokedex.services
{
    public class TypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string typeName = value as string;
            if (string.IsNullOrWhiteSpace(typeName)) return null;

            // Transforma "Fire" em "FIRE" para bater exatamente com o nome do seu arquivo
            string fileName = $"{typeName.ToUpper()}.png";

            // Aponta para a pasta 'names' em vez de 'icon'
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "type_images", "names", fileName);

            if (File.Exists(imagePath))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Evita travar o arquivo
                bitmap.EndInit();
                bitmap.Freeze(); // Melhora a performance
                return bitmap;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}