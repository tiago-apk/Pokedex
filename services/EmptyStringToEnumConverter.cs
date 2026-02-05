using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Pokedex.models;

public class EmptyStringToEnumConverter : JsonConverter<PokemonType>
{
    public override PokemonType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string value = reader.GetString();

        // Se a string for vazia ou nula, retorna o enum 'None'
        if (string.IsNullOrEmpty(value))
        {
            return PokemonType.None;
        }

        // Tenta converter o texto (ex: "Fire") para o Enum. 
        // Se falhar, retorna None em vez de estourar erro.
        if (Enum.TryParse<PokemonType>(value, true, out var result))
        {
            return result;
        }

        return PokemonType.None;
    }

    public override void Write(Utf8JsonWriter writer, PokemonType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}