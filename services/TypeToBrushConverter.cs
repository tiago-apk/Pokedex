using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace Pokedex.services
{
    public class TypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 1. Obtém a lista e filtra strings vazias ou nulas (ex: ["Stellar", ""])
            var types = (value as List<string>)?
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToList();

            if (types == null || !types.Any())
                return new SolidColorBrush(Color.FromRgb(168, 168, 120)); // Cor Normal default

            // 2. Pega a cor do primeiro tipo
            Color color1 = GetColorFromType(types[0]);

            // 3. Se houver apenas um tipo válido, retorna cor sólida
            if (types.Count == 1)
            {
                return new SolidColorBrush(color1);
            }

            // 4. Se houver dois ou mais, cria um gradiente linear elegante
            Color color2 = GetColorFromType(types[1]);
            return new LinearGradientBrush(color1, color2, 45.0);
        }

        private Color GetColorFromType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return Color.FromRgb(168, 168, 120);

            // Switch expression com todas as cores oficiais da franquia
            return type.Trim().ToUpper() switch
            {
                "NORMAL" => Color.FromRgb(168, 168, 120),
                "FIRE" => Color.FromRgb(240, 128, 48),
                "WATER" => Color.FromRgb(104, 144, 240),
                "GRASS" => Color.FromRgb(120, 200, 80),
                "ELECTRIC" => Color.FromRgb(248, 208, 48),
                "ICE" => Color.FromRgb(152, 216, 216),
                "FIGHTING" => Color.FromRgb(192, 48, 40),
                "POISON" => Color.FromRgb(160, 64, 160),
                "GROUND" => Color.FromRgb(224, 192, 104),
                "FLYING" => Color.FromRgb(168, 144, 240),
                "PSYCHIC" => Color.FromRgb(248, 88, 136),
                "BUG" => Color.FromRgb(168, 184, 32),
                "ROCK" => Color.FromRgb(184, 160, 56),
                "GHOST" => Color.FromRgb(112, 88, 152),
                "DRAGON" => Color.FromRgb(112, 56, 248),
                "STEEL" => Color.FromRgb(184, 184, 208),
                "FAIRY" => Color.FromRgb(238, 153, 172),
                "DARK" => Color.FromRgb(112, 88, 72),
                "STELLAR" => Color.FromRgb(67, 165, 158),
                _ => Color.FromRgb(168, 168, 120) // Default para Normal
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}