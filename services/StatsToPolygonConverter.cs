using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Pokedex.services
{
    public class StatsToPolygonConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 6) return new PointCollection();

            try
            {
                // Pega os 6 status do Pokémon que virão do XAML
                double hp = System.Convert.ToDouble(values[0]);
                double atk = System.Convert.ToDouble(values[1]);
                double def = System.Convert.ToDouble(values[2]);
                double spa = System.Convert.ToDouble(values[3]);
                double spd = System.Convert.ToDouble(values[4]);
                double spe = System.Convert.ToDouble(values[5]);

                // Configurações Matemáticas do Gráfico
                double maxStat = 255.0; // 255 é o status máximo possível no jogo (HP da Blissey)
                double radius = 65.0;   // Tamanho visual do hexágono na tela
                double cx = 100.0;      // Centro X
                double cy = 100.0;      // Centro Y

                double cos30 = 0.8660254; // Cosseno de 30 graus
                double sin30 = 0.5;       // Seno de 30 graus

                PointCollection points = new PointCollection();

                // 1. HP (Topo) 
                points.Add(new Point(cx, cy - (hp / maxStat) * radius));

                // 2. Attack (Topo-Direita)
                points.Add(new Point(cx + (atk / maxStat) * radius * cos30, cy - (atk / maxStat) * radius * sin30));

                // 3. Defense (Baixo-Direita)
                points.Add(new Point(cx + (def / maxStat) * radius * cos30, cy + (def / maxStat) * radius * sin30));

                // 4. Speed (Baixo) 
                points.Add(new Point(cx, cy + (spe / maxStat) * radius));

                // 5. Sp. Defense (Baixo-Esquerda)
                points.Add(new Point(cx - (spd / maxStat) * radius * cos30, cy + (spd / maxStat) * radius * sin30));

                // 6. Sp. Attack (Topo-Esquerda)
                points.Add(new Point(cx - (spa / maxStat) * radius * cos30, cy - (spa / maxStat) * radius * sin30));

                return points;
            }
            catch
            {
                return new PointCollection();
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}