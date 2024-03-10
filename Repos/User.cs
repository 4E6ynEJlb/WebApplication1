using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Repos
{
    [Table("users")]
    public class User
    {
        [Column("_user_id")]
        [Required]
        [Key]
        public Guid Id { get; set; }


        [Column("_user_name")]
        [Required]
        [MinLength(4, ErrorMessage = "Name is too short")]
        [MaxLength(16, ErrorMessage = "Name is too long")]
        public string Name { get; set; }


        [Column("is_admin")]
        [Required]
        public bool IsAdmin { get; set; }
    }
}
