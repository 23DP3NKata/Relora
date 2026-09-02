using Relora.Items.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Relora.Persistance.Configurations.Items;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(category => category.Id);
        builder.Property(category => category.Id).ValueGeneratedNever();
        builder.Property(category => category.ParentId);
        builder.Property(category => category.NameKey).HasMaxLength(160).IsRequired();
        builder.Property(category => category.Slug).HasMaxLength(120).IsRequired();
        builder.Property(category => category.SortOrder).IsRequired();
        builder.Property(category => category.IsActive).IsRequired();
        builder.Property(category => category.MeasurementProfile).HasMaxLength(80).IsRequired();
        builder.Property(category => category.CreatedAt).IsRequired();
        builder.Property(category => category.UpdatedAt);

        builder.HasIndex(category => category.Slug).IsUnique();
        builder.HasIndex(category => category.ParentId);

        builder.OwnsMany(category => category.AllowedDepartments, department =>
        {
            department.ToTable("category_departments");
            department.WithOwner().HasForeignKey(item => item.CategoryId);
            department.HasKey(item => new { item.CategoryId, item.Department });
            department.Property(item => item.Department).HasConversion<string>().HasMaxLength(20);
        });
    }
}

public sealed class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder.ToTable("materials");
        builder.HasKey(material => material.Id);
        builder.Property(material => material.Id).ValueGeneratedNever();
        builder.Property(material => material.Code).HasMaxLength(80).IsRequired();
        builder.Property(material => material.NameKey).HasMaxLength(160).IsRequired();
        builder.Property(material => material.SortOrder).IsRequired();
        builder.Property(material => material.IsActive).IsRequired();
        builder.HasIndex(material => material.Code).IsUnique();
    }
}

public sealed class ItemColorConfiguration : IEntityTypeConfiguration<ItemColor>
{
    public void Configure(EntityTypeBuilder<ItemColor> builder)
    {
        builder.ToTable("colors");
        builder.HasKey(color => color.Id);
        builder.Property(color => color.Id).ValueGeneratedNever();
        builder.Property(color => color.Code).HasMaxLength(80).IsRequired();
        builder.Property(color => color.NameKey).HasMaxLength(160).IsRequired();
        builder.Property(color => color.SortOrder).IsRequired();
        builder.Property(color => color.IsActive).IsRequired();
        builder.HasIndex(color => color.Code).IsUnique();
    }
}

public sealed class MeasurementDefinitionConfiguration : IEntityTypeConfiguration<MeasurementDefinition>
{
    public void Configure(EntityTypeBuilder<MeasurementDefinition> builder)
    {
        builder.ToTable("measurement_definitions");
        builder.HasKey(definition => definition.Id);
        builder.Property(definition => definition.Id).ValueGeneratedNever();
        builder.Property(definition => definition.Profile).HasMaxLength(80).IsRequired();
        builder.Property(definition => definition.Key).HasMaxLength(80).IsRequired();
        builder.Property(definition => definition.LabelKey).HasMaxLength(160).IsRequired();
        builder.Property(definition => definition.IsRequired).IsRequired();
        builder.Property(definition => definition.SortOrder).IsRequired();
        builder.Property(definition => definition.Unit).HasMaxLength(12).IsRequired();
        builder.Property(definition => definition.IsActive).IsRequired();
        builder.HasIndex(definition => new { definition.Profile, definition.Key }).IsUnique();
    }
}

public sealed class ProofDocumentTypeConfiguration : IEntityTypeConfiguration<ProofDocumentType>
{
    public void Configure(EntityTypeBuilder<ProofDocumentType> builder)
    {
        builder.ToTable("proof_document_types");
        builder.HasKey(type => type.Id);
        builder.Property(type => type.Id).ValueGeneratedNever();
        builder.Property(type => type.Code).HasMaxLength(80).IsRequired();
        builder.Property(type => type.NameKey).HasMaxLength(160).IsRequired();
        builder.Property(type => type.SortOrder).IsRequired();
        builder.Property(type => type.IsActive).IsRequired();
        builder.HasIndex(type => type.Code).IsUnique();
    }
}

public sealed class PendingLotProofDocumentUploadConfiguration
    : IEntityTypeConfiguration<PendingLotProofDocumentUpload>
{
    public void Configure(EntityTypeBuilder<PendingLotProofDocumentUpload> builder)
    {
        builder.ToTable("pending_lot_proof_document_uploads");
        builder.HasKey(upload => upload.Id);
        builder.Property<uint>("xmin").IsRowVersion();
        builder.Property(upload => upload.OwnerId).IsRequired();
        builder.Property(upload => upload.DocumentTypeId).IsRequired();
        builder.Property(upload => upload.OriginalFileName).HasMaxLength(240).IsRequired();
        builder.Property(upload => upload.StorageKey).HasMaxLength(500).IsRequired();
        builder.Property(upload => upload.MimeType).HasMaxLength(120).IsRequired();
        builder.Property(upload => upload.SizeBytes).IsRequired();
        builder.Property(upload => upload.CreatedAtUtc).IsRequired();
        builder.HasIndex(upload => upload.StorageKey).IsUnique();
        builder.HasIndex(upload => upload.OwnerId);
        builder.HasIndex(upload => upload.CreatedAtUtc);
    }
}
