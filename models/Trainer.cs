using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pokedex.models
{
    [Table("trainers")]
    public class Trainer
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("trainer_id")]
        public string TrainerId { get; set; }

        [Column("region")]
        public string Region { get; set; }

        [Column("generation")]
        public string Generation { get; set; }

        [Column("game")]
        public string Game { get; set; }

        [NotMapped]
        public DateTime RegisteredAt { get; set; }
    }
}