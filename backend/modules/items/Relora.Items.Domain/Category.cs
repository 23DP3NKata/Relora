using Relora.Items.Domain.Enums;
using Relora.Shared.Domain.Abstractions;

namespace Relora.Items.Domain;

public sealed class Category : Entity<Guid>
{
    private readonly List<CategoryDepartment> _allowedDepartments = new();

    private Category()
    {
    }

    public Category(
        Guid id,
        Guid? parentId,
        string nameKey,
        string slug,
        int sortOrder,
        string measurementProfile,
        bool isActive,
        DateTime createdAt,
        DateTime? updatedAt = null) : base(id)
    {
        if (string.IsNullOrWhiteSpace(nameKey))
        {
            throw new ArgumentException("Category name key is required.");
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException("Category slug is required.");
        }

        ParentId = parentId;
        NameKey = nameKey.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        SortOrder = sortOrder;
        MeasurementProfile = string.IsNullOrWhiteSpace(measurementProfile)
            ? "default"
            : measurementProfile.Trim();
        IsActive = isActive;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Guid? ParentId { get; private set; }
    public string NameKey { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
    public string MeasurementProfile { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public IReadOnlyCollection<CategoryDepartment> AllowedDepartments => _allowedDepartments.AsReadOnly();

    public bool AllowsDepartment(LotDepartment department)
    {
        return _allowedDepartments.Count == 0 ||
               _allowedDepartments.Any(item => item.Department == department);
    }

    public void AllowDepartment(LotDepartment department)
    {
        if (_allowedDepartments.Any(item => item.Department == department))
        {
            return;
        }

        _allowedDepartments.Add(new CategoryDepartment(Id, department));
    }
}

public sealed class CategoryDepartment
{
    private CategoryDepartment()
    {
    }

    public CategoryDepartment(Guid categoryId, LotDepartment department)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException("Category id is required.");
        }

        CategoryId = categoryId;
        Department = department;
    }

    public Guid CategoryId { get; private set; }
    public LotDepartment Department { get; private set; }
}
