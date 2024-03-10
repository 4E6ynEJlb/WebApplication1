using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Repos
{
    public partial class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Advertisement> Ads { get; set; }
    }
}
