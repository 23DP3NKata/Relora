using Relora.Items.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Relora.Persistance.Configurations.Items;

/// <summary>
/// Represents the lot configuration class.
/// </summary>
public sealed class LotConfiguration : IEntityTypeConfiguration<Lot>
{
    /// <summary>
    /// Performs the configure operation.
    /// </summary>
    /// <param name="builder">Builder.</param>
    public void Configure(EntityTypeBuilder<Lot> builder)
    {
        builder.ToTable("lots");

        builder.HasKey(lot => lot.Id);

        builder.Property(l => l.SellerId)
            .IsRequired();

        builder.Property(lot => lot.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(lot => lot.Description)
            .IsRequired()
            .HasMaxLength(5000);

        builder.Property(lot => lot.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(l => l.Category)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(l => l.CategoryId)
            .IsRequired();

        builder.Property(l => l.Department)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(l => l.Gender)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(l => l.Size)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(l => l.Brand)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(l => l.Condition)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(l => l.Color)
            .HasMaxLength(50);

        builder.Property(l => l.PrimaryColorId)
            .IsRequired();

        builder.Property(l => l.Country)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(l => l.City)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(l => l.Style)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(l => l.ModelName)
            .HasMaxLength(120);

        builder.Property(l => l.AcquisitionYear);

        builder.Property(l => l.ProductionYear);

        builder.Property(l => l.IsVintage)
            .IsRequired();

        builder.Property(l => l.VintageNotes)
            .HasMaxLength(1000);

        builder.Property(l => l.ShippingPrice)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(l => l.ShippingCurrency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(l => l.ShippingOriginCountry)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(l => l.ShipsToCountries)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(l => l.ShippingHandlingDays)
            .IsRequired();

        builder.Property(l => l.ModerationRejectionReason)
            .HasMaxLength(2000);

        builder.Property(l => l.ModerationReviewedByAdminId);

        builder.Property(l => l.ModerationReviewedAtUtc);

        builder.Property(l => l.Age)
            .HasMaxLength(100)
            .IsRequired();



        builder.Property(l => l.CreatedAt)
            .IsRequired();

        builder.OwnsOne(lot => lot.Price, money =>
        {
            money.Property(m => m.Amount)
                .IsRequired()
                .HasColumnName("price_amount")
                .HasColumnType("numeric(18,2)");

            money.Property(m => m.Currency)
                .IsRequired()
                .HasColumnName("price_currency")
                .HasMaxLength(3);
        });

        builder.OwnsMany(l => l.Media, media =>
        {
            media.ToTable("lot_media");

            media.WithOwner().HasForeignKey("LotId");

            media.HasKey("Id");

            media.Property(m => m.Id)
                .ValueGeneratedNever();

            media.Property(m => m.Key)
                .HasColumnName("media_key")
                .HasMaxLength(500)
                .IsRequired();

            media.Property(m => m.Type)
                .HasColumnName("media_type")
                .HasMaxLength(50)
                .IsRequired();

            media.Property(m => m.SortOrder)
                .HasColumnName("sort_order")
                .IsRequired();

            media.Property(m => m.IsCover)
                .HasColumnName("is_cover")
                .IsRequired();

            media.HasIndex("LotId", "Key", "Type")
                .IsUnique();

            media.HasIndex("LotId", "SortOrder");
        });

        builder.OwnsMany(l => l.Materials, material =>
        {
            material.ToTable("lot_materials");
            material.WithOwner().HasForeignKey("LotId");
            material.HasKey(m => m.Id);
            material.Property(m => m.Id).ValueGeneratedNever();
            material.Property(m => m.MaterialId).IsRequired();
            material.Property(m => m.Percentage).HasColumnType("numeric(5,2)");
            material.Property(m => m.OtherName).HasMaxLength(120);
            material.HasIndex("LotId", "MaterialId").IsUnique();
        });

        builder.OwnsMany(l => l.SecondaryColors, color =>
        {
            color.ToTable("lot_secondary_colors");
            color.WithOwner().HasForeignKey("LotId");
            color.HasKey(c => c.Id);
            color.Property(c => c.Id).ValueGeneratedNever();
            color.Property(c => c.ColorId).IsRequired();
            color.HasIndex("LotId", "ColorId").IsUnique();
        });

        builder.OwnsMany(l => l.Measurements, measurement =>
        {
            measurement.ToTable("lot_measurements");
            measurement.WithOwner().HasForeignKey("LotId");
            measurement.HasKey(m => m.Id);
            measurement.Property(m => m.Id).ValueGeneratedNever();
            measurement.Property(m => m.Key).HasMaxLength(80).IsRequired();
            measurement.Property(m => m.Value).HasColumnType("numeric(8,2)").IsRequired();
            measurement.Property(m => m.Unit).HasMaxLength(12).IsRequired();
            measurement.HasIndex("LotId", "Key").IsUnique();
        });

        builder.OwnsMany(l => l.ProofDocuments, document =>
        {
            document.ToTable("lot_proof_documents");
            document.WithOwner().HasForeignKey("LotId");
            document.HasKey(d => d.Id);
            document.Property(d => d.Id).ValueGeneratedNever();
            document.Property(d => d.LotId).IsRequired();
            document.Property(d => d.OwnerId).IsRequired();
            document.Property(d => d.DocumentTypeId).IsRequired();
            document.Property(d => d.OriginalFileName).HasMaxLength(240).IsRequired();
            document.Property(d => d.StorageKey).HasMaxLength(500).IsRequired();
            document.Property(d => d.MimeType).HasMaxLength(120).IsRequired();
            document.Property(d => d.SizeBytes).IsRequired();
            document.Property(d => d.CreatedAtUtc).IsRequired();
            document.HasIndex("LotId", "StorageKey").IsUnique();
            document.HasIndex(d => d.OwnerId);
        });

        builder.HasIndex(l => l.CategoryId);
        builder.HasIndex(l => l.Department);
        builder.HasIndex(l => l.PrimaryColorId);
        builder.HasIndex(l => l.IsVintage);
        builder.HasIndex(l => l.ProductionYear);
    }
}
