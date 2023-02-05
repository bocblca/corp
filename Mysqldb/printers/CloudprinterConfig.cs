using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mysqldb
{
    public class CloudprinterConfig : IEntityTypeConfiguration<Cloudprinter>
    {
        public void Configure(EntityTypeBuilder<Cloudprinter> builder)
        {
            builder.ToTable("cloundprinters");
            builder.HasKey(x => x.Code);
            builder.Property(x => x.Code).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Name).HasMaxLength(100).IsUnicode();
            builder.Property(x=>x.Fprname).HasMaxLength(100);
            builder.Property(x=>x.Sprname).HasMaxLength(100);
            builder.Property(x=>x.straff).HasMaxLength(10);
            builder.Property(x=>x.Bankcode).HasMaxLength(50);
            builder.Property(x=>x.Regcode).HasMaxLength(50);
            builder.Property(x=>x.BankName).HasMaxLength(50).IsUnicode();
           
        }
    }
}
