using Microsoft.EntityFrameworkCore;



namespace Mysqldb
{
    public class Order_TBLConfig : IEntityTypeConfiguration<Order_TBL>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Order_TBL> builder)
        {
            builder.ToTable("Order_TBLs");
            
            builder.HasKey(e => e.orderID);
            builder.Property(e => e.openid).HasMaxLength(50).IsRequired();
            builder.Property(e => e.orderTime).HasMaxLength(50);
            builder.Property(e => e.orderInfo).HasMaxLength(50);
            builder.Property(e => e.orderName).HasMaxLength(50).IsUnicode(true);
            builder.Property(e => e.orderTel).HasMaxLength(50);
            builder.Property(e => e.sub).HasMaxLength(50);
            builder.Property(e => e.remark).HasMaxLength(100);
            builder.HasOne(e => e.straff).WithMany(e => e.order).HasForeignKey(e=>e.straffID);
        }
    }
}
