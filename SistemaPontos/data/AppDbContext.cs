using Microsoft.EntityFrameworkCore;
using SistemaPontos.models;

namespace SistemaPontos.data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<PunchClock> PunchClocks { get; set; }
        public DbSet<Settings> Settings { get; set; }
    }
}
