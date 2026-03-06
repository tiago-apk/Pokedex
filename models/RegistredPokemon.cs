using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pokedex.models
{
    [Table("caught_pokemon")]
    public class RegisteredPokemon
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("trainer_id")]
        public string TrainerId { get; set; }

        [Column("pokemon_id")]
        public string PokemonId { get; set; }

        [Column("nickname")]
        public string? Nickname { get; set; }

        [Column("level")]
        public int Level { get; set; }

        [Column("nature")]
        public string Nature { get; set; }

        [Column("ability")]
        public string Ability { get; set; }

        [Column("move_1")] public string? Move1 { get; set; }
        [Column("move_2")] public string? Move2 { get; set; }
        [Column("move_3")] public string? Move3 { get; set; }
        [Column("move_4")] public string? Move4 { get; set; }

        [Column("iv_hp")] public int IvHp { get; set; }
        [Column("iv_atk")] public int IvAtk { get; set; }
        [Column("iv_def")] public int IvDef { get; set; }
        [Column("iv_spa")] public int IvSpa { get; set; }
        [Column("iv_spd")] public int IvSpd { get; set; }
        [Column("iv_spe")] public int IvSpe { get; set; }

        [Column("ev_hp")] public int EvHp { get; set; }
        [Column("ev_atk")] public int EvAtk { get; set; }
        [Column("ev_def")] public int EvDef { get; set; }
        [Column("ev_spa")] public int EvSpa { get; set; }
        [Column("ev_spd")] public int EvSpd { get; set; }
        [Column("ev_spe")] public int EvSpe { get; set; }

        [Column("date_caught")]
        public DateTime DateCaught { get; set; }
        [Column("is_shiny")]
        public bool IsShiny { get; set; }

        // Propriedades auxiliares que o C# usa mas o banco não tem
        [NotMapped]
        public string BasePokemon { get; set; }

        [NotMapped]
        public string Form { get; set; }

        [NotMapped]
        public string TrainerName { get; set; }
    }
}