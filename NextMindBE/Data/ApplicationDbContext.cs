using Microsoft.EntityFrameworkCore;
using NextMindBE.Model;

namespace NextMindBE.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Ping> Ping { get; set; }
        public DbSet<User> User { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

    }
}
