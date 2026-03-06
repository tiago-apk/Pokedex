using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pokedex.models
{
    [Table("trainers")] // Diz que o nome da tabela em minúsculo
    public class Trainer
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("trainer_id")] // <-- ISTO RESOLVE O ERRO DO TRAINER ID!
        public string TrainerId { get; set; }

        [Column("region")]
        public string Region { get; set; }

        [Column("generation")]
        public string Generation { get; set; }

        [Column("game")]
        public string Game { get; set; }

        [NotMapped] // <-- ISTO RESOLVE O ERRO DO REGISTERED AT!
        public DateTime RegisteredAt { get; set; }
    }
}