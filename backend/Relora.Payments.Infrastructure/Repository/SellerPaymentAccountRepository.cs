using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Relora.Payments.Application.Interfaces;
using Relora.Payments.Domain;
using Relora.Persistance;

using Microsoft.EntityFrameworkCore;

namespace Relora.Payments.Infrastructure.Repository;

public class SellerPaymentAccountRepository(ReloraDbContext reloraDbContext) : ISellerPaymentAccountRepository
{
    private readonly ReloraDbContext _reloraDbContext = reloraDbContext;

    public Task AddSellerPaymentAccountAsync(SellerPaymentAccount sellerPaymentAccount, CancellationToken cancellationToken)
    {
        _reloraDbContext.SellerPaymentAccounts.Add(sellerPaymentAccount);
        return _reloraDbContext.SaveChangesAsync(cancellationToken);
    }

    public Task DeleteSellerPaymentAccountAsync(Guid sellerPaymentAccountId, CancellationToken cancellationToken)
    {
        _reloraDbContext.SellerPaymentAccounts.Remove(new SellerPaymentAccount { Id = sellerPaymentAccountId });
        return _reloraDbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsForSellerAsync(Guid sellerId, CancellationToken cancellationToken)
    {
       return _reloraDbContext.SellerPaymentAccounts.AnyAsync(s => s.SellerId == sellerId, cancellationToken);
    }

    public async Task<SellerPaymentAccount?> GetSellerPaymentAccountByIdAsync(Guid sellerPaymentAccountId, CancellationToken cancellationToken)
    {
        return await _reloraDbContext.SellerPaymentAccounts
            .FirstOrDefaultAsync(s => s.Id == sellerPaymentAccountId, cancellationToken);
    }

    public Task<SellerPaymentAccount?> GetBySellerIdAsync(Guid sellerId, CancellationToken cancellationToken)
    {
        return _reloraDbContext.SellerPaymentAccounts
            .FirstOrDefaultAsync(s => s.SellerId == sellerId, cancellationToken);
    }

    public Task UpdateSellerPaymentAccountAsync(SellerPaymentAccount sellerPaymentAccount, CancellationToken cancellationToken)
    {
        _reloraDbContext.Update(sellerPaymentAccount);
        return _reloraDbContext.SaveChangesAsync(cancellationToken);
    }
}
