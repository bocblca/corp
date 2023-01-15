using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mysqldb
{
    public class BookingConfig : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.ToTable("bookings");
            builder.HasKey(e=>e.Unionid);
            builder.Property(e=>e.Unionid).HasMaxLength(50).IsRequired();
            builder.Property(e => e.Wxopenid).HasMaxLength(50);
            builder.Property(e=>e.CustomerName).HasMaxLength(20).IsUnicode(true);
            builder.Property(e => e.phone).HasMaxLength(20);
            builder.Property(e=>e.Straff).HasMaxLength(20);
            builder.Property(e=>e.Information).HasMaxLength(200).IsUnicode(true);
            builder.Property(e=>e.Nums).HasMaxLength(100);
        }
    }
    public class  RcbStraffConfig : IEntityTypeConfiguration<Rcbstraff>
    {
        public void Configure(EntityTypeBuilder<Rcbstraff> builder)
        {
            builder.ToTable("rcbstraffs");
            builder.HasKey(e => e.Straff);
            builder.Property(e => e.Straff).HasMaxLength(50).IsRequired();
            builder.Property(e => e.BankId).HasMaxLength(50);
            builder.Property(e => e.Name).HasMaxLength(20).IsUnicode(true);
            builder.Property(e=>e.Ace).HasMaxLength(50).IsUnicode(true);
            builder.Property(e => e.Unionid).HasMaxLength(50);
            builder.Property(e=>e.phone).HasMaxLength(20);

        }
    }
}
