using System;
using System.ComponentModel.DataAnnotations;

namespace Pokedex.models
{
    public class RegisteredPokemon
    {
        [Key]
        public int Id { get; set; }

        // A ligação ao Treinador (Foreign Key lógica)
        public int TrainerId { get; set; }
        public string TrainerName { get; set; }

        // Dados Base
        public string BasePokemon { get; set; }
        public string Form { get; set; }
        public string Nickname { get; set; }
        public bool IsShiny { get; set; }
        public int Level { get; set; }
        public string Nature { get; set; }
        public string Ability { get; set; }

        // Golpes
        public string Move1 { get; set; }
        public string Move2 { get; set; }
        public string Move3 { get; set; }
        public string Move4 { get; set; }

        // IVs
        public int IvHp { get; set; }
        public int IvAtk { get; set; }
        public int IvDef { get; set; }
        public int IvSpa { get; set; }
        public int IvSpd { get; set; }
        public int IvSpe { get; set; }

        // EVs
        public int EvHp { get; set; }
        public int EvAtk { get; set; }
        public int EvDef { get; set; }
        public int EvSpa { get; set; }
        public int EvSpd { get; set; }
        public int EvSpe { get; set; }

        public DateTime RegisteredAt { get; set; } = DateTime.Now;
    }
}