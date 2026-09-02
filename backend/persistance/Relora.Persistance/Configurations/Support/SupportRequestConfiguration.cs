using Relora.Support.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Relora.Persistance.Configurations.Support;

public sealed class SupportRequestConfiguration : IEntityTypeConfiguration<SupportRequest>
{
    public void Configure(EntityTypeBuilder<SupportRequest> builder)
    {
        builder.ToTable("support_requests");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Email).HasMaxLength(320).IsRequired();
        builder.Property(x => x.Category).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Subject).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Message).HasMaxLength(5000).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.HasIndex(x => x.CreatedAt);
    }
}
