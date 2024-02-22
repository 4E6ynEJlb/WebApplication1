using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMakler
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
        [MinLength(4, ErrorMessage ="Name is too short")]
        [MaxLength(16, ErrorMessage = "Name is too long")]
        public string Name { get; set; }


        [Column("is_admin")]
        [Required]
        public bool IsAdmin { get; set; }
    }


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
        [Required]
        public DateTime CreationDate { get; set; }


        [Column("deletion_date")]
        [Required]
        public DateTime DeletionDate { get; set; }
    }
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Advertisement> Ads { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = @"Data Source=DESKTOP-1EUC064;
                                        AttachDbFilename=D:\TestTaskDex\TTPrimary.mdf;
                                        Integrated Security=True;
                                        Trust Server Certificate=True";
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
