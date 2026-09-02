using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Relora.Payments.Domain;

namespace Relora.Payments.Application.Interfaces;

public interface ISellerPaymentAccountRepository
{
    Task AddSellerPaymentAccountAsync(SellerPaymentAccount sellerPaymentAccount, CancellationToken cancellationToken);
    Task<SellerPaymentAccount?> GetSellerPaymentAccountByIdAsync(Guid sellerPaymentAccountId, CancellationToken cancellationToken);
    Task UpdateSellerPaymentAccountAsync(SellerPaymentAccount sellerPaymentAccount, CancellationToken cancellationToken);
    Task DeleteSellerPaymentAccountAsync(Guid sellerPaymentAccountId, CancellationToken cancellationToken);
    Task<bool> ExistsForSellerAsync(Guid sellerId, CancellationToken cancellationToken);
    Task<SellerPaymentAccount?> GetBySellerIdAsync(Guid sellerId, CancellationToken cancellationToken);
}
