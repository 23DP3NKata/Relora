using Relora.Orders.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Relora.Persistance.Configurations.Orders;

/// <summary>
/// Represents the orders configuration class.
/// </summary>
public sealed class OrdersConfiguration : IEntityTypeConfiguration<Order>
{
    /// <summary>
    /// Performs the configure operation.
    /// </summary>
    /// <param name="builder">Builder.</param>
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(x => x.Id);
        builder.Property<uint>("xmin").IsRowVersion();

        builder.Property(x => x.AuctionId).IsRequired();
        builder.Property(x => x.SellerId).IsRequired();
        builder.Property(x => x.BuyerId).IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.Price)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(x => x.ShippingPrice)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(x => x.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(x => x.ShippingCurrency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(x => x.ShippingOriginCountry)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ShipsToCountries)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.ShippingHandlingDays)
            .IsRequired();

        builder.Property(x => x.PaymentDeadlineUtc).IsRequired();
        builder.Property(x => x.StripeCheckoutSessionId).HasMaxLength(200);
        builder.Property(x => x.CheckoutSessionVersion).IsRequired();
        builder.Property(x => x.PaidAtUtc);
        builder.Property(x => x.ShipByUtc);
        builder.Property(x => x.ShippedAtUtc);
        builder.Property(x => x.DeliveredAtUtc);
        builder.Property(x => x.CompletedAtUtc);
        builder.Property(x => x.DeliveryIssueAvailableFromUtc);
        builder.Property(x => x.DeliveryIssueReportedAtUtc);
        builder.Property(x => x.DeliveryIssueReason).HasMaxLength(1000);
        builder.Property(x => x.CarrierName).HasMaxLength(120);
        builder.Property(x => x.TrackingNumber).HasMaxLength(120);
        builder.Property(x => x.ShippingFullName).HasMaxLength(160);
        builder.Property(x => x.ShippingCountryCode).HasMaxLength(2);
        builder.Property(x => x.ShippingCountry).HasMaxLength(100);
        builder.Property(x => x.ShippingCity).HasMaxLength(100);
        builder.Property(x => x.ShippingPostalCode).HasMaxLength(30);
        builder.Property(x => x.ShippingAddressLine1).HasMaxLength(240);
        builder.Property(x => x.ShippingAddressLine2).HasMaxLength(240);
        builder.Property(x => x.ShippingPhone).HasMaxLength(40);

        builder.HasIndex(x => x.AuctionId).IsUnique();
        builder.HasIndex(x => new { x.Status, x.PaymentDeadlineUtc });
        builder.HasIndex(x => new { x.Status, x.ShipByUtc });
        builder.HasIndex(x => new { x.Status, x.DeliveryIssueReportedAtUtc });
        builder.HasIndex(x => x.BuyerId);
        builder.HasIndex(x => x.SellerId);
    }
}
