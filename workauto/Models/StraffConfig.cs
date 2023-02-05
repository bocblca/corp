using Microsoft.EntityFrameworkCore;

using Mysqldb;

namespace weixin.Models
{
    public class StraffConfig: IEntityTypeConfiguration<Straff>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Straff> builder)
        {
            builder.ToTable("Straffs");
            builder.HasKey(e => e.straffID);
            builder.Property(e => e.straffID).HasMaxLength(10).IsRequired();
            builder.Property(e => e.openid).HasMaxLength(50).IsRequired();
            builder.Property(e => e.straffName).HasMaxLength(50).IsUnicode(true);
            builder.Property(e => e.straffTel).HasMaxLength(50);
            builder.Property(e => e.straffSex).HasMaxLength(50);
            builder.Property(e => e.straffAge).HasMaxLength(50);
            builder.Property(e=>e.acess).HasMaxLength(50);
            

            builder.HasOne<Bank>(e => e.bank).WithMany();
          
                   
        }
    }
}
