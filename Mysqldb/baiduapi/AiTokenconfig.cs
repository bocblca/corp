using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace  Mysqldb

{
    public class AiTokenconfig : IEntityTypeConfiguration<Aitoken>
    {
        public void Configure(EntityTypeBuilder<Aitoken> builder)
        {
            builder.ToTable("aitokens");
          
            builder.Property(e=>e.AiToken).IsRequired().HasMaxLength(100);
       
        }
    }
}
