using System;
using System.Threading.Tasks;
using System.Windows;
using Pokedex.data;
using Pokedex.services;

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
                // Task.Run para processar o banco sem travar a interface
                await Task.Run(() =>
                {
                    using (var db = new AppDbContext())
                    {
                        DatabaseSeeder.Initialize(db);
                    }
                });

                await Task.Delay(5000); // Pausa breve para feedback visual

                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados: {ex.Message}");
                this.Close();
            }
        }
    }
}