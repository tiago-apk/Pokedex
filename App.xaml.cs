using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Media;

namespace Pokedex
{
    public partial class App : Application
    {
        private bool _isDarkMode = false;

        public void ToggleTheme()
        {
            _isDarkMode = !_isDarkMode;

            // Se for Dark Mode, aplicamos cores escuras
            if (_isDarkMode)
            {
                // Fundos
                Current.Resources["AppBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#121212")); // Fundo super escuro
                Current.Resources["ColorSurface"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E")); // Painéis
                Current.Resources["ColorSurfaceVariant"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2C2C2E")); // Paineis secundários
                Current.Resources["ColorHome"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5E5EA"));
                Current.Resources["ColorHomeText"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#212121"));

                // Textos e Linhas
                Current.Resources["TextColorPrimary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0")); // Texto Branco/Cinza
                Current.Resources["TextColorSecondary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A0A0A0")); // Texto Cinza Claro
                Current.Resources["ColorBorder"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#424242")); // Bordas escuras
                Current.Resources["SliderBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#424242")); // Fundo do Slider escuro
            }
            // Se for Light Mode, voltamos às originais
            else
            {
                Current.Resources["AppBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FAFAFA"));
                Current.Resources["ColorSurface"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
                Current.Resources["ColorSurfaceVariant"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F2F2F7"));
                Current.Resources["ColorHome"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#212121"));
                Current.Resources["ColorHomeText"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));

                Current.Resources["TextColorPrimary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2C2C2E"));
                Current.Resources["TextColorSecondary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888"));
                Current.Resources["ColorBorder"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5E5EA"));
                Current.Resources["SliderBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5E5EA"));
            }
        }
    }
}