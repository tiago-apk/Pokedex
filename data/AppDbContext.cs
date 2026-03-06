using Microsoft.EntityFrameworkCore;
using Pokedex.models;

namespace Pokedex.data
{
    public class AppDbContext : DbContext
    {
        // Tabelas Principais do Pokémon
        public DbSet<Pokemon> Pokemons { get; set; }
        public DbSet<PokemonType> PokemonTypes { get; set; }
        public DbSet<PokemonAbility> PokemonAbilities { get; set; }
        public DbSet<PokemonMove> PokemonMoves { get; set; }
        public DbSet<PokemonEggGroup> PokemonEggGroups { get; set; }
        public DbSet<Evolution> Evolutions { get; set; }
        public DbSet<CosmeticForm> CosmeticForms { get; set; }
        public DbSet<LocalDex> LocalDexes { get; set; }
        public DbSet<PokedexDescription> PokedexDescriptions { get; set; }

        // Tabelas Adicionais (Que já tinhas no teu projeto)
        public DbSet<Move> Moves { get; set; }
        public DbSet<Nature> Natures { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<RegisteredPokemon> RegisteredPokemons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Pega o caminho base de onde o programa está a correr (a pasta bin/Debug...)
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // Junta o caminho com a tua pasta "data_raw"
            string dbPath = System.IO.Path.Combine(baseDir, "data_raw", "pokedex.db");

            // Diz ao SQLite para usar esse caminho exato
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- Configuração das Chaves Primárias Compostas ---
            modelBuilder.Entity<PokemonType>()
                .HasKey(pt => new { pt.PokemonId, pt.TypeName });

            modelBuilder.Entity<PokemonAbility>()
                .HasKey(pa => new { pa.PokemonId, pa.AbilityName });

            modelBuilder.Entity<PokemonEggGroup>()
                .HasKey(pe => new { pe.PokemonId, pe.EggGroupName });

            // --- Configuração das Relações (Foreign Keys) ---
            modelBuilder.Entity<Pokemon>()
                .HasMany(p => p.Types).WithOne(pt => pt.Pokemon).HasForeignKey(pt => pt.PokemonId);

            modelBuilder.Entity<Pokemon>()
                .HasMany(p => p.Abilities).WithOne(pa => pa.Pokemon).HasForeignKey(pa => pa.PokemonId);

            modelBuilder.Entity<Pokemon>()
                .HasMany(p => p.Descriptions).WithOne(pd => pd.Pokemon).HasForeignKey(pd => pd.PokemonId);

            modelBuilder.Entity<Pokemon>()
                .HasMany(p => p.Moves).WithOne(pm => pm.Pokemon).HasForeignKey(pm => pm.PokemonId);

            modelBuilder.Entity<Pokemon>()
                .HasMany(p => p.EggGroups).WithOne(pe => pe.Pokemon).HasForeignKey(pe => pe.PokemonId);

            modelBuilder.Entity<Pokemon>()
                .HasMany(p => p.CosmeticForms).WithOne(cf => cf.Pokemon).HasForeignKey(cf => cf.PokemonId);

            modelBuilder.Entity<Pokemon>()
                .HasMany(p => p.LocalDexes).WithOne(ld => ld.Pokemon).HasForeignKey(ld => ld.PokemonId);

            // A tabela de evoluções tem DUAS chaves estrangeiras para a mesma tabela (Pokemon)
            modelBuilder.Entity<Evolution>()
                .HasOne(e => e.FromPokemon)
                .WithMany(p => p.EvolvesTo)
                .HasForeignKey(e => e.FromPokemonId);

            modelBuilder.Entity<Evolution>()
                .HasOne(e => e.ToPokemon)
                .WithMany(p => p.EvolvesFrom)
                .HasForeignKey(e => e.ToPokemonId);

            base.OnModelCreating(modelBuilder);
        }
    }
}