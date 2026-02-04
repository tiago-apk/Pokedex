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
                // Task.Run garante que o seeder não "congele" a animação da Pokébola
                await Task.Run(() =>
                {
                    using (var db = new AppDbContext())
                    {
                        // Garante que o banco existe e importa os JSONs
                        DatabaseSeeder.Initialize(db);
                    }
                });

                // Pequena pausa para o utilizador ver a animação terminando
                await Task.Delay(500);

                // Abre a janela principal e fecha esta
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