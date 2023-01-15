using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace workapi.account
{
    public class AccountConfig : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("accounts");
            builder.HasKey(e=>e.Nums);
            builder.Property(e=>e.Legal).HasMaxLength(20).IsUnicode(true);
            builder.Property(e=>e.Address).HasMaxLength(200).IsUnicode(true);
            builder.Property(e=>e.Dept).HasMaxLength(100).IsUnicode(true);
            builder.Property(e=>e.Name).HasMaxLength(200).IsUnicode(true);
            builder.Property(e=>e.Regcapital).HasMaxLength(50).IsUnicode(true);
            builder.Property(e=>e.Pname).HasMaxLength(50).IsUnicode(true);
            builder.Property(e=>e.Sex).HasMaxLength(10).IsUnicode(true);
            builder.Property(e=>e.Birth).HasMaxLength(50);
            builder.Property(e=>e.Faildate).HasMaxLength(50);
            builder.Property(e=>e.Issudate).HasMaxLength(50);
            builder.Property(e=>e.Nation).HasMaxLength(50).IsUnicode(true);
            builder.Property(e=>e.Paddress).HasMaxLength(200).IsUnicode(true);
            builder.Property(e=>e.Phone).HasMaxLength(50);
            builder.Property(e=>e.Pnums).HasMaxLength(50);
            builder.Property(e=>e.Range).HasMaxLength(500).IsUnicode(true);
            builder.Property(e=>e.Type).HasMaxLength(50).IsUnicode(true);
            builder.Property(e=>e.Validdate).HasMaxLength(50);
        }
    }
}
