using AuthOnion.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthOnion.Api.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
    }
}
