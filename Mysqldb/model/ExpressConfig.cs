using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;



namespace Mysqldb
{
    public class ExpressConfig : IEntityTypeConfiguration<Express>
    {
        public void Configure(EntityTypeBuilder<Express> builder)
        {
            builder.ToTable("express");
            builder.HasKey(e => e.expressID);
            builder.Property(e=>e.expressID).IsRequired().HasMaxLength(50);
            builder.Property(e=>e.transID).HasMaxLength(50);
            builder.Property(e=>e.phoneLast).HasMaxLength(50);

        }
    }
}
