using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pokedex.models
{
    [Table("pokemon")]
    public class Pokemon
    {
        [Key]
        [Column("id")]
        public string Id { get; set; }

        [Column("parent_id")] public string? ParentId { get; set; }
        [Column("national_dex")] public int? NationalDex { get; set; }
        [Column("name")] public string? Name { get; set; }
        [Column("form")] public string? Form { get; set; }
        [Column("species")] public string? Species { get; set; }
        [Column("height")] public string? Height { get; set; }
        [Column("weight")] public string? Weight { get; set; }
        [Column("dex_color")] public string? DexColor { get; set; }

        [Column("image_normal")] public string? ImageNormal { get; set; }
        [Column("image_shiny")] public string? ImageShiny { get; set; }
        [Column("image_female")] public string? ImageFemale { get; set; }
        [Column("image_female_shiny")] public string? ImageFemaleShiny { get; set; }

        [Column("hp")] public int? Hp { get; set; }
        [Column("attack")] public int? Attack { get; set; }
        [Column("defense")] public int? Defense { get; set; }
        [Column("sp_atk")] public int? SpAtk { get; set; }
        [Column("sp_def")] public int? SpDef { get; set; }
        [Column("speed")] public int? Speed { get; set; }
        [Column("total_stats")] public int? TotalStats { get; set; }

        [Column("ev_hp")] public int? EvHp { get; set; }
        [Column("ev_attack")] public int? EvAttack { get; set; }
        [Column("ev_defense")] public int? EvDefense { get; set; }
        [Column("ev_sp_atk")] public int? EvSpAtk { get; set; }
        [Column("ev_sp_def")] public int? EvSpDef { get; set; }
        [Column("ev_speed")] public int? EvSpeed { get; set; }

        [Column("catch_rate")] public string? CatchRate { get; set; }
        [Column("base_friendship")] public int? BaseFriendship { get; set; }
        [Column("base_exp")] public int? BaseExp { get; set; }
        [Column("growth_rate")] public string? GrowthRate { get; set; }
        [Column("gender_ratio")] public string? GenderRatio { get; set; }
        [Column("egg_cycles")] public string? EggCycles { get; set; }

        public virtual ICollection<PokemonType> Types { get; set; } = new List<PokemonType>();
        public virtual ICollection<PokemonAbility> Abilities { get; set; } = new List<PokemonAbility>();
        public virtual ICollection<PokedexDescription> Descriptions { get; set; } = new List<PokedexDescription>();
        public virtual ICollection<PokemonMove> Moves { get; set; } = new List<PokemonMove>();
        public virtual ICollection<PokemonEggGroup> EggGroups { get; set; } = new List<PokemonEggGroup>();
        public virtual ICollection<CosmeticForm> CosmeticForms { get; set; } = new List<CosmeticForm>();
        public virtual ICollection<LocalDex> LocalDexes { get; set; } = new List<LocalDex>();

        [InverseProperty("FromPokemon")]
        public virtual ICollection<Evolution> EvolvesTo { get; set; } = new List<Evolution>();

        [InverseProperty("ToPokemon")]
        public virtual ICollection<Evolution> EvolvesFrom { get; set; } = new List<Evolution>();
    }

    [Table("pokemon_types")]
    public class PokemonType
    {
        [Column("pokemon_id")] public string? PokemonId { get; set; }
        [Column("type_name")] public string? TypeName { get; set; }
        [Column("slot")] public int? Slot { get; set; }
        public virtual Pokemon Pokemon { get; set; }
    }

    [Table("pokemon_abilities")]
    public class PokemonAbility
    {
        [Column("pokemon_id")] public string? PokemonId { get; set; }
        [Column("ability_name")] public string? AbilityName { get; set; }
        [Column("is_hidden")] public bool? IsHidden { get; set; }
        [Column("slot")] public int? Slot { get; set; }
        public virtual Pokemon Pokemon { get; set; }
    }

    [Table("pokemon_moves")]
    public class PokemonMove
    {
        [Key][Column("id")] public int Id { get; set; }
        [Column("pokemon_id")] public string? PokemonId { get; set; }
        [Column("move_name")] public string? MoveName { get; set; }
        [Column("generation")] public string? Generation { get; set; }
        [Column("learn_method")] public string? LearnMethod { get; set; }
        [Column("level")] public string? Level { get; set; }
        [Column("tm_number")] public string? TmNumber { get; set; }
        [Column("tr_number")] public string? TrNumber { get; set; }
        [Column("hm_number")] public string? HmNumber { get; set; }
        [Column("game")] public string? Game { get; set; }
        public virtual Pokemon Pokemon { get; set; }
    }

    [Table("pokemon_egg_groups")]
    public class PokemonEggGroup
    {
        [Column("pokemon_id")] public string? PokemonId { get; set; }
        [Column("egg_group_name")] public string? EggGroupName { get; set; }
        public virtual Pokemon Pokemon { get; set; }
    }

    [Table("evolutions")]
    public class Evolution
    {
        [Key][Column("id")] public int Id { get; set; }
        [Column("from_pokemon_id")] public string? FromPokemonId { get; set; }
        [Column("to_pokemon_id")] public string? ToPokemonId { get; set; }
        [Column("method")] public string? Method { get; set; }
        [Column("min_level")] public int? MinLevel { get; set; }
        [Column("item")] public string? Item { get; set; }

        [ForeignKey("FromPokemonId")] public virtual Pokemon FromPokemon { get; set; }
        [ForeignKey("ToPokemonId")] public virtual Pokemon ToPokemon { get; set; }
    }

    [Table("cosmetic_forms")]
    public class CosmeticForm
    {
        [Key][Column("id")] public int Id { get; set; }
        [Column("pokemon_id")] public string? PokemonId { get; set; }
        [Column("form_name")] public string? FormName { get; set; }
        [Column("image_normal")] public string? ImageNormal { get; set; }
        [Column("image_shiny")] public string? ImageShiny { get; set; }
        [Column("image_female")] public string? ImageFemale { get; set; }
        [Column("image_female_shiny")] public string? ImageFemaleShiny { get; set; }
        public virtual ICollection<CosmeticDexEntry> Descriptions { get; set; } = new List<CosmeticDexEntry>();
        public virtual Pokemon Pokemon { get; set; }
    }

    [Table("local_dex")]
    public class LocalDex
    {
        [Key][Column("id")] public int Id { get; set; }
        [Column("pokemon_id")] public string? PokemonId { get; set; }
        [Column("region")] public string? Region { get; set; }
        [Column("dex_number")] public string? DexNumber { get; set; }
        [Column("generation")] public string? Generation { get; set; }
        public virtual Pokemon Pokemon { get; set; }
    }

    [Table("pokedex_entries")]
    public class PokedexDescription
    {
        [Key][Column("id")] public int Id { get; set; }
        [Column("pokemon_id")] public string? PokemonId { get; set; }
        [Column("game")] public string? Game { get; set; }
        [Column("description")] public string? Description { get; set; }
        public virtual Pokemon Pokemon { get; set; }
    }

    [Table("cosmetic_dex_entries")]
    public class CosmeticDexEntry
    {
        [Key][Column("id")] public int Id { get; set; }
        [Column("cosmetic_form_id")] public int? CosmeticFormId { get; set; }
        [Column("game")] public string? Game { get; set; }
        [Column("description")] public string? Description { get; set; }
        [ForeignKey("CosmeticFormId")] public virtual CosmeticForm CosmeticForm { get; set; }
    }
}