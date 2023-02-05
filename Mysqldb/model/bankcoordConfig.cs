using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;



namespace Mysqldb
{
    public class bankcoordConfig : IEntityTypeConfiguration<bankcoord>
    {
        public void Configure(EntityTypeBuilder<bankcoord> builder)
        {
            builder.ToTable("bankcoords");
            builder.HasKey(e => e.bankID);
            builder.HasOne<Bank>(e => e.bank).WithOne(e=>e.bankcoord).HasForeignKey<bankcoord>(e=>e.bankID);
            builder.Property(e => e.bankID).HasMaxLength(50).IsRequired();
            builder.Property (e=>e.bankName ).HasMaxLength(50).IsUnicode();

        }
    }
}
