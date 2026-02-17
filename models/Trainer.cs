using System;
using System.ComponentModel.DataAnnotations;

namespace Pokedex.models
{
    public class Trainer
    {
        [Key]
        public int Id { get; set; } // O ID interno do banco de dados (Automático)

        public string Name { get; set; }
        public string TrainerId { get; set; }
        public string Region { get; set; }
        public string Generation { get; set; }
        public string Game { get; set; }

        // Regista a data e hora exata em que o treinador foi criado!
        public DateTime RegisteredAt { get; set; } = DateTime.Now;
    }
}