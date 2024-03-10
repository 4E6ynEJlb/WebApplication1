using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repos
{
    [Table("ads")]
    public class Advertisement
    {
        [Column("ad_id")]
        [Required]
        [Key]
        public Guid Id { get; set; }


        [Column("phone_number")]
        [Required]
        public int Number { get; set; }


        [Column("_user_id")]
        [Required]
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }


        [Column("ad_text")]
        [Required]
        [MinLength(8, ErrorMessage = "Text is too short")]
        [MaxLength(512, ErrorMessage = "Text is too long")]
        public string Text { get; set; }


        [Column("pic_link")]
        [Required]
        public string PicLink { get; set; }


        [Column("rating")]
        public int Rating { get; set; }


        [Column("creation_date")]
        public DateTime CreationDate { get; set; }


        [Column("deletion_date")]
        public DateTime DeletionDate { get; set; }
    }
}
