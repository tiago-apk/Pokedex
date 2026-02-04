using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Pokedex.models
{
    // Usamos o conversor de string globalmente para todos os enums deste arquivo
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PokemonForm
    {
        [EnumMember(Value = "Altered")] Altered,
        [EnumMember(Value = "Alternative")] Alternative,
        [EnumMember(Value = "")] Empty,
        [EnumMember(Value = "Eternamax")] Eternamax,
        [EnumMember(Value = "Gender")] Gender,
        [EnumMember(Value = "Gigantamax")] Gigantamax,
        [EnumMember(Value = "Megaevolution")] Megaevolution,
        [EnumMember(Value = "Origin")] Origin,
        [EnumMember(Value = "Primal")] Primal,
        [EnumMember(Value = "Regional")] Regional,
        [EnumMember(Value = "Stellar")] Stellar,
        [EnumMember(Value = "Terastal")] Terastal
    }

    public enum PokemonType
    {
        Normal, Fire, Water, Grass, Electric, Ice, Fighting, Poison,
        Ground, Flying, Psychic, Bug, Rock, Ghost, Dragon, Steel,
        Fairy, Dark, Stellar, None
    }

    public enum Generation
    {
        [JsonPropertyName("I")] I,
        [JsonPropertyName("II")] II,
        [JsonPropertyName("III")] III,
        [JsonPropertyName("IV")] IV,
        [JsonPropertyName("V")] V,
        [JsonPropertyName("VI")] VI,
        [JsonPropertyName("VII")] VII,
        [JsonPropertyName("VIII")] VIII,
        [JsonPropertyName("IX")] IX
    }

    public enum AccuracyType
    {
        [JsonPropertyName("100%")] Acc100,
        [JsonPropertyName("95%")] Acc95,
        [JsonPropertyName("90%")] Acc90,
        [JsonPropertyName("85%")] Acc85,
        [JsonPropertyName("80%")] Acc80,
        [JsonPropertyName("75%")] Acc75,
        [JsonPropertyName("70%")] Acc70,
        [JsonPropertyName("50%")] Acc50,
        [JsonPropertyName("30%")] Acc30,
        [JsonPropertyName("—")] None
    }

    public enum StatType
    {
        [JsonPropertyName("HP")] Hp,
        [JsonPropertyName("Attack")] Attack,
        [JsonPropertyName("Defense")] Defense,
        [JsonPropertyName("Sp. Atk")] SpecialAttack,
        [JsonPropertyName("Sp. Def")] SpecialDefense,
        [JsonPropertyName("Speed")] Speed,
        [JsonPropertyName("None")] None
    }
}