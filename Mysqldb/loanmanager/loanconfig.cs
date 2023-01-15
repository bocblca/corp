using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mysqldb
{
    public class loanprocessConfig : IEntityTypeConfiguration<LoanProcess>
    {
        public void Configure(EntityTypeBuilder<LoanProcess> builder)
        {
            builder.Property(e => e.ProcID).HasMaxLength(100).IsRequired();
            builder.Property(e => e.straff1).HasMaxLength(10);
            builder.Property(e => e.straff2).HasMaxLength(10);
            builder.Property(e => e.straff3).HasMaxLength(10);
            builder.Property(e => e.straff4).HasMaxLength(10);
            builder.Property(e => e.bankid).HasMaxLength(10);
            builder.Property(e => e.bankname).HasMaxLength(100);
            builder.Property(e=>e.message).HasMaxLength(255);
            builder.Property(e => e.parentid).HasMaxLength(10);
        }
    }
    public class LoanuserConfig : IEntityTypeConfiguration<Loanuser>
    {
        public void Configure(EntityTypeBuilder<Loanuser> builder)
        {
            builder.Property(e => e.straff).HasMaxLength(10).IsRequired();
            builder.Property(e => e.straff_name).HasMaxLength(20);
            builder.Property(e => e.unionid).HasMaxLength(100);
            builder.Property(e => e.bankid).HasMaxLength(10);
          
        }
    }
    public class ContractConfig : IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> builder)
        {
            builder.ToTable("contracts");
            builder.HasKey(e => e.number);
            builder.Property(e => e.name).HasMaxLength(10);
            builder.Property(e => e.num).HasMaxLength(100);
            builder.Property(e => e.product).HasMaxLength(100);
        }
    }
    public class LoanConfig : IEntityTypeConfiguration<Loans>
    {
        public void Configure(EntityTypeBuilder<Loans> builder)
        {
            builder.ToTable("loans");
            builder.HasKey(e => e.LoanID);
            builder.Property(e=>e.LoanID).HasMaxLength(100);
            builder.Property(e => e.Name).HasMaxLength(100);
            builder.Property(e => e.Type).HasMaxLength(50);
            builder.Property(e => e.Money).HasMaxLength(50);
            builder.Property(e=>e.Vdate).HasMaxLength(50);
        }
    }
    public class hangjobConfig : IEntityTypeConfiguration<hangjob>
    {
        public void Configure(EntityTypeBuilder<hangjob> builder)
        {
            builder.ToTable("hangjobs");
            builder.HasKey(e => e.jobid);
            builder.Property(e => e.jobid).HasMaxLength(100);
            builder.Property(e => e.procid).HasMaxLength(100);
           
        }
    }
}
