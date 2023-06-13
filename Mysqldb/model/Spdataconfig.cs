using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mysqldb.model
{
    internal class Spdataconfig : IEntityTypeConfiguration<Spdata>
    {
        public void Configure(EntityTypeBuilder<Spdata> builder)
        {
            builder.ToTable("spdatas");
            builder.HasKey(e => e.Sp_no);
            builder.Property(e => e.Sp_no).HasMaxLength(100);
            builder.Property(e=>e.Sp_status).HasMaxLength(20);
            builder.Property(e=>e.Sp_type).HasMaxLength(20);
            builder.Property(e=>e.Apply_departid).HasMaxLength(20);
            builder.Property(e=>e.Apply_userid).HasMaxLength(20);
            builder.Property(e => e.Username).HasMaxLength(20).IsRequired(false);
            builder.Property(e=>e.Departname).HasMaxLength(50).IsRequired(false);
            builder.Property(e=>e.Apply_time).HasMaxLength(200);
            builder.Property(e=>e.Start).HasMaxLength(200);
            builder.Property(e=>e.End).HasMaxLength(200);
     

        }
    }
}
