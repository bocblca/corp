using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace workapi.baiduapi
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
