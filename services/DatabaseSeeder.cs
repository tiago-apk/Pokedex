using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Pokedex.data;
using Pokedex.models;

namespace Pokedex.services
{
    public class DatabaseSeeder
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string dataRawPath = Path.Combine(baseDir, "data_raw");

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString |
                                 JsonNumberHandling.WriteAsString
            };

            // Mantemos apenas conversores de resiliência, sem Enums.
            jsonOptions.Converters.Add(new FlexibleStringConverter());
            jsonOptions.Converters.Add(new FlexibleIntConverter());

            ImportPokedex(context, Path.Combine(dataRawPath, "pokedex.json"), jsonOptions);
            ImportNatures(context, Path.Combine(dataRawPath, "natures.json"), jsonOptions);
            ImportMoves(context, Path.Combine(dataRawPath, "moves.json"), jsonOptions);
        }

        private static void ImportPokedex(AppDbContext context, string path, JsonSerializerOptions options)
        {
            if (context.PokedexEntries.Any() || !File.Exists(path)) return;

            string json = File.ReadAllText(path).Trim();
            var wrapper = JsonSerializer.Deserialize<PokedexWrapper>(json, options);

            if (wrapper?.Pokedex != null)
            {
                foreach (var entry in wrapper.Pokedex)
                {
                    // =========================================================
                    // 1. SANITIZAÇÃO DE DESCRIÇÃO (Herança da Forma Base)
                    // =========================================================
                    if (string.IsNullOrWhiteSpace(entry.Description) ||
                        entry.Description.Trim().Equals("description not found", StringComparison.OrdinalIgnoreCase))
                    {
                        // Se o Dex tem ponto (ex: 003.1), procuramos a base (003)
                        if (!string.IsNullOrWhiteSpace(entry.Dex) && entry.Dex.Contains("."))
                        {
                            string baseDex = entry.Dex.Split('.')[0];
                            var baseEntry = wrapper.Pokedex.FirstOrDefault(p => p.Dex == baseDex);

                            // Se encontrou a base e ela TEM uma descrição válida, copiamos
                            if (baseEntry != null &&
                                !string.IsNullOrWhiteSpace(baseEntry.Description) &&
                                !baseEntry.Description.Trim().Equals("description not found", StringComparison.OrdinalIgnoreCase))
                            {
                                entry.Description = baseEntry.Description;
                            }
                        }
                    }

                    // =========================================================
                    // 2. SANITIZAÇÃO DE VARIANTES E SHINIES
                    // =========================================================

                    // Sanitização do Shiny
                    if (!string.IsNullOrWhiteSpace(entry.Shiny))
                    {
                        string s = entry.Shiny.Trim().ToLower();
                        if (s == "null" || s == "none") entry.Shiny = "";
                    }
                    else
                    {
                        entry.Shiny = ""; // Garante que nulos se tornem vazios limpos
                    }

                    // Sanitização do Female
                    if (!string.IsNullOrWhiteSpace(entry.Female))
                    {
                        string f = entry.Female.Trim().ToLower();
                        if (f == "null" || f == "none" || f == "0" || f == "false") entry.Female = "";
                    }
                    else
                    {
                        entry.Female = "";
                    }

                    // Sanitização do FemaleShiny
                    if (!string.IsNullOrWhiteSpace(entry.FemaleShiny))
                    {
                        string fs = entry.FemaleShiny.Trim().ToLower();
                        if (fs == "null" || fs == "none") entry.FemaleShiny = "";
                    }
                    else
                    {
                        entry.FemaleShiny = "";
                    }
                }

                context.PokedexEntries.AddRange(wrapper.Pokedex);
                context.SaveChanges();
            }
        }

        private static void ImportNatures(AppDbContext context, string path, JsonSerializerOptions options)
        {
            if (context.Natures.Any() || !File.Exists(path)) return;
            string json = File.ReadAllText(path).Trim();
            var wrapper = JsonSerializer.Deserialize<NaturesWrapper>(json, options);
            if (wrapper?.Natures != null)
            {
                context.Natures.AddRange(wrapper.Natures);
                context.SaveChanges();
            }
        }

        private static void ImportMoves(AppDbContext context, string path, JsonSerializerOptions options)
        {
            if (context.Moves.Any() || !File.Exists(path)) return;
            string json = File.ReadAllText(path).Trim();
            var wrapper = JsonSerializer.Deserialize<MovesWrapper>(json, options);
            if (wrapper?.Moves != null)
            {
                context.Moves.AddRange(wrapper.Moves);
                context.SaveChanges();
            }
        }

        private class PokedexWrapper { public List<PokedexEntry> Pokedex { get; set; } }
        private class NaturesWrapper { public List<Nature> Natures { get; set; } }
        private class MovesWrapper { public List<Move> Moves { get; set; } }
    }

    public class FlexibleStringConverter : JsonConverter<string?>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            if (reader.TokenType == JsonTokenType.Number) return reader.GetDouble().ToString();
            if (reader.TokenType == JsonTokenType.String)
            {
                string value = reader.GetString();
                return string.IsNullOrWhiteSpace(value) ? null : value;
            }
            return null;
        }
        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value);
    }

    public class FlexibleIntConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string value = reader.GetString();
                return int.TryParse(value, out int result) ? result : 0;
            }
            return reader.TokenType == JsonTokenType.Number ? reader.GetInt32() : 0;
        }
        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options) =>
            writer.WriteNumberValue(value);
    }
}