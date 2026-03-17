using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using Pokedex.models;

namespace Pokedex.services
{
    public class TypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var types = value as ICollection<PokemonType>;
            if (types == null || !types.Any())
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A8A878"));

            var typeList = types.OrderBy(t => t.Slot).ToList();
            var color1 = GetColorFromType(typeList[0].TypeName);

            if (typeList.Count == 1)
            {
                return new SolidColorBrush(color1);
            }
            var color2 = GetColorFromType(typeList[1].TypeName);

            return new LinearGradientBrush(color1, color2, new System.Windows.Point(0, 0), new System.Windows.Point(1, 1));
        }

        private Color GetColorFromType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return (Color)ColorConverter.ConvertFromString("#A8A878");

            string hex = typeName.ToLower() switch
            {
                "fire" => "#F08030",
                "water" => "#6890F0",
                "grass" => "#78C850",
                "electric" => "#F8D030",
                "ice" => "#98D8D8",
                "fighting" => "#C03028",
                "poison" => "#A040A0",
                "ground" => "#E0C068",
                "flying" => "#A890F0",
                "psychic" => "#F85888",
                "bug" => "#A8B820",
                "rock" => "#B8A038",
                "ghost" => "#705898",
                "dragon" => "#7038F8",
                "dark" => "#705848",
                "steel" => "#B8B8D0",
                "fairy" => "#EE99AC",
                "normal" => "#A8A878",
                _ => "#A8A878"
            };

            return (Color)ColorConverter.ConvertFromString(hex);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}