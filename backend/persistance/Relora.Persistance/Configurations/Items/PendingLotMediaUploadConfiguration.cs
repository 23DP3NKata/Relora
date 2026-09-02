using Relora.Items.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Relora.Persistance.Configurations.Items;

internal sealed class PendingLotMediaUploadConfiguration : IEntityTypeConfiguration<PendingLotMediaUpload>
{
    public void Configure(EntityTypeBuilder<PendingLotMediaUpload> builder)
    {
        builder.ToTable("pending_lot_media_uploads");

        builder.HasKey(upload => upload.Id);
        builder.Property<uint>("xmin").IsRowVersion();

        builder.Property(upload => upload.OwnerId).IsRequired();
        builder.Property(upload => upload.Key).HasMaxLength(500).IsRequired();
        builder.Property(upload => upload.CreatedAtUtc).IsRequired();

        builder.HasIndex(upload => upload.Key).IsUnique();
        builder.HasIndex(upload => upload.CreatedAtUtc);
        builder.HasIndex(upload => upload.OwnerId);
    }
}
