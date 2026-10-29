using biblioteca_kuantum.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace biblioteca_kuantum.DataAccessLayer.Data.Configurations
{
    public class DocumentPageIndexConfiguration : IEntityTypeConfiguration<DocumentPageIndex>
    {
        public void Configure(EntityTypeBuilder<DocumentPageIndex> builder)
        {
            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("now()");
        }
    }
}
