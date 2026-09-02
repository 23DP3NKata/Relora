using Microsoft.EntityFrameworkCore;

using Relora.Auctions.Domain;
using Relora.Items.Domain;
using Relora.Bids.Domain;
using Relora.Identity.Domain;
using Relora.Orders.Domain;
using Relora.Payments.Domain;
using Relora.Support.Domain;

namespace Relora.Persistance;

/// <summary>
/// Represents the relora db context class.
/// </summary>
public sealed class ReloraDbContext : DbContext
{
    public DbSet<Auction> Auctions => Set<Auction>();
    public DbSet<Lot> Lots => Set<Lot>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<ItemColor> Colors => Set<ItemColor>();
    public DbSet<MeasurementDefinition> MeasurementDefinitions => Set<MeasurementDefinition>();
    public DbSet<ProofDocumentType> ProofDocumentTypes => Set<ProofDocumentType>();
    public DbSet<PendingLotMediaUpload> PendingLotMediaUploads => Set<PendingLotMediaUpload>();
    public DbSet<PendingLotProofDocumentUpload> PendingLotProofDocumentUploads => Set<PendingLotProofDocumentUpload>();
    public DbSet<Bid> Bids => Set<Bid>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDispute> OrderDisputes => Set<OrderDispute>();
    public DbSet<OrderDisputeEvidence> OrderDisputeEvidence => Set<OrderDisputeEvidence>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<SellerPaymentAccount> SellerPaymentAccounts => Set<SellerPaymentAccount>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Payout> Payouts => Set<Payout>();
    public DbSet<SupportRequest> SupportRequests => Set<SupportRequest>();

    public ReloraDbContext(DbContextOptions<ReloraDbContext> options) : base(options)
    {
    }
        
    /// <summary>
    /// Performs the on model creating operation.
    /// </summary>
    /// <param name="modelBuilder">Input data for the operation.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ReloraDbContext).Assembly
        );
    }
}
