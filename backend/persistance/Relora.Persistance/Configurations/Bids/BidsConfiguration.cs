using Relora.Bids.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Relora.Persistance.Configurations.Bids;

/// <summary>
/// Represents the bids configuration class.
/// </summary>
public sealed class BidsConfiguration : IEntityTypeConfiguration<Bid>
{
    /// <summary>
    /// Performs the configure operation.
    /// </summary>
    /// <param name="builder">Builder.</param>
    public void Configure(EntityTypeBuilder<Bid> builder)
    {
        builder.ToTable("bids");

        builder.HasKey(bid => bid.Id);

        builder.Property(bid => bid.AuctionId)
            .IsRequired();

        builder.Property(bid => bid.BidderId)
            .IsRequired();

        builder.Property(bid => bid.PlacedAt)
            .IsRequired();

        builder.Property(bid => bid.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.OwnsOne(bid => bid.Amount, money =>
        {
            money.Property(m => m.Amount)
                .IsRequired()
                .HasColumnName("amount_value")
                .HasColumnType("numeric(18,2)");

            money.Property(m => m.Currency)
                .IsRequired()
                .HasColumnName("amount_currency")
                .HasMaxLength(3);
        });
    }
}
