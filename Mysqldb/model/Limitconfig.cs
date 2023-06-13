using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mysqldb.model
{
    public class Limitconfig : IEntityTypeConfiguration<Limit>
    {
        public void Configure(EntityTypeBuilder<Limit> builder)
        {
            builder.ToTable("limits");
            builder.HasKey(e => e.Limitid);
            builder.Property(e => e.Limitid).IsRequired(true).HasMaxLength(100);
            builder.Property(e => e.Userid).IsRequired(true).HasMaxLength(20);
            builder.Property(e => e.Username).IsRequired(false).HasMaxLength(20);
            builder.Property(e=>e.Leaderid).IsRequired(false).HasMaxLength(20);
            builder.Property(e=>e.Leadername).IsRequired(false).HasMaxLength(20);
            builder.Property(e => e.Departid).IsRequired(false).HasMaxLength(20);
            builder.Property(e => e.Departname).IsRequired(false).HasMaxLength(50);
            builder.Property(e=>e.Conttype).IsRequired(true).HasMaxLength(100);
            builder.Property(e=>e.Content).IsRequired(true);
            builder.Property(e=>e.Approval).IsRequired(false).HasMaxLength(20);
            builder.Property(e => e.Approval_content).IsRequired(false);    
            builder.Property(e=>e.Transtime).IsRequired(true);
            builder.Property(e=>e.Relay_approval).IsRequired(false).HasMaxLength(20);
            builder.Property(e=>e.Relay_approval_content).IsRequired(false);
            builder.Property(e=>e.Relay_departid).IsRequired(false).HasMaxLength(20);
            builder.Property(e=>e.Relay_departname).IsRequired(false).HasMaxLength(20);
            builder.Property(e=>e.Relay_leaderid).IsRequired(false).HasMaxLength(20);
            builder.Property(e=>e.Relay_leadername).IsRequired(false).HasMaxLength(20);
            builder.Property(e=>e.Relay_userid).IsRequired(false).HasMaxLength(20);
            builder.Property(e=>e.Relay_username).IsRequired(false).HasMaxLength(20);
            builder.Property(e=>e.Departname).IsRequired(false).HasMaxLength(100);
            builder.Property(e => e.Info).IsRequired(true);
            builder.Property(e=>e.Detail).IsRequired(false);

        }
    }
}
