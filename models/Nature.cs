using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Pokedex.models
{
    [Table("natures")]
    public class Nature
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("Increased")]
        public StatType Increased { get; set; }

        [JsonPropertyName("Decreased")]
        public StatType Decreased { get; set; }

        // Propriedade utilitária: retorna se a natureza é neutra
        [NotMapped]
        public bool IsNeutral => Increased == Decreased || Increased == StatType.None;
    }
}