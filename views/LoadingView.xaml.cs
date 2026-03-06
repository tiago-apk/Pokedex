using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Pokedex.data;

namespace Pokedex.views
{
    public partial class LoadingView : Window
    {
        public LoadingView()
        {
            InitializeComponent();
            StartInitialization();
        }

        private async void StartInitialization()
        {
            try
            {
                // Task.Run para aquecer o banco sem travar a animação da interface
                await Task.Run(() =>
                {
                    using (var db = new AppDbContext())
                    {
                        // "Aquece" o Entity Framework fazendo a primeira consulta (MUITO mais rápido que o Seeder antigo)
                        bool isDbAlive = db.Pokemons.Any();
                    }
                });

                // Baixei a pausa de 5000 (5 segundos) para 1500 (1.5 segundos)
                // Isto é apenas para o utilizador ver a tua animação bonita da Pokébola antes de abrir a app!
                await Task.Delay(1500);

                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database connection error: {ex.Message}\nCheck if pokedex.db is in the correct folder.", "Loading Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }
    }
}