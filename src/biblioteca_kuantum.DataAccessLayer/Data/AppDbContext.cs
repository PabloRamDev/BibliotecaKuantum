using biblioteca_kuantum.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;


namespace biblioteca_kuantum.DataAccessLayer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {            
        }

        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentPageIndex> PageIndices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
