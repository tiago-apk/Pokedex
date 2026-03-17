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
            string cleanPath = relativePath.Replace("\\", "/").TrimStart('/');

            string fileName = Path.GetFileName(cleanPath);
            if (!fileName.StartsWith("#"))
            {
                string dir = Path.GetDirectoryName(cleanPath).Replace("\\", "/");
                cleanPath = $"{dir}/#{fileName}";
            }

            string diskPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, cleanPath.Replace("/", "\\"));
            if (File.Exists(diskPath))
            {
                return diskPath;
            }

            try
            {
                string escapedPath = cleanPath.Replace("#", "%23");
                return new Uri($"pack://application:,,,/{escapedPath}", UriKind.Absolute);
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}