
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SMarket.Core.Model;

namespace SMarket.Core.Data
{
    public class SMarketContext : IdentityDbContext<Users>
    {
        public SMarketContext(DbContextOptions<SMarketContext> options)
            : base(options)
        {
        }

        public DbSet<Itens> Itens { get; set; }
        public DbSet<Logs> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
