using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pokedex.models
{
    [Table("moves")]
    public class Move
    {
        [Key]
        [Column("name")]
        public string Name { get; set; }
        [Column("type_name")]
        public string TypeName { get; set; }
        [Column("category")]
        public string Category { get; set; }
        [Column("power")]
        public string Power { get; set; }
        [Column("accuracy")]
        public string Accuracy { get; set; }
    }
}