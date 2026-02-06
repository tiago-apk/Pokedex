using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Pokedex.models
{
    [Table("pokedex_entries")]
    public class PokedexEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [JsonPropertyName("Dex")]
        public string Dex { get; set; }

        [JsonPropertyName("Name")]
        public string? Name { get; set; }

        [JsonPropertyName("Height")]
        public string? Height { get; set; }

        [JsonPropertyName("Weight")]
        public string? Weight { get; set; }

        [JsonPropertyName("Female")]
        public string? Female { get; set; }

        [JsonPropertyName("Types")]
        public List<string> Types { get; set; } = new List<string>();

        [JsonPropertyName("Abilities")]
        public List<string> Abilities { get; set; } = new List<string>();

        [JsonPropertyName("HP")]
        public int Hp { get; set; }

        [JsonPropertyName("ATK")]
        public int Atk { get; set; }

        [JsonPropertyName("DEF")]
        public int Def { get; set; }

        [JsonPropertyName("SPA")]
        public int Spa { get; set; }

        [JsonPropertyName("SPD")]
        public int Spd { get; set; }

        [JsonPropertyName("SPE")]
        public int Spe { get; set; }

        [JsonPropertyName("BST")]
        public int Bst { get; set; }

        [JsonPropertyName("G-Max Move")]
        public string? GMaxMove { get; set; }

        [JsonPropertyName("Forms")]
        public string? Forms { get; set; }

        [JsonPropertyName("Image")]
        public string? Image { get; set; }

        [JsonPropertyName("Shiny")]
        public string? Shiny { get; set; }

        [JsonPropertyName("Female_Shiny")]
        public string? FemaleShiny { get; set; }
    }
}