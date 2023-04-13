using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mysqldb
{
    public class SupernoticeapprovalConfig : IEntityTypeConfiguration<Supernoticeapproval>
    {
        public void Configure(EntityTypeBuilder<Supernoticeapproval> builder)
        {
            builder.ToTable("Supernoticeapprovals");
            builder.HasKey(b => b.Noticeid);
            builder.Property(b=>b.Noticeid).IsRequired().HasMaxLength(100);
                    
        }
    }
}
