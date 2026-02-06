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
        public string Type { get; set; }

        [JsonPropertyName("Category")]
        public string Category { get; set; }

        [JsonPropertyName("PP Min")]
        public string PpMin { get; set; }

        [JsonPropertyName("PP Max")]
        public string PpMax { get; set; }

        [JsonPropertyName("Power")]
        public string Power { get; set; }

        [JsonPropertyName("Accuracy")]
        public string Accuracy { get; set; }

        [JsonPropertyName("Gen")]
        public string Gen { get; set; }
    }
}