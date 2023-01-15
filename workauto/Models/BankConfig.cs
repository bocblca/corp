using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mysqldb;

namespace weixin.Models
{
    public class BankConfig : IEntityTypeConfiguration<Bank>
    {
        public void Configure(EntityTypeBuilder<Bank> builder)
        {
            builder.ToTable("Banks");
            builder.HasKey(e => e.bankID);
            builder.Property (e => e.bankName).HasMaxLength (50).IsUnicode().IsRequired();
            builder.Property (e => e.bankID).HasMaxLength (20).IsRequired();
           // builder.HasMany<Straff>(e => e.straff).WithOne(e => e.bankdp).HasForeignKey<Bankdp>(e => e.bankID);
           // builder.HasOne<Bank>(e => e.parent).WithMany(b => b.Children);


        }
    }
}
