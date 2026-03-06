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

            // 1. Limpa as barras do caminho para o padrão Windows
            string cleanPath = relativePath.Replace("\\", "/").TrimStart('/');

            // 2. MAGIA AQUI: Verifica se falta o '#' no nome do ficheiro e adiciona-o!
            string fileName = Path.GetFileName(cleanPath);
            if (!fileName.StartsWith("#"))
            {
                string dir = Path.GetDirectoryName(cleanPath).Replace("\\", "/");
                cleanPath = $"{dir}/#{fileName}";
                // Exemplo: "assets/.../normal/0001.png" vira "assets/.../normal/#0001.png"
            }

            // 3. Tenta encontrar a imagem fisicamente no disco (Pasta bin/Debug)
            string diskPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, cleanPath.Replace("/", "\\"));
            if (File.Exists(diskPath))
            {
                return diskPath;
            }

            // 4. Se não estiver no disco, tenta carregar como Recurso Embutido
            // ATENÇÃO: O '#' estraga os URIs do WPF, por isso temos de o transformar em "%23"
            try
            {
                string escapedPath = cleanPath.Replace("#", "%23");
                return new Uri($"pack://application:,,,/{escapedPath}", UriKind.Absolute);
            }
            catch
            {
                return null; // Se falhar tudo, devolve null e não crasha a app
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}