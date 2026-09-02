using Relora.Orders.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Relora.Persistance.Configurations.Orders;

public sealed class OrderDisputeEvidenceConfiguration : IEntityTypeConfiguration<OrderDisputeEvidence>
{
    public void Configure(EntityTypeBuilder<OrderDisputeEvidence> builder)
    {
        builder.ToTable("order_dispute_evidence");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.DisputeId).IsRequired();
        builder.Property(x => x.Key).HasMaxLength(500).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.HasIndex(x => x.DisputeId);
    }
}
