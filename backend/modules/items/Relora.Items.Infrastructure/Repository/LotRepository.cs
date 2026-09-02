using Relora.Items.Application.Interfaces;
using Relora.Items.Domain;
using Relora.Persistance;

using Microsoft.EntityFrameworkCore;

namespace Relora.Items.Infrastructure.Repository;
/// <summary>
/// Represents the lot repository class.
/// </summary>
public class LotRepository : ILotRepository
{
    private readonly ReloraDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="LotRepository"/> class.
    /// </summary>
    /// <param name="context">Context.</param>
    public LotRepository(ReloraDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets lot by id.
    /// </summary>
    /// <param name="id">Entity identifier.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    ///

    /// добавить отедльные запросы для получения данных для отображения на странице и редакирования чтобы ускроить загрузку данных (сеньор тайп движения)  
    public async Task<Lot?> GetLotById(Guid? id, CancellationToken cancellationToken)
    {
        return await _context.Lots
            .Include(l => l.Media)
            .Include(l => l.Materials)
            .Include(l => l.SecondaryColors)
            .Include(l => l.Measurements)
            .Include(l => l.ProofDocuments)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Lot>> GetLotsBySellerIdAsync(Guid sellerId, CancellationToken cancellationToken)
    {
        return await _context.Lots
            .AsNoTracking()
            .Include(l => l.Media)
            .Include(l => l.Materials)
            .Include(l => l.SecondaryColors)
            .Include(l => l.Measurements)
            .Include(l => l.ProofDocuments)
            .Where(l => l.SellerId == sellerId)
            .OrderByDescending(l => l.Id)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Performs the save lot async operation.
    /// </summary>
    /// <param name="lot">Lot.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SaveLotAsync(Lot lot, CancellationToken cancellationToken)
    {
        _context.Lots.Update(lot);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Adds lot async.
    /// </summary>
    /// <param name="lot">Lot.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task AddLotAsync(Lot lot, CancellationToken cancellationToken)
    {
        await _context.Lots.AddAsync(lot, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes lot async.
    /// </summary>
    /// <param name="lot">Lot.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task DeleteLotAsync(Lot lot, CancellationToken cancellationToken)
    {
        _context.Remove(lot);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Gets queryable.
    /// </summary>
    /// <returns>The operation result.</returns>
    public IQueryable<Lot> GetQueryable()
    {
        return _context.Lots
            .Include(x => x.Media)
            .Include(x => x.Materials)
            .Include(x => x.SecondaryColors)
            .Include(x => x.Measurements)
            .Include(x => x.ProofDocuments)
            .AsQueryable()
            .AsNoTracking();
    }
}
