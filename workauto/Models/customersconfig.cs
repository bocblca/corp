using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mysqldb;
using workapi;

namespace weixin.Models
{
    public class customersconfig : IEntityTypeConfiguration<customers>
    {
        public void Configure(EntityTypeBuilder<customers> builder)
        {
            builder.ToTable("customers");
            builder.HasKey(e => e.openid);
            builder.Property(e=>e.openid).IsRequired().HasMaxLength(50);
            builder.Property(e => e.unionId).HasMaxLength(50);
            builder.Property(e => e.customerName).HasMaxLength(50).IsUnicode();
            builder.Property(e => e.customerTel1).HasMaxLength(50);
            builder.Property(e => e.customerTel2).HasMaxLength(50);
            builder.Property(e => e.customerTel3).HasMaxLength(50);
            builder.Property(e => e.customerTel4).HasMaxLength(50);
            builder.Property(e => e.customerTel5).HasMaxLength(50);
            builder.Property(e => e.customerAdr1).HasMaxLength(50);
            builder.Property(e => e.customerAdr2).HasMaxLength(50);
            builder.Property(e => e.customerAdr3).HasMaxLength(50);
            builder.Property(e => e.customerAdr4).HasMaxLength(50);
            builder.Property(e => e.customerAdr5).HasMaxLength(50);
            builder.Property(e=>e.remark).HasMaxLength(50);
            builder.HasOne<Straff>(e=>e.straff).WithMany();
        }
    }
}
