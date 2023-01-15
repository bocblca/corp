using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Metadata;
using System.Text.Json.Nodes;

namespace workapi.corp
{
    public class WorkdepartConfig : IEntityTypeConfiguration<Workdepart>
    {
        public void Configure(EntityTypeBuilder<Workdepart> builder)
        {
            builder.HasKey(e => e.id);
            builder.Property(e => e.id).IsRequired().ValueGeneratedOnAddOrUpdate().ValueGeneratedNever();
            builder.Property(e => e.name).HasMaxLength(100).IsUnicode(true);
        }
    }
    public class MemberConfig : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {

            builder.HasKey(e => e.userid);
            builder.Property(e => e.name).HasMaxLength(100).IsUnicode(true);


        }
    }
    public class Assetconfig : IEntityTypeConfiguration<Asset>
    {
        public void Configure(EntityTypeBuilder<Asset> builder)
        {

            builder.HasKey(e => e.Qrcode);
            builder.Property(e => e.Qrcode).HasMaxLength(200).IsUnicode(true);
            builder.Property(e => e.Pname).HasMaxLength(200).IsUnicode(true);
            builder.Property(e=>e.Img).HasMaxLength(200).IsUnicode(true);
            builder.HasOne<Gps>(e => e.Point).WithMany().IsRequired();



        }
    }
    public class Asset_sateconfig : IEntityTypeConfiguration<asset_state>
    {
        public void Configure(EntityTypeBuilder<asset_state> builder)
        {
            builder.ToTable("asset_states");
            builder.HasKey(e => e.spanid);
            builder.Property(e => e.spanid).HasMaxLength(50).ValueGeneratedNever();
            builder.Property(e => e.origin_id).HasMaxLength(50);
            builder.Property(e => e.target_id).HasMaxLength(50);
            builder.Property(e => e.operator_id).HasMaxLength(50);
            builder.Property(e => e.operator_content).HasMaxLength(255);
            builder.Property(e => e.qrcode).HasMaxLength(255);
        } 
    }

}
