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

        [JsonPropertyName("Increase")] // Conforme o JSON
        public string Increase { get; set; }

        [JsonPropertyName("Decrease")] // Conforme o JSON
        public string Decrease { get; set; }
    }
}