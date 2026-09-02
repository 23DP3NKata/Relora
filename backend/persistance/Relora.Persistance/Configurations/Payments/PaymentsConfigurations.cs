using Relora.Payments.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Relora.Persistence.Configurations.Payments;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderId).IsRequired();
        builder.Property(x => x.BuyerId).IsRequired();

        builder.Property(x => x.Amount)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(x => x.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(x => x.StripeSessionId)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.StripePaymentIntentId)
            .HasMaxLength(255);

        builder.Property(x => x.StripeRefundId)
            .HasMaxLength(255);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.PaidAt);
        builder.Property(x => x.RefundedAt);

        builder.HasIndex(x => x.StripeSessionId).IsUnique();
        builder.HasIndex(x => x.OrderId);
    }
}
