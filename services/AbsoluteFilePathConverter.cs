using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Pokedex.services
{
    public class AbsoluteFilePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string relativePath = value as string;
            if (string.IsNullOrWhiteSpace(relativePath)) return null;

            // 1. Corrige as barras (o JSON usa '/' mas o Windows prefere '\')
            string cleanPath = relativePath.Replace("/", "\\").TrimStart('\\');

            // 2. Monta o caminho completo baseado em onde o seu .exe está rodando
            // Isso aponta para Pokedex\bin\Debug\net8.0-windows\assets\...
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, cleanPath);

            // 3. Verificação de segurança: se o arquivo não existir, não quebra o app
            if (File.Exists(fullPath))
            {
                return fullPath;
            }

            // Opcional: Se não achar, você pode retornar um caminho para uma imagem de "erro"
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}