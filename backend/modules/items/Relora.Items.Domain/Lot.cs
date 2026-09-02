using Relora.Items.Domain.Enums;
using Relora.Shared.Domain.Abstractions;
using Relora.Shared.Domain.Enums;
using Relora.Shared.Domain.ValueObjects;

namespace Relora.Items.Domain;

/// <summary>
/// Represents a marketplace lot.
/// </summary>
public sealed class Lot : Entity<Guid>
{
    private readonly List<LotMedia> _media = new();
    private readonly List<LotMaterial> _materials = new();
    private readonly List<LotSecondaryColor> _secondaryColors = new();
    private readonly List<LotMeasurement> _measurements = new();
    private readonly List<LotProofDocument> _proofDocuments = new();

    private Lot()
    {
    }

    public Lot(
        Guid id,
        Guid sellerId,
        string title,
        string description,
        Money price,
        Guid categoryId,
        LotDepartment department,
        LotCategory legacyCategory,
        LotGender legacyGender,
        LotSize size,
        string brand,
        LotCondition condition,
        Guid primaryColorId,
        string? legacyColor,
        string country,
        string? city,
        string? modelName,
        int? acquisitionYear,
        int? productionYear,
        bool isVintage,
        string? vintageNotes,
        string age,
        string style,
        decimal shippingPrice,
        string shippingCurrency,
        string shippingOriginCountry,
        string shipsToCountries,
        int shippingHandlingDays,
        IEnumerable<LotMaterial>? materials,
        IEnumerable<Guid>? secondaryColorIds,
        IEnumerable<LotMeasurement>? measurements,
        DateTime utcNow,
        DateTime? createdAt = null) : base(id)
    {
        SellerId = sellerId;
        Status = LotStatus.Draft;
        CreatedAt = createdAt ?? utcNow;

        SetCoreDetails(
            title,
            description,
            price,
            categoryId,
            department,
            legacyCategory,
            legacyGender,
            size,
            brand,
            condition,
            primaryColorId,
            legacyColor,
            country,
            city,
            modelName,
            acquisitionYear,
            productionYear,
            isVintage,
            vintageNotes,
            age,
            style,
            utcNow);
        SetMaterials(materials ?? []);
        SetSecondaryColors(primaryColorId, secondaryColorIds ?? []);
        SetMeasurements(measurements ?? []);
        SetShippingTerms(
            shippingPrice,
            shippingCurrency,
            shippingOriginCountry,
            shipsToCountries,
            shippingHandlingDays);
    }

    public Lot(
        Guid id,
        Guid sellerId,
        string title,
        string description,
        Money price,
        LotCategory category,
        LotGender gender,
        LotSize size,
        string brand,
        LotCondition condition,
        string? color,
        string country,
        string? city,
        string age,
        string style,
        decimal shippingPrice,
        string shippingCurrency,
        string shippingOriginCountry,
        string shipsToCountries,
        int shippingHandlingDays,
        DateTime? createdAt = null
    ) : this(
        id,
        sellerId,
        title,
        description,
        price,
        Guid.Empty,
        MapDepartment(gender),
        category,
        gender,
        size,
        brand,
        condition,
        Guid.Empty,
        color,
        country,
        city,
        null,
        null,
        null,
        category == LotCategory.Vintage,
        category == LotCategory.Vintage ? "Legacy vintage category migration pending." : null,
        age,
        style,
        shippingPrice,
        shippingCurrency,
        shippingOriginCountry,
        shipsToCountries,
        shippingHandlingDays,
        [],
        [],
        [],
        DateTime.UtcNow,
        createdAt)
    {
    }

    public Guid SellerId { get; private set; }
    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public Money Price { get; private set; } = default!;
    public LotStatus Status { get; private set; }

    public Guid CategoryId { get; private set; }
    public LotDepartment Department { get; private set; }

    public LotCategory Category { get; private set; }
    public LotGender Gender { get; private set; }
    public LotSize Size { get; private set; }
    public string Brand { get; private set; } = default!;
    public string? ModelName { get; private set; }
    public LotCondition Condition { get; private set; }
    public Guid PrimaryColorId { get; private set; }
    public string? Color { get; private set; }
    public string Country { get; private set; } = default!;
    public string? City { get; private set; }
    public string Age { get; private set; } = default!;
    public string Style { get; private set; } = default!;
    public int? AcquisitionYear { get; private set; }
    public int? ProductionYear { get; private set; }
    public bool IsVintage { get; private set; }
    public string? VintageNotes { get; private set; }
    public decimal ShippingPrice { get; private set; }
    public string ShippingCurrency { get; private set; } = default!;
    public string ShippingOriginCountry { get; private set; } = default!;
    public string ShipsToCountries { get; private set; } = default!;
    public int ShippingHandlingDays { get; private set; }
    public string? ModerationRejectionReason { get; private set; }
    public Guid? ModerationReviewedByAdminId { get; private set; }
    public DateTime? ModerationReviewedAtUtc { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public List<LotMedia> Media => _media;
    public List<LotMaterial> Materials => _materials;
    public List<LotSecondaryColor> SecondaryColors => _secondaryColors;
    public List<LotMeasurement> Measurements => _measurements;
    public List<LotProofDocument> ProofDocuments => _proofDocuments;

    public void Edit(
        Guid sellerId,
        string title,
        string description,
        Money price,
        Guid categoryId,
        LotDepartment department,
        LotCategory legacyCategory,
        LotGender legacyGender,
        LotSize size,
        string brand,
        LotCondition condition,
        Guid primaryColorId,
        string? color,
        string? country,
        string? city,
        string? modelName,
        int? acquisitionYear,
        int? productionYear,
        bool isVintage,
        string? vintageNotes,
        string age,
        string style,
        decimal shippingPrice,
        string shippingCurrency,
        string shippingOriginCountry,
        string shipsToCountries,
        int shippingHandlingDays,
        IEnumerable<LotMaterial> materials,
        IEnumerable<Guid> secondaryColorIds,
        IEnumerable<LotMeasurement> measurements,
        DateTime utcNow)
    {
        EnsureSellerCanEdit(sellerId);
        EnsureEditable();

        SetCoreDetails(
            title,
            description,
            price,
            categoryId,
            department,
            legacyCategory,
            legacyGender,
            size,
            brand,
            condition,
            primaryColorId,
            color,
            country,
            city,
            modelName,
            acquisitionYear,
            productionYear,
            isVintage,
            vintageNotes,
            age,
            style,
            utcNow);
        SetMaterials(materials);
        SetSecondaryColors(primaryColorId, secondaryColorIds);
        SetMeasurements(measurements);
        SetShippingTerms(
            shippingPrice,
            shippingCurrency,
            shippingOriginCountry,
            shipsToCountries,
            shippingHandlingDays);

        if (Status == LotStatus.Rejected)
        {
            ModerationRejectionReason = null;
            ModerationReviewedByAdminId = null;
            ModerationReviewedAtUtc = null;
            Status = LotStatus.Draft;
        }
    }

    public void Edit(
        Guid sellerId,
        string title,
        string description,
        Money price,
        LotCategory category,
        LotGender gender,
        LotSize size,
        string brand,
        LotCondition condition,
        string? color,
        string? country,
        string? city,
        string age,
        string style,
        decimal shippingPrice,
        string shippingCurrency,
        string shippingOriginCountry,
        string shipsToCountries,
        int shippingHandlingDays)
    {
        Edit(
            sellerId,
            title,
            description,
            price,
            CategoryId,
            MapDepartment(gender),
            category,
            gender,
            size,
            brand,
            condition,
            PrimaryColorId,
            color,
            country,
            city,
            ModelName,
            AcquisitionYear,
            ProductionYear,
            IsVintage,
            VintageNotes,
            age,
            style,
            shippingPrice,
            shippingCurrency,
            shippingOriginCountry,
            shipsToCountries,
            shippingHandlingDays,
            _materials,
            _secondaryColors.Select(item => item.ColorId),
            _measurements,
            DateTime.UtcNow);
    }

    public static bool IsVintageEligible(int productionYear, DateTime utcNow)
    {
        return utcNow.Year - productionYear >= 15;
    }

    public void AddPhoto(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Media key is required.");
        }

        var normalizedKey = key.Trim();

        if (_media.Any(m => m.Key == normalizedKey && m.Type == "photo"))
        {
            return;
        }

        var photoCount = _media.Count(m => m.Type == "photo");
        _media.Add(new LotMedia(normalizedKey, "photo", photoCount, photoCount == 0));
    }

    public void ReplacePhotos(Guid sellerId, IEnumerable<LotPhotoState> photos)
    {
        EnsureSellerCanEdit(sellerId);
        EnsureEditable();

        var normalizedPhotos = photos
            .Select((photo, index) => new LotPhotoState(
                photo.Key?.Trim() ?? string.Empty,
                index,
                photo.IsCover))
            .ToList();

        if (normalizedPhotos.Count < 5)
        {
            throw new ArgumentException("At least 5 photos are required.");
        }

        if (normalizedPhotos.Count > 15)
        {
            throw new ArgumentException("Maximum 15 photos are allowed.");
        }

        if (normalizedPhotos.Any(photo => string.IsNullOrWhiteSpace(photo.Key)) ||
            normalizedPhotos.Select(photo => photo.Key).Distinct(StringComparer.Ordinal).Count() != normalizedPhotos.Count)
        {
            throw new ArgumentException("Photo keys must be unique and non-empty.");
        }

        if (normalizedPhotos.Count(photo => photo.IsCover) != 1)
        {
            throw new ArgumentException("Exactly one cover photo is required.");
        }

        var existingPhotosByKey = _media
            .Where(media => media.Type == "photo")
            .ToDictionary(media => media.Key, StringComparer.Ordinal);

        var updatedPhotos = normalizedPhotos
            .Select(photo =>
            {
                if (existingPhotosByKey.TryGetValue(photo.Key, out var existing))
                {
                    existing.UpdatePresentation(photo.SortOrder, photo.IsCover);
                    return existing;
                }

                return new LotMedia(photo.Key, "photo", photo.SortOrder, photo.IsCover);
            })
            .ToList();

        _media.RemoveAll(media => media.Type == "photo");
        _media.AddRange(updatedPhotos);
    }

    public void DeletePhoto(LotMedia lotToDelete)
    {
        if (lotToDelete is null)
        {
            throw new ArgumentNullException(nameof(lotToDelete));
        }

        if (string.IsNullOrWhiteSpace(lotToDelete.Key))
        {
            throw new ArgumentException("Media key is required.");
        }

        if (_media.Any(m => m.Id == lotToDelete.Id && m.Type == "photo"))
        {
            _media.Remove(lotToDelete);
        }
    }

    public void AddProofDocument(
        Guid id,
        Guid ownerId,
        Guid documentTypeId,
        string originalFileName,
        string storageKey,
        string mimeType,
        long sizeBytes,
        DateTime createdAtUtc)
    {
        EnsureSellerCanEdit(ownerId);
        EnsureEditable();

        if (_proofDocuments.Count >= 5)
        {
            throw new InvalidOperationException("Maximum 5 proof documents are allowed.");
        }

        if (_proofDocuments.Any(document => document.StorageKey == storageKey.Trim()))
        {
            return;
        }

        _proofDocuments.Add(new LotProofDocument(
            id,
            Id,
            ownerId,
            documentTypeId,
            originalFileName,
            storageKey,
            mimeType,
            sizeBytes,
            createdAtUtc));
    }

    public LotProofDocument RemoveProofDocument(Guid ownerId, Guid documentId)
    {
        EnsureSellerCanEdit(ownerId);
        EnsureEditable();

        var document = _proofDocuments.FirstOrDefault(item => item.Id == documentId);
        if (document is null)
        {
            throw new KeyNotFoundException("Proof document not found.");
        }

        _proofDocuments.Remove(document);
        return document;
    }

    public void Publish()
    {
        if (Status != LotStatus.Pending)
        {
            throw new InvalidOperationException("Only pending lots can be published.");
        }

        Status = LotStatus.Published;
    }

    public void Submit()
    {
        if (Status != LotStatus.Draft && Status != LotStatus.Rejected)
        {
            throw new InvalidOperationException("Only draft or rejected lots can be submitted.");
        }

        ModerationRejectionReason = null;
        ModerationReviewedByAdminId = null;
        ModerationReviewedAtUtc = null;
        Status = LotStatus.Pending;
    }

    public void Sold()
    {
        if (Status != LotStatus.Listed)
        {
            throw new InvalidOperationException("Only listed lots can be sold.");
        }

        Status = LotStatus.Sold;
    }

    public void MarkUnsold()
    {
        if (Status != LotStatus.Listed && Status != LotStatus.Sold)
        {
            throw new InvalidOperationException("Only listed or sold lots can be marked as unsold.");
        }

        Status = LotStatus.Unsold;
    }

    public void List()
    {
        if (Status != LotStatus.Published && Status != LotStatus.Unsold)
        {
            throw new InvalidOperationException("Only published or unsold lots can be listed.");
        }

        Status = LotStatus.Listed;
    }

    public void Reject(Guid adminId, string reason, DateTime reviewedAtUtc)
    {
        if (Status != LotStatus.Pending)
        {
            throw new InvalidOperationException("Only pending lots can be rejected.");
        }

        if (adminId == Guid.Empty)
        {
            throw new ArgumentException("Admin id is required.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Rejection reason is required.");
        }

        var normalizedReason = reason.Trim();

        if (normalizedReason.Length > 2000)
        {
            throw new ArgumentException("Rejection reason must be 2000 characters or fewer.");
        }

        ModerationRejectionReason = normalizedReason;
        ModerationReviewedByAdminId = adminId;
        ModerationReviewedAtUtc = reviewedAtUtc;
        Status = LotStatus.Rejected;
    }

    public void Accept(Guid adminId, DateTime reviewedAtUtc)
    {
        if (Status != LotStatus.Pending)
        {
            throw new InvalidOperationException("Only pending lots can be accepted.");
        }

        if (adminId == Guid.Empty)
        {
            throw new ArgumentException("Admin id is required.");
        }

        ModerationRejectionReason = null;
        ModerationReviewedByAdminId = adminId;
        ModerationReviewedAtUtc = reviewedAtUtc;
        Status = LotStatus.Published;
    }

    public bool CanShowProofMetadata(Guid? viewerUserId, bool viewerIsAdmin)
    {
        return viewerIsAdmin || (viewerUserId.HasValue && viewerUserId.Value == SellerId);
    }

    private void SetCoreDetails(
        string title,
        string description,
        Money price,
        Guid categoryId,
        LotDepartment department,
        LotCategory legacyCategory,
        LotGender legacyGender,
        LotSize size,
        string brand,
        LotCondition condition,
        Guid primaryColorId,
        string? color,
        string? country,
        string? city,
        string? modelName,
        int? acquisitionYear,
        int? productionYear,
        bool isVintage,
        string? vintageNotes,
        string age,
        string style,
        DateTime utcNow)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Trim().Length < 5 || title.Trim().Length > 200)
        {
            throw new ArgumentException("Title must be between 5 and 200 characters.");
        }

        if (string.IsNullOrWhiteSpace(description) || description.Trim().Length < 100 || description.Trim().Length > 2000)
        {
            throw new ArgumentException("Description must be between 100 and 2000 characters.");
        }

        if (price is null || price.Amount <= 0)
        {
            throw new ArgumentException("Price must be greater than zero.");
        }

        if (!Enum.IsDefined(typeof(LotDepartment), department))
        {
            throw new ArgumentException("Department is required.");
        }

        if (!Enum.IsDefined(typeof(LotSize), size))
        {
            throw new ArgumentException("Size is required.");
        }

        if (string.IsNullOrWhiteSpace(brand))
        {
            throw new ArgumentException("Brand is required.");
        }

        if (!Enum.IsDefined(typeof(LotCondition), condition))
        {
            throw new ArgumentException("Condition is required.");
        }

        ValidateYears(acquisitionYear, productionYear, isVintage, vintageNotes, utcNow);

        Title = title.Trim();
        Description = description.Trim();
        Price = price;
        CategoryId = categoryId;
        Department = department;
        Category = legacyCategory;
        Gender = legacyGender;
        Size = size;
        Brand = brand.Trim();
        ModelName = string.IsNullOrWhiteSpace(modelName) ? null : modelName.Trim();
        Condition = condition;
        PrimaryColorId = primaryColorId;
        Color = string.IsNullOrWhiteSpace(color) ? null : color.Trim();
        if (string.IsNullOrWhiteSpace(country))
        {
            throw new ArgumentException("Country is required.");
        }

        Country = country.Trim();
        City = string.IsNullOrWhiteSpace(city) ? null : city.Trim();
        AcquisitionYear = acquisitionYear;
        ProductionYear = productionYear;
        IsVintage = isVintage;
        VintageNotes = string.IsNullOrWhiteSpace(vintageNotes) ? null : vintageNotes.Trim();
        Age = string.IsNullOrWhiteSpace(age) ? string.Empty : age.Trim();
        Style = string.IsNullOrWhiteSpace(style) ? string.Empty : style.Trim();
    }

    private void SetMaterials(IEnumerable<LotMaterial> materials)
    {
        var candidates = materials
            .Where(material => material.MaterialId != Guid.Empty)
            .ToList();

        if (candidates.Count > 3)
        {
            throw new ArgumentException("You can select up to 3 materials.");
        }

        if (candidates.Select(material => material.MaterialId).Distinct().Count() != candidates.Count)
        {
            throw new ArgumentException("Material ids must be unique.");
        }

        var normalizedMaterials = candidates;

        if (normalizedMaterials.Any(material =>
                material.Percentage.HasValue &&
                (material.Percentage.Value < 0 || material.Percentage.Value > 100)))
        {
            throw new ArgumentException("Material percentages must be between 0 and 100.");
        }

        if (normalizedMaterials.Count > 0 && normalizedMaterials.All(material => material.Percentage.HasValue))
        {
            var total = normalizedMaterials.Sum(material => material.Percentage!.Value);
            if (total != 100)
            {
                throw new ArgumentException("Material percentages must add up to 100.");
            }
        }

        _materials.Clear();
        _materials.AddRange(normalizedMaterials);
    }

    private void SetSecondaryColors(Guid primaryColorId, IEnumerable<Guid> secondaryColorIds)
    {
        var normalizedColorIds = secondaryColorIds
            .Where(colorId => colorId != Guid.Empty && colorId != primaryColorId)
            .Distinct()
            .Take(8)
            .Select(colorId => new LotSecondaryColor(colorId))
            .ToList();

        _secondaryColors.Clear();
        _secondaryColors.AddRange(normalizedColorIds);
    }

    private void SetMeasurements(IEnumerable<LotMeasurement> measurements)
    {
        var normalizedMeasurements = measurements
            .Where(measurement => !string.IsNullOrWhiteSpace(measurement.Key))
            .GroupBy(measurement => measurement.Key.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToList();

        if (normalizedMeasurements.Any(measurement => measurement.Value <= 0))
        {
            throw new ArgumentException("Measurement values must be positive.");
        }

        _measurements.Clear();
        _measurements.AddRange(normalizedMeasurements);
    }

    private void SetShippingTerms(
        decimal shippingPrice,
        string shippingCurrency,
        string shippingOriginCountry,
        string shipsToCountries,
        int shippingHandlingDays)
    {
        if (shippingPrice < 0)
        {
            throw new ArgumentException("Shipping price cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(shippingCurrency))
        {
            throw new ArgumentException("Shipping currency is required.");
        }

        if (string.IsNullOrWhiteSpace(shippingOriginCountry))
        {
            throw new ArgumentException("Shipping origin country is required.");
        }

        if (string.IsNullOrWhiteSpace(shipsToCountries))
        {
            throw new ArgumentException("At least one shipping destination is required.");
        }

        if (shippingHandlingDays < 1 || shippingHandlingDays > 30)
        {
            throw new ArgumentException("Shipping handling time must be between 1 and 30 business days.");
        }

        ShippingPrice = shippingPrice;
        ShippingCurrency = shippingCurrency.Trim().ToUpperInvariant();
        ShippingOriginCountry = shippingOriginCountry.Trim();
        ShipsToCountries = shipsToCountries.Trim();
        ShippingHandlingDays = shippingHandlingDays;
    }

    private static void ValidateYears(
        int? acquisitionYear,
        int? productionYear,
        bool isVintage,
        string? vintageNotes,
        DateTime utcNow)
    {
        var currentYear = utcNow.Year;

        if (productionYear.HasValue && (productionYear.Value < 1800 || productionYear.Value > currentYear))
        {
            throw new ArgumentException("Production year is not valid.");
        }

        if (acquisitionYear.HasValue && (acquisitionYear.Value < 1900 || acquisitionYear.Value > currentYear))
        {
            throw new ArgumentException("Acquisition year is not valid.");
        }

        if (acquisitionYear.HasValue && productionYear.HasValue && acquisitionYear.Value < productionYear.Value)
        {
            throw new ArgumentException("Acquisition year cannot be earlier than production year.");
        }

        if (!isVintage)
        {
            return;
        }

        if (productionYear.HasValue && !IsVintageEligible(productionYear.Value, utcNow))
        {
            throw new ArgumentException("Items must be at least 15 years old to be marked as vintage.");
        }

        if (!productionYear.HasValue && string.IsNullOrWhiteSpace(vintageNotes))
        {
            throw new ArgumentException("Vintage notes are required when production year is unknown.");
        }
    }

    private void EnsureSellerCanEdit(Guid sellerId)
    {
        if (sellerId != SellerId)
        {
            throw new InvalidOperationException("Only the seller can edit this lot.");
        }
    }

    private void EnsureEditable()
    {
        if (Status != LotStatus.Draft && Status != LotStatus.Rejected && Status != LotStatus.Pending)
        {
            throw new InvalidOperationException("Only draft, pending, or rejected lots can be edited.");
        }
    }

    private static LotDepartment MapDepartment(LotGender gender)
    {
        return gender switch
        {
            LotGender.Women => LotDepartment.Women,
            LotGender.Men => LotDepartment.Men,
            LotGender.Unisex => LotDepartment.Unisex,
            _ => LotDepartment.Unisex
        };
    }

    private static LotGender MapLegacyGender(LotDepartment department)
    {
        return department switch
        {
            LotDepartment.Women => LotGender.Women,
            LotDepartment.Men => LotGender.Men,
            LotDepartment.Unisex => LotGender.Unisex,
            _ => LotGender.Unisex
        };
    }

    public static LotGender ToLegacyGender(LotDepartment department)
    {
        return MapLegacyGender(department);
    }

    public sealed record LotPhotoState(string Key, int SortOrder, bool IsCover);

    public sealed class LotMedia
    {
        private LotMedia()
        {
        }

        public LotMedia(string key, string type, int sortOrder = 0, bool isCover = false)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Media key is required.");
            }

            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ArgumentException("Media type is required.");
            }

            Id = Guid.NewGuid();
            Key = key.Trim();
            Type = type.Trim();
            UpdatePresentation(sortOrder, isCover);
        }

        public Guid Id { get; private set; }
        public string Key { get; private set; } = default!;
        public string Type { get; private set; } = default!;
        public int SortOrder { get; private set; }
        public bool IsCover { get; private set; }

        public void UpdatePresentation(int sortOrder, bool isCover)
        {
            if (sortOrder < 0)
            {
                throw new ArgumentException("Photo sort order cannot be negative.");
            }

            SortOrder = sortOrder;
            IsCover = isCover;
        }
    }

    public sealed class LotMaterial
    {
        private LotMaterial()
        {
        }

        public LotMaterial(Guid materialId, decimal? percentage, string? otherName)
        {
            if (materialId == Guid.Empty)
            {
                throw new ArgumentException("Material id is required.");
            }

            MaterialId = materialId;
            Percentage = percentage;
            OtherName = string.IsNullOrWhiteSpace(otherName) ? null : otherName.Trim();
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid MaterialId { get; private set; }
        public decimal? Percentage { get; private set; }
        public string? OtherName { get; private set; }
    }

    public sealed class LotSecondaryColor
    {
        private LotSecondaryColor()
        {
        }

        public LotSecondaryColor(Guid colorId)
        {
            if (colorId == Guid.Empty)
            {
                throw new ArgumentException("Color id is required.");
            }

            ColorId = colorId;
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid ColorId { get; private set; }
    }

    public sealed class LotMeasurement
    {
        private LotMeasurement()
        {
        }

        public LotMeasurement(string key, decimal value, string? unit)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Measurement key is required.");
            }

            if (value <= 0)
            {
                throw new ArgumentException("Measurement value must be positive.");
            }

            Key = key.Trim();
            Value = value;
            Unit = string.IsNullOrWhiteSpace(unit) ? "cm" : unit.Trim().ToLowerInvariant();
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Key { get; private set; } = default!;
        public decimal Value { get; private set; }
        public string Unit { get; private set; } = "cm";
    }

    public sealed class LotProofDocument
    {
        private LotProofDocument()
        {
        }

        public LotProofDocument(
            Guid id,
            Guid lotId,
            Guid ownerId,
            Guid documentTypeId,
            string originalFileName,
            string storageKey,
            string mimeType,
            long sizeBytes,
            DateTime createdAtUtc)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Proof document id is required.");
            }

            if (lotId == Guid.Empty)
            {
                throw new ArgumentException("Lot id is required.");
            }

            if (ownerId == Guid.Empty)
            {
                throw new ArgumentException("Owner id is required.");
            }

            if (documentTypeId == Guid.Empty)
            {
                throw new ArgumentException("Document type is required.");
            }

            if (string.IsNullOrWhiteSpace(storageKey))
            {
                throw new ArgumentException("Storage key is required.");
            }

            Id = id;
            LotId = lotId;
            OwnerId = ownerId;
            DocumentTypeId = documentTypeId;
            OriginalFileName = Path.GetFileName(originalFileName ?? string.Empty);
            StorageKey = storageKey.Trim();
            MimeType = string.IsNullOrWhiteSpace(mimeType)
                ? "application/octet-stream"
                : mimeType.Trim().ToLowerInvariant();
            SizeBytes = sizeBytes;
            CreatedAtUtc = createdAtUtc;
        }

        public Guid Id { get; private set; }
        public Guid LotId { get; private set; }
        public Guid OwnerId { get; private set; }
        public Guid DocumentTypeId { get; private set; }
        public string OriginalFileName { get; private set; } = default!;
        public string StorageKey { get; private set; } = default!;
        public string MimeType { get; private set; } = default!;
        public long SizeBytes { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
    }
}
