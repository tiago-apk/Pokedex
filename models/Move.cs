using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Pokedex.models
{
    [Table("moves")]
    public class Move
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("Type")]
        public PokemonType Type { get; set; } // Usando o Enum Global

        [JsonPropertyName("Category")]
        public string Category { get; set; }

        [JsonPropertyName("PP Min")]
        public string PpMin { get; set; }

        [JsonPropertyName("PP Max")]
        public string PpMax { get; set; }

        [JsonPropertyName("Power")]
        public string Power { get; set; }

        [JsonPropertyName("Accuracy")]
        public AccuracyType Accuracy { get; set; } // Usando Enum

        [JsonPropertyName("Gen")]
        public Generation Gen { get; set; } // Usando Enum
    }
}