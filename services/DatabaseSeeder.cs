using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
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

            // REGISTRO DE CONVERSORES
            jsonOptions.Converters.Add(new FlexibleStringConverter());
            jsonOptions.Converters.Add(new FlexibleIntConverter());
            jsonOptions.Converters.Add(new UniversalEnumConverter<PokemonType>());
            jsonOptions.Converters.Add(new UniversalEnumConverter<PokemonForm>());
            jsonOptions.Converters.Add(new UniversalEnumConverter<AccuracyType>());
            jsonOptions.Converters.Add(new UniversalEnumConverter<StatType>());
            jsonOptions.Converters.Add(new UniversalEnumConverter<Generation>());

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
                    // 1. EXTRAÇÃO DO NOME DO ARQUIVO BASE
                    // Se o JSON diz: "assets/pokemon_images/normal/#0869_01_02.png"
                    // Nós extraímos apenas o "#0869_01_02.png"
                    string fullPath = entry.Image ?? "";
                    string fileName = Path.GetFileName(fullPath);

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        // 2. GERAÇÃO DOS CAMINHOS SHINY (Baseado na pasta padrão)
                        // Se a imagem normal existe, a shiny segue o mesmo nome de arquivo
                        entry.Shiny = $"assets/pokemon_images/shiny/{fileName}";

                        // 3. LÓGICA PARA FÊMEAS (Diferença de gênero)
                        // No seu JSON, o campo 'Female' costuma estar vazio se não houver diferença.
                        // Se houver (como no Pikachu ou Hippowdon), geramos os caminhos.
                        if (entry.Dex == "0025" || entry.Dex == "0172" || !string.IsNullOrEmpty(entry.Female))
                        {
                            entry.Female = $"assets/pokemon_images/female/{fileName}";
                            entry.FemaleShiny = $"assets/pokemon_images/female_shiny/{fileName}";
                        }
                    }

                    // 4. MAPEAMENTO DO G-MAX MOVE
                    // Como seu JSON usa "G-Max Move", o atributo [JsonPropertyName("G-Max Move")] 
                    // no seu Model PokedexEntry.cs deve resolver. 
                    // Se ele estiver vindo vazio, é porque o JSON realmente está vazio para aquele item.
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

    public class UniversalEnumConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string value = reader.GetString();
            if (string.IsNullOrEmpty(value)) return default;

            foreach (var field in typeToConvert.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var attribute = field.GetCustomAttribute<EnumMemberAttribute>();
                if (attribute != null && attribute.Value == value) return (T)field.GetValue(null);
                if (field.Name.Equals(value, StringComparison.OrdinalIgnoreCase)) return (T)field.GetValue(null);
            }
            return default;
        }
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value.ToString());
    }
}