using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pokedex.models
{

    [Table("pokedex_entries")]
    public class PokedexEntry
    {
        [Key]
        [JsonPropertyName("dex")]
        public int Dex { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("height")]
        public string Height { get; set; }

        [JsonPropertyName("weight")]
        public string Weight { get; set; }

        [JsonPropertyName("female")]
        public string Female { get; set; }

        // Listas no C#
        [JsonPropertyName("types")]
        public List<PokemonType> Types { get; set; } = new List<PokemonType>();

        [JsonPropertyName("abilities")]
        public List<string> Abilities { get; set; } = new List<string>();

        // Atributos de Status (Inteiros)
        [JsonPropertyName("hp")]
        public int Hp { get; set; }

        [JsonPropertyName("atk")]
        public int Atk { get; set; }

        [JsonPropertyName("def")]
        public int Def { get; set; }

        [JsonPropertyName("spa")]
        public int Spa { get; set; }

        [JsonPropertyName("spd")]
        public int Spd { get; set; }

        [JsonPropertyName("spe")]
        public int Spe { get; set; }

        [JsonPropertyName("bst")]
        public int Bst { get; set; }

        [JsonPropertyName("gMaxMove")]
        public string GMaxMove { get; set; }

        // Enumerado
        [JsonPropertyName("forms")]
        public PokemonForm Forms { get; set; }

        // Textos (Caminhos de Imagem)
        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("shiny")]
        public string Shiny { get; set; }

        [JsonPropertyName("femaleShiny")]
        public string FemaleShiny { get; set; }
    }
}
