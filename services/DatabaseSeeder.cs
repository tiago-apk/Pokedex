using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Pokedex.data;
using Pokedex.models;

namespace Pokedex.services
{
    public class DatabaseSeeder
    {
        public static void Initialize(AppDbContext context)
        {
            // Garante que o banco e as tabelas existam
            context.Database.EnsureCreated();

            // Caminho base para a pasta data_raw
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string dataRawPath = Path.Combine(baseDir, "data_raw");

            // Configurações de JSON (Case Insensitive para bater com camelCase ou snake_case)
            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // 1. Importar Pokedex
            ImportPokedex(context, Path.Combine(dataRawPath, "pokedex.json"), jsonOptions);

            // 2. Importar Natures
            ImportNatures(context, Path.Combine(dataRawPath, "natures.json"), jsonOptions);

            // 3. Importar Moves
            ImportMoves(context, Path.Combine(dataRawPath, "moves.json"), jsonOptions);
        }

        private static void ImportPokedex(AppDbContext context, string path, JsonSerializerOptions options)
        {
            if (context.PokedexEntries.Any() || !File.Exists(path)) return;

            string json = File.ReadAllText(path);
            var data = JsonSerializer.Deserialize<List<PokedexEntry>>(json, options);
            if (data != null)
            {
                context.PokedexEntries.AddRange(data);
                context.SaveChanges();
            }
        }

        private static void ImportNatures(AppDbContext context, string path, JsonSerializerOptions options)
        {
            // Verificamos se a tabela de Natures está vazia (supondo que você criou o DbSet<Nature>)
            if (context.Natures.Any() || !File.Exists(path)) return;

            string json = File.ReadAllText(path);
            var data = JsonSerializer.Deserialize<List<Nature>>(json, options);
            if (data != null)
            {
                context.Natures.AddRange(data);
                context.SaveChanges();
            }
        }

        private static void ImportMoves(AppDbContext context, string path, JsonSerializerOptions options)
        {
            // Verificamos se a tabela de Moves está vazia (supondo que você criou o DbSet<Move>)
            if (context.Moves.Any() || !File.Exists(path)) return;

            string json = File.ReadAllText(path);
            var data = JsonSerializer.Deserialize<List<Move>>(json, options);
            if (data != null)
            {
                context.Moves.AddRange(data);
                context.SaveChanges();
            }
        }
    }
}