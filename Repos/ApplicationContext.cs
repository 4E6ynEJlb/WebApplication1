using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace Repos
{
    public partial class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
        public ApplicationContext()
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");
            
            modelBuilder.Entity<User>().Property(u => u.Id).HasColumnName("_user_id");
            modelBuilder.Entity<User>().Property(u => u.Name).HasColumnName("_user_name").IsRequired().HasColumnType("varchar(16)");
            modelBuilder.Entity<User>().Property(u => u.IsAdmin).HasColumnName("is_admin");
            
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().HasIndex(u => u.Id).IsUnique();
            modelBuilder.Entity<User>().HasMany(u => u.Ads).WithOne(a => a.User).HasForeignKey(a => a.UserId);


            modelBuilder.Entity<Advertisement>().ToTable("ads");
            
            modelBuilder.Entity<Advertisement>().Property(a => a.Id).HasColumnName("ad_id").IsRequired();
            modelBuilder.Entity<Advertisement>().Property(a => a.Number).HasColumnName("phone_number").IsRequired();
            modelBuilder.Entity<Advertisement>().Property(a => a.UserId).HasColumnName("_user_id").IsRequired();
            modelBuilder.Entity<Advertisement>().Property(a => a.Text).HasColumnName("ad_text").HasMaxLength(512).IsRequired();
            modelBuilder.Entity<Advertisement>().Property(a => a.PicLink).HasColumnName("pic_link").HasDefaultValue("Empty").HasMaxLength(128);
            modelBuilder.Entity<Advertisement>().Property(a => a.Rating).HasColumnName("rating").HasDefaultValue(0);
            modelBuilder.Entity<Advertisement>().Property(a => a.CreationDate).HasColumnName("creation_date").IsRequired();
            modelBuilder.Entity<Advertisement>().Property(a => a.DeletionDate).HasColumnName("deletion_date").IsRequired();

            modelBuilder.Entity<Advertisement>().HasKey(a => a.Id);
            modelBuilder.Entity<Advertisement>().HasIndex(a => a.Id).IsUnique();

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Advertisement> Ads { get; set; }
    }
    //P.S. Пытался создать индексы для Ads.PicLink и Ads.DeletionDate, но mssql server management выдавал ошибку о превышении времени ожидания
}
