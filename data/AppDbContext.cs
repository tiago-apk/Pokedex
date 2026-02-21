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
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<RegisteredPokemon> RegisteredPokemons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Define o arquivo físico do banco de dados SQLite
            optionsBuilder.UseSqlite("Data Source=pokedex.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- Configurações para PokedexEntry ---

            // Converte a List<string> para String (JSON) para o SQLite
            modelBuilder.Entity<PokedexEntry>()
                .Property(p => p.Types)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null)
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

            // =========================================================
            // NOVAS CONVERSÕES PARA DADOS COMPLEXOS DA POKEAPI
            // =========================================================

            modelBuilder.Entity<PokedexEntry>()
                .Property(p => p.Evolution)
                .IsRequired(false) // <-- ADICIONA ISTO PARA PERMITIR NULOS
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<EvolutionData>(v, (JsonSerializerOptions)null)
                );

            // CORREÇÃO AQUI: Mudado de Dictionary para List<MoveData>
            modelBuilder.Entity<PokedexEntry>()
                .Property(p => p.Moves)
                .IsRequired(false) // <-- ADICIONA ISTO PARA PERMITIR NULOS
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<MoveData>>(v, (JsonSerializerOptions)null)
                );


            // --- Configurações para Move ---
            modelBuilder.Entity<Move>().Property(m => m.Type).HasConversion<string>();
            modelBuilder.Entity<Move>().Property(m => m.Accuracy).HasConversion<string>();
            modelBuilder.Entity<Move>().Property(m => m.Gen).HasConversion<string>();

            // --- Configurações para Nature ---
            modelBuilder.Entity<Nature>().Property(n => n.Increase).HasConversion<string>();
            modelBuilder.Entity<Nature>().Property(n => n.Decrease).HasConversion<string>();

            base.OnModelCreating(modelBuilder);
        }
    }
}