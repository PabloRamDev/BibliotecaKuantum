using biblioteca_kuantum.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace biblioteca_kuantum.DataAccessLayer.Data.Configurations
{
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.Property(d => d.CreatedAt)
                .HasDefaultValueSql("now()");

            builder.HasQueryFilter(d => d.DeletedAt == null);
        }
    }
    
}
