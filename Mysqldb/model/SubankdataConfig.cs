using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;



namespace Mysqldb
{
    public class SubankdataConfig : IEntityTypeConfiguration<Subankdata>
    {
        public void Configure(EntityTypeBuilder<Subankdata> builder)
        {
            builder.ToTable("Subankdatas");
           
            builder.HasKey(e => e.Id);
         
                        
            builder.Property(e => e.SubName).HasMaxLength(50).IsRequired().IsUnicode();
            builder.Property(e => e.nums).IsRequired();
        }
    }
}
