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
            if (values.Length < 6) return null;
            double[] stats = values.Select(v => System.Convert.ToDouble(v ?? 0)).ToArray();

            double centerX = 100;
            double centerY = 100;
            double maxRadius = 80;
            double maxValue = 255;

            PointCollection points = new PointCollection();

            for (int i = 0; i < 6; i++)
            {
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