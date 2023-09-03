
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dbc_Dave.Data.Models
{
    [Table("Queries")]
    public class DataQuery 
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [MaxLength(255)]
        public string QueryName { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string QueryText { get; set; }


    }
}