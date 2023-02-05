using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Mysqldb
{
    public class gold_TBLconfig : IEntityTypeConfiguration<gold_TBL>
    {
        public void Configure(EntityTypeBuilder<gold_TBL> builder)
        {
            builder.ToTable("gold_TBLs");
            builder.HasKey(e => e.goldID);
            builder.Property (e=>e.goldID).HasMaxLength(50).IsRequired();
            builder.Property(e => e.goldName).HasMaxLength(50).IsUnicode();
            builder.Property(e => e.goldSize).HasMaxLength(50);
            builder.Property(e => e.goldPro).HasMaxLength(50);
            builder.Property(e => e.goldType).HasMaxLength(50);
            builder.Property(e => e.remark).HasMaxLength(50);


        }
    }
}
