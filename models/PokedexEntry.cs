using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Text.Json;

namespace Pokedex.models
{
    [Table("pokedex_entries")]
    public class PokedexEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore] // O Id é gerado pelo banco, o JSON não o tem
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

        // ==========================================
        // ARRAYS SIMPLES (Tipos e Habilidades)
        // ==========================================
        [JsonPropertyName("Types")]
        [NotMapped]
        public List<string> Types { get; set; } = new List<string>();

        [JsonIgnore]
        public string TypesDb
        {
            get => Types == null ? null : JsonSerializer.Serialize(Types);
            set => Types = string.IsNullOrWhiteSpace(value) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(value);
        }

        [JsonPropertyName("Abilities")]
        [NotMapped]
        public List<string> Abilities { get; set; } = new List<string>();

        [JsonIgnore]
        public string AbilitiesDb
        {
            get => Abilities == null ? null : JsonSerializer.Serialize(Abilities);
            set => Abilities = string.IsNullOrWhiteSpace(value) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(value);
        }

        // ==========================================
        // STATUS E INFOS GERAIS
        // ==========================================
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

        // ==========================================
        // DADOS COMPLEXOS DA POKEAPI
        // ==========================================
        [JsonPropertyName("Description")]
        public string? Description { get; set; }

        [JsonPropertyName("Evolution")]
        public EvolutionData? Evolution { get; set; }

        // CORRIGIDO: Alterado de Dictionary para List para bater exatamente com o JSON
        [JsonPropertyName("Moves")]
        [NotMapped] // Adicionado NotMapped para evitar erro no Entity Framework (usando o mesmo padrão de TypesDb)
        public List<MoveData> Moves { get; set; }

        // Adicionado o backing field para o Entity Framework conseguir gravar no banco de dados
        [JsonIgnore]
        public string MovesDb
        {
            get => Moves == null || Moves.Count == 0 ? "[]" : JsonSerializer.Serialize(Moves);
            set => Moves = string.IsNullOrWhiteSpace(value) ? new List<MoveData>() : JsonSerializer.Deserialize<List<MoveData>>(value);
        }
    }

    // ==========================================
    // CLASSES AUXILIARES PARA LER O NOVO JSON
    // ==========================================
    public class EvolutionData
    {
        [JsonPropertyName("EvolvesFrom")]
        public string EvolvesFrom { get; set; }

        [JsonPropertyName("EvolvesTo")]
        public List<EvolutionDetail> EvolvesTo { get; set; }
    }

    public class EvolutionDetail
    {
        [JsonPropertyName("Species")]
        public string Species { get; set; }

        [JsonPropertyName("Method")]
        public string Method { get; set; }

        [JsonPropertyName("MinLevel")]
        public int? MinLevel { get; set; }

        [JsonPropertyName("Item")]
        public string Item { get; set; }
    }

    public class MoveData
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("Method")]
        public string Method { get; set; }

        [JsonPropertyName("Level")]
        public int Level { get; set; }
    }
}