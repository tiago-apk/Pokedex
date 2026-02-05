using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Pokedex.models
{
    // Removido o [JsonConverter] global daqui, pois vamos registrar 
    // o UniversalEnumConverter diretamente no JsonSerializerOptions do Seeder.

    public enum PokemonForm
    {
        // Importante: O valor que representa "" ou nulo deve ser o primeiro (0)
        [EnumMember(Value = "")] Empty = 0,
        Altered,
        Alternative,
        Eternamax,
        Gender,
        Gigantamax,
        Megaevolution,
        Origin,
        Primal,
        Regional,
        Stellar,
        Terastal
    }

    public enum PokemonType
    {
        None = 0,
        Normal, Fire, Water, Grass, Electric, Ice, Fighting, Poison,
        Ground, Flying, Psychic, Bug, Rock, Ghost, Dragon, Steel,
        Fairy, Dark, Stellar
    }

    public enum Generation
    {
        [EnumMember(Value = "")] None = 0,
        [EnumMember(Value = "I")] Gen1,
        [EnumMember(Value = "II")] Gen2,
        [EnumMember(Value = "III")] Gen3,
        [EnumMember(Value = "IV")] Gen4,
        [EnumMember(Value = "V")] Gen5,
        [EnumMember(Value = "VI")] Gen6,
        [EnumMember(Value = "VII")] Gen7,
        [EnumMember(Value = "VIII")] Gen8,
        [EnumMember(Value = "IX")] Gen9
    }

    public enum AccuracyType
    {
        None = 0,
        [EnumMember(Value = "100%")] Acc100,
        [EnumMember(Value = "95%")] Acc95,
        [EnumMember(Value = "90%")] Acc90,
        [EnumMember(Value = "85%")] Acc85,
        [EnumMember(Value = "80%")] Acc80,
        [EnumMember(Value = "75%")] Acc75,
        [EnumMember(Value = "70%")] Acc70,
        [EnumMember(Value = "50%")] Acc50,
        [EnumMember(Value = "30%")] Acc30,
        [EnumMember(Value = "—")] SymbolNone
    }

    public enum StatType
    {
        None = 0,
        Hp,
        Attack,
        Defense,
        SpecialAttack,
        SpecialDefense,
        Speed
    }
}