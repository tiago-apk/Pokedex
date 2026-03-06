using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace Pokedex.services
{
    public class StatsToPolygonConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Verifica se recebemos os 6 stats (HP, Atk, Def, Spa, Spd, Spe)
            if (values.Length < 6) return null;

            // Converte os valores para double, tratando nulos como 0
            double[] stats = values.Select(v => System.Convert.ToDouble(v ?? 0)).ToArray();

            // Define o centro e o raio do polígono (deve bater com o Canvas do XAML)
            double centerX = 100;
            double centerY = 100;
            double maxRadius = 80; // Ajuste conforme o tamanho do seu gráfico
            double maxValue = 255; // Valor máximo de um status Pokémon

            PointCollection points = new PointCollection();

            for (int i = 0; i < 6; i++)
            {
                // Calcula o ângulo para cada um dos 6 status (60 graus cada)
                double angle = (Math.PI / 180) * (i * 60 - 90);
                double radius = (stats[i] / maxValue) * maxRadius;

                double x = centerX + radius * Math.Cos(angle);
                double y = centerY + radius * Math.Sin(angle);

                points.Add(new System.Windows.Point(x, y));
            }

            return points;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => null;
    }
}