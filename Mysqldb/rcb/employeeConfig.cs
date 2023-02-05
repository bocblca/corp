using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Mysqldb
{
    public class employeeConfig : IEntityTypeConfiguration<employee>
    {
        public void Configure(EntityTypeBuilder<employee> builder)
        {
            builder.ToTable("employees");
            builder.HasKey(e => e.EmployeeID);
            builder.Property(e => e.EmployeeID).IsRequired().HasMaxLength(12);
            builder.Property(e => e.Name).HasMaxLength(50).IsUnicode();
            builder.Property(e => e.Depart).HasMaxLength(50);
            builder.Property(e => e.DepartID).HasMaxLength(10);
            builder.Property(e => e.acces).HasMaxLength(20);
         
          

        }
    }
}
