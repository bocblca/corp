using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Mysqldb
{
    public class Trans_TBLConfig : IEntityTypeConfiguration<Trans_TBL>
    {
        public void Configure(EntityTypeBuilder<Trans_TBL> builder)
        {
            builder.ToTable("Trans_TBLs");
            builder.HasKey(e => e.busID);
            builder.Property(e=>e.busID).HasMaxLength(50).IsRequired();
            builder.Property(e=>e.transID).HasMaxLength(50);
            builder.Property(e => e.prepay_ID).HasMaxLength(50);
            builder.Property(e => e.openid).HasMaxLength(50);
            builder.Property(e => e.straffID).HasMaxLength(10);
            builder.Property(e => e.productID).HasMaxLength(50);
            builder.Property(e => e.transType).HasMaxLength(10);
            builder.Property(e => e.remark).HasMaxLength(100);
            builder.HasOne(e=>e.express).WithOne(e=>e.trans).HasForeignKey<Express>(e=>e.transID);
            builder.HasOne<gold_TBL>(e => e.goods).WithMany().HasForeignKey(e=>e.productID);

        }
    }
}
