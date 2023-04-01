using Microsoft.EntityFrameworkCore;



namespace Mysqldb
{
    public class SupernoticeConfig: IEntityTypeConfiguration<Supernotice>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Supernotice> builder)
        {
            builder.ToTable("Supernotices");
            builder.HasKey(e => e.NoticeId);
            builder.Property(e => e.NoticeId).HasMaxLength(50).IsUnicode(true);
      
        }
    }
}
