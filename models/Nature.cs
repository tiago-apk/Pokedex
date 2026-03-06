using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pokedex.models
{
    [Table("natures")]
    public class Nature
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("increase_stat")]
        public string Increase { get; set; }
        [Column("decrease_stat")]
        public string Decrease { get; set; }
    }
}