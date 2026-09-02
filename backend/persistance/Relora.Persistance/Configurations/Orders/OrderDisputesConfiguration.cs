using Relora.Orders.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Relora.Persistance.Configurations.Orders;

public sealed class OrderDisputesConfiguration : IEntityTypeConfiguration<OrderDispute>
{
    public void Configure(EntityTypeBuilder<OrderDispute> builder)
    {
        builder.ToTable("order_disputes");

        builder.HasKey(x => x.Id);
        builder.Property<uint>("xmin").IsRowVersion();

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.OrderId).IsRequired();
        builder.Property(x => x.BuyerId).IsRequired();
        builder.Property(x => x.SellerId).IsRequired();

        builder.Property(x => x.Reason)
            .HasConversion<string>()
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(x => x.OpenedAtUtc).IsRequired();
        builder.Property(x => x.ResolvedAtUtc);
        builder.Property(x => x.ResolvedByAdminId);

        builder.Property(x => x.Decision)
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.Property(x => x.DecisionReason)
            .HasMaxLength(2000);

        builder.HasMany(x => x.Evidence)
            .WithOne()
            .HasForeignKey(x => x.DisputeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Evidence)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => x.OrderId)
            .HasFilter("\"Status\" = 'Open'")
            .IsUnique();
        builder.HasIndex(x => new { x.Status, x.OpenedAtUtc });
    }
}
