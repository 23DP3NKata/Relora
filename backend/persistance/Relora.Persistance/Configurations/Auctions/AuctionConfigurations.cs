using Relora.Auctions.Domain;
using Relora.Bids.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Relora.Persistance.Configurations.Auctions;

/// <summary>
/// Represents the auction configuration class.
/// </summary>
public sealed class AuctionConfiguration : IEntityTypeConfiguration<Auction>
{
    /// <summary>
    /// Performs the configure operation.
    /// </summary>
    /// <param name="builder">Builder.</param>
    public void Configure(EntityTypeBuilder<Auction> builder)
    {
        builder.ToTable("auctions");

        builder.HasKey(a => a.Id);
        builder.Property<uint>("xmin").IsRowVersion();

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(a => a.StartDate);
        builder.Property(a => a.EndDate);

        builder.Property(a => a.LotId)
            .IsRequired();

        builder.HasIndex(a => a.LotId)
            .IsUnique();

        builder.OwnsOne(a => a.CurrentPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("current_price_amount")
                .HasColumnType("numeric(18,2)");

            money.Property(m => m.Currency)
                .HasColumnName("current_price_currency")
                .HasMaxLength(3);
        });

        builder.HasMany(a => a.Bids)
            .WithOne()
            .HasForeignKey(b => b.AuctionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(a => a.Bids)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne<Relora.Items.Domain.Lot>()
            .WithOne()
            .HasForeignKey<Auction>(a => a.LotId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
