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
                    // Lógica para preencher caminhos de imagens baseada no arquivo fornecido no JSON
                    if (!string.IsNullOrEmpty(entry.Image))
                    {
                        string fileName = Path.GetFileName(entry.Image);
                        entry.Shiny = $"assets/pokemon_images/shiny/{fileName}";

                        if (!string.IsNullOrEmpty(entry.Female))
                        {
                            entry.Female = $"assets/pokemon_images/female/{fileName}";
                            entry.FemaleShiny = $"assets/pokemon_images/female_shiny/{fileName}";
                        }
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