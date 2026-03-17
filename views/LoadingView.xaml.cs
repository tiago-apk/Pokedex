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
                await Task.Run(() =>
                {
                    using (var db = new AppDbContext())
                    {
                        bool isDbAlive = db.Pokemons.Any();
                    }
                });

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