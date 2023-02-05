using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mysqldb
{
    public class wxunionidConfig : IEntityTypeConfiguration<wxunionid>
    {
        public void Configure(EntityTypeBuilder<wxunionid> builder)
        {
            builder.ToTable("unionids");
            builder.HasKey(e=>e.WxUnionid);
            builder.Property(e=>e.WxUnionid).HasMaxLength(100).IsRequired();
            builder.Property(e => e.Wxopenid).HasMaxLength(100);
            builder.Property(e => e.Appopenid).HasMaxLength(100);
            builder.Property(e => e.Gappid).HasMaxLength(100);
            builder.Property(e=>e.Xappid).HasMaxLength(100);
        }
    }
}
