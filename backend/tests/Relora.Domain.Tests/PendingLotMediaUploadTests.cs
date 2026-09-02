using Relora.Items.Application.Commands;
using Relora.Items.Application.Handlers.Commands;
using Relora.Items.Application.Interfaces;
using Relora.Items.Domain;
using Relora.Items.Domain.Enums;
using Relora.Shared.Domain.Persistence;
using Relora.Shared.Domain.Time;
using Relora.Shared.Domain.ValueObjects;

using Xunit;

namespace Relora.Domain.Tests;

public sealed class PendingLotMediaUploadTests
{
    [Fact]
    public async Task CreateLot_ConsumesOnlyTheSellerPendingUploads()
    {
        var sellerId = Guid.NewGuid();
        var photoKeys = CreatePhotoKeys(sellerId);
        var uploads = photoKeys
            .Select(key => PendingLotMediaUpload.Create(sellerId, key, DateTime.UtcNow))
            .ToList();
        var lots = new InMemoryLotRepository();
        var pendingUploads = new InMemoryPendingUploadRepository(uploads);
        var handler = new CreateLotCommandHandler(
            lots,
            new InMemoryLotCatalogRepository(),
            pendingUploads,
            new InMemoryPendingProofUploadRepository(),
            new InlineTransactionRunner(),
            new FixedClock());

        var lotId = await handler.Handle(CreateCommand(sellerId, photoKeys), CancellationToken.None);

        Assert.Equal(lots.AddedLot!.Id, lotId);
        Assert.Equal(5, lots.AddedLot.Media.Count);
        Assert.Equal(photoKeys.Order(), pendingUploads.DeletedKeys.Order());
    }

    [Fact]
    public async Task CreateLot_RejectsPhotoKeysThatWereNotUploadedBySeller()
    {
        var sellerId = Guid.NewGuid();
        var photoKeys = CreatePhotoKeys(sellerId);
        var uploads = photoKeys
            .Take(4)
            .Select(key => PendingLotMediaUpload.Create(sellerId, key, DateTime.UtcNow))
            .ToList();
        var lots = new InMemoryLotRepository();
        var pendingUploads = new InMemoryPendingUploadRepository(uploads);
        var handler = new CreateLotCommandHandler(
            lots,
            new InMemoryLotCatalogRepository(),
            pendingUploads,
            new InMemoryPendingProofUploadRepository(),
            new InlineTransactionRunner(),
            new FixedClock());

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(CreateCommand(sellerId, photoKeys), CancellationToken.None));

        Assert.Null(lots.AddedLot);
        Assert.Empty(pendingUploads.DeletedKeys);
    }

    [Fact]
    public void Lot_RejectsMoreThanThreeMaterials()
    {
        var sellerId = Guid.NewGuid();
        var materials = Enumerable.Range(1, 4)
            .Select(_ => new Lot.LotMaterial(Guid.NewGuid(), null, null))
            .ToArray();

        var exception = Assert.Throws<ArgumentException>(() => CreateDomainLot(sellerId, materials));

        Assert.Equal("You can select up to 3 materials.", exception.Message);
    }

    [Fact]
    public void Lot_StoresPhotoOrderAndSingleCoverPhoto()
    {
        var sellerId = Guid.NewGuid();
        var photoKeys = CreatePhotoKeys(sellerId);
        var lot = CreateDomainLot(sellerId, []);

        lot.ReplacePhotos(
            sellerId,
            photoKeys.Select((key, index) => new Lot.LotPhotoState(key, index, index == 2)));

        Assert.Equal(photoKeys, lot.Media.OrderBy(media => media.SortOrder).Select(media => media.Key));
        Assert.Equal(photoKeys[2], lot.Media.Single(media => media.IsCover).Key);
    }

    private static Lot CreateDomainLot(Guid sellerId, IReadOnlyList<Lot.LotMaterial> materials)
    {
        return new Lot(
            Guid.NewGuid(),
            sellerId,
            "Vintage jacket",
            new string('D', 100),
            new Money(50, "EUR"),
            Guid.NewGuid(),
            LotDepartment.Unisex,
            LotCategory.Outerwear,
            LotGender.Unisex,
            LotSize.M,
            "Relora",
            LotCondition.Worn,
            Guid.NewGuid(),
            null,
            "Latvia",
            "Riga",
            null,
            null,
            1990,
            false,
            null,
            "1990s",
            "Casual",
            5,
            "EUR",
            "Latvia",
            "LV",
            3,
            materials,
            [],
            [],
            new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
    }

    private static CreateLotCommand CreateCommand(Guid sellerId, IReadOnlyList<string> photoKeys)
    {
        return new CreateLotCommand(
            sellerId,
            "Vintage jacket",
            new string('D', 100),
            50,
            "EUR",
            LotCategory.Outerwear,
            LotGender.Unisex,
            LotSize.M,
            "Relora",
            LotCondition.Worn,
            null,
            "Latvia",
            "Riga",
            null,
            null,
            null,
            null,
            null,
            null,
            false,
            null,
            "1990s",
            "Casual",
            5,
            "EUR",
            "Latvia",
            "LV",
            3,
            photoKeys,
            null,
            null,
            null,
            null);
    }

    private static IReadOnlyList<string> CreatePhotoKeys(Guid sellerId)
    {
        return Enumerable.Range(1, 5)
            .Select(index => $"lots/{sellerId:N}/{index}.jpg")
            .ToArray();
    }

    private sealed class InMemoryLotRepository : ILotRepository
    {
        public Lot? AddedLot { get; private set; }

        public Task<Lot?> GetLotById(Guid? id, CancellationToken cancellationToken) => Task.FromResult<Lot?>(null);
        public Task<IReadOnlyList<Lot>> GetLotsBySellerIdAsync(Guid sellerId, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Lot>>([]);
        public Task SaveLotAsync(Lot lot, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task AddLotAsync(Lot lot, CancellationToken cancellationToken)
        {
            AddedLot = lot;
            return Task.CompletedTask;
        }

        public Task DeleteLotAsync(Lot lot, CancellationToken cancellationToken) => Task.CompletedTask;
        public IQueryable<Lot> GetQueryable() => Enumerable.Empty<Lot>().AsQueryable();
    }

    private sealed class InMemoryPendingUploadRepository(IEnumerable<PendingLotMediaUpload> uploads) : IPendingLotMediaUploadRepository
    {
        private readonly List<PendingLotMediaUpload> _uploads = uploads.ToList();

        public List<string> DeletedKeys { get; } = [];

        public Task AddAsync(PendingLotMediaUpload upload, CancellationToken cancellationToken)
        {
            _uploads.Add(upload);
            return Task.CompletedTask;
        }

        public Task<PendingLotMediaUpload?> GetByOwnerAndKeyAsync(Guid ownerId, string key, CancellationToken cancellationToken)
        {
            return Task.FromResult(_uploads.FirstOrDefault(upload => upload.OwnerId == ownerId && upload.Key == key));
        }

        public Task<IReadOnlyList<PendingLotMediaUpload>> GetByOwnerAndKeysAsync(
            Guid ownerId,
            IReadOnlyCollection<string> keys,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<PendingLotMediaUpload> result = _uploads
                .Where(upload => upload.OwnerId == ownerId && keys.Contains(upload.Key))
                .ToList();

            return Task.FromResult(result);
        }

        public Task DeleteAsync(PendingLotMediaUpload upload, CancellationToken cancellationToken)
        {
            _uploads.Remove(upload);
            DeletedKeys.Add(upload.Key);
            return Task.CompletedTask;
        }

        public Task DeleteRangeAsync(IEnumerable<PendingLotMediaUpload> uploads, CancellationToken cancellationToken)
        {
            foreach (var upload in uploads)
            {
                _uploads.Remove(upload);
                DeletedKeys.Add(upload.Key);
            }

            return Task.CompletedTask;
        }
    }

    private sealed class InMemoryPendingProofUploadRepository : IPendingLotProofDocumentUploadRepository
    {
        public Task AddAsync(PendingLotProofDocumentUpload upload, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task<PendingLotProofDocumentUpload?> GetByOwnerAndIdAsync(
            Guid ownerId,
            Guid id,
            CancellationToken cancellationToken) => Task.FromResult<PendingLotProofDocumentUpload?>(null);

        public Task<IReadOnlyList<PendingLotProofDocumentUpload>> GetByOwnerAndIdsAsync(
            Guid ownerId,
            IReadOnlyCollection<Guid> ids,
            CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<PendingLotProofDocumentUpload>>([]);

        public Task DeleteAsync(PendingLotProofDocumentUpload upload, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task DeleteRangeAsync(IEnumerable<PendingLotProofDocumentUpload> uploads, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class InMemoryLotCatalogRepository : ILotCatalogRepository
    {
        private readonly Category _category;
        private readonly ItemColor _otherColor = new(Guid.Parse("00000000-0000-0000-0000-000000000002"), "other", "colors.other", 99, true);

        public InMemoryLotCatalogRepository()
        {
            _category = new Category(
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Guid.Parse("00000000-0000-0000-0000-000000000010"),
                "categories.otherClothing",
                "other-clothing",
                99,
                "tops",
                true,
                DateTime.UtcNow);
            _category.AllowDepartment(LotDepartment.Unisex);
        }

        public Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<Category>>([_category]);
        }

        public Task<Category?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult<Category?>(_category.Id == id ? _category : null);
        }

        public Task<Category?> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken)
        {
            return Task.FromResult<Category?>(slug == _category.Slug ? _category : null);
        }

        public Task<IReadOnlyList<Category>> GetCategoriesBySlugsAsync(
            IReadOnlyCollection<string> slugs,
            CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<Category>>(slugs.Contains(_category.Slug) ? [_category] : []);
        }

        public Task<IReadOnlyList<Material>> GetMaterialsAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Material>>([]);
        public Task<IReadOnlyList<Material>> GetMaterialsByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Material>>([]);
        public Task<Material?> GetMaterialByCodeAsync(string code, CancellationToken cancellationToken) => Task.FromResult<Material?>(null);
        public Task<IReadOnlyList<ItemColor>> GetColorsAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<ItemColor>>([_otherColor]);
        public Task<IReadOnlyList<ItemColor>> GetColorsByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<ItemColor>>([]);
        public Task<ItemColor?> GetColorByIdAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult<ItemColor?>(_otherColor.Id == id ? _otherColor : null);
        public Task<ItemColor?> GetColorByCodeAsync(string code, CancellationToken cancellationToken) => Task.FromResult<ItemColor?>(code == _otherColor.Code ? _otherColor : null);
        public Task<IReadOnlyList<MeasurementDefinition>> GetMeasurementDefinitionsAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<MeasurementDefinition>>([]);
        public Task<IReadOnlyList<MeasurementDefinition>> GetMeasurementDefinitionsByProfileAsync(string profile, CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<MeasurementDefinition>>([]);
        public Task<IReadOnlyList<ProofDocumentType>> GetProofDocumentTypesAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<ProofDocumentType>>([]);
        public Task<ProofDocumentType?> GetProofDocumentTypeByIdAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult<ProofDocumentType?>(null);
    }

    private sealed class FixedClock : IClock
    {
        public DateTime UtcNow { get; } = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }

    private sealed class InlineTransactionRunner : ITransactionRunner
    {
        public Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken)
        {
            return operation(cancellationToken);
        }
    }
}
