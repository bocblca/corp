using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mysqldb;
using workapi;

namespace weixin.Models
{
    public class CredittokenConfig : IEntityTypeConfiguration<Credittoken>
    {
        public void Configure(EntityTypeBuilder<Credittoken> builder)
        {
            builder.ToTable("credits");
            builder.HasKey(x => x.Id);
            builder.Property(e => e.Id).IsRequired();
            builder.Property(e=>e.token).IsRequired().HasMaxLength(100).IsUnicode();
        }
    }
}
