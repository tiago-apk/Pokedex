using Microsoft.EntityFrameworkCore;
using Pokedex.models;
using System.Collections.Generic;
using System.Text.Json;

namespace Pokedex.data
{
    public class AppDbContext : DbContext
    {
        // Tabelas do Banco de Dados
        public DbSet<PokedexEntry> PokedexEntries { get; set; }
        public DbSet<Move> Moves { get; set; }
        public DbSet<Nature> Natures { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Define o arquivo físico do banco de dados SQLite
            optionsBuilder.UseSqlite("Data Source=pokedex.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- Configurações para PokedexEntry ---

            // Converte a List<PokemonType> para String (JSON) para o SQLite
            modelBuilder.Entity<PokedexEntry>()
                .Property(p => p.Types)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<PokemonType>>(v, (JsonSerializerOptions)null)
                );

            // Converte a List<string> de Abilities para String (JSON)
            modelBuilder.Entity<PokedexEntry>()
                .Property(p => p.Abilities)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null)
                );

            // Salva o Enum Forms como String no banco
            modelBuilder.Entity<PokedexEntry>()
                .Property(p => p.Forms)
                .HasConversion<string>();


            // --- Configurações para Move ---

            // Salva os Enums de Move como String para facilitar leitura externa
            modelBuilder.Entity<Move>().Property(m => m.Type).HasConversion<string>();
            modelBuilder.Entity<Move>().Property(m => m.Accuracy).HasConversion<string>();
            modelBuilder.Entity<Move>().Property(m => m.Gen).HasConversion<string>();


            // --- Configurações para Nature ---

            // Salva os Enums de StatType como String
            modelBuilder.Entity<Nature>().Property(n => n.Increased).HasConversion<string>();
            modelBuilder.Entity<Nature>().Property(n => n.Decreased).HasConversion<string>();

            base.OnModelCreating(modelBuilder);
        }
    }
}