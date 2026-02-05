using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Pokedex.services
{
    public class TypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var types = value as List<string>;
            if (types == null || types.Count == 0) return Brushes.Teal;

            // Pega a cor do tipo 1
            Color color1 = GetColorFromType(types[0]);

            // Se tiver apenas 1 tipo, retorna cor sólida
            if (types.Count == 1)
            {
                return new SolidColorBrush(color1);
            }

            // Se tiver 2 tipos, cria um gradiente
            Color color2 = GetColorFromType(types[1]);
            return new LinearGradientBrush(color1, color2, 45.0); // Gradiente a 45 graus
        }

        private Color GetColorFromType(string type)
        {
            return (type.ToUpper()) switch
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
                "DARK" => Color.FromRgb(112, 88, 72),
                "DRAGON" => Color.FromRgb(112, 56, 248),
                "STEEL" => Color.FromRgb(184, 184, 208),
                "FAIRY" => Color.FromRgb(238, 153, 172),
                _ => Colors.Teal
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}