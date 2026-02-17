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
                "NORMAL" => (Color)ColorConverter.ConvertFromString("#9fa19f"),
                "FIRE" => (Color)ColorConverter.ConvertFromString("#e62829"), // Laranja vibrante
                "WATER" => (Color)ColorConverter.ConvertFromString("#2980ef"), // Azul vivo
                "GRASS" => (Color)ColorConverter.ConvertFromString("#3fa129"), // Verde folha limpo
                "ELECTRIC" => (Color)ColorConverter.ConvertFromString("#fac000"), // Amarelo elétrico
                "ICE" => (Color)ColorConverter.ConvertFromString("#3fd8ff"), // Ciano gelado
                "FIGHTING" => (Color)ColorConverter.ConvertFromString("#ff8000"), // Vermelho marcial moderno
                "POISON" => (Color)ColorConverter.ConvertFromString("#9141cb"), // Roxo tóxico
                "GROUND" => (Color)ColorConverter.ConvertFromString("#915121"), // Terra quente
                "FLYING" => (Color)ColorConverter.ConvertFromString("#81b9ef"), // Azul céu pastel
                "PSYCHIC" => (Color)ColorConverter.ConvertFromString("#ef4179"), // Rosa psíquico
                "BUG" => (Color)ColorConverter.ConvertFromString("#91a119"), // Verde inseto neon
                "ROCK" => (Color)ColorConverter.ConvertFromString("#afa981"), // Areia escuro
                "GHOST" => (Color)ColorConverter.ConvertFromString("#704170"), // Índigo sombrio
                "DRAGON" => (Color)ColorConverter.ConvertFromString("#5060e1"), // Azul dragão profundo
                "STEEL" => (Color)ColorConverter.ConvertFromString("#60a1b8"), // Prata/Azul metálico
                "FAIRY" => (Color)ColorConverter.ConvertFromString("#ef70ef"), // Rosa fada brilhante
                "DARK" => (Color)ColorConverter.ConvertFromString("#50413f"), // Cinza chumbo escuro
                "STELLAR" => (Color)ColorConverter.ConvertFromString("#83cfc5"), // Ciano cristal brilhante
                _ => (Color)ColorConverter.ConvertFromString("#9fa19f")  // Default
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}