using FIAPCloudGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace FIAPCloudGames.Infrastructure.DatabaseContext
{
    public class AppDbContext : DbContext
    {
        public DbSet<Game> Games { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
    }
}
