using Microsoft.EntityFrameworkCore;

namespace OrderDispatcher.EngagementService.API.Entities
{
    public sealed class EngagementDbContext : DbContext
    {
        public EngagementDbContext(DbContextOptions<EngagementDbContext> options) : base(options) { }

        public DbSet<Profile> Profiles => Set<Profile>();
        public DbSet<Address> Addresses => Set<Address>();
    }
}
