using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PaymentPayout = Relora.Payments.Domain.Payout;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Relora.Persistance.Configurations.Payout;

internal sealed class PayoutConfigurations : IEntityTypeConfiguration<PaymentPayout>
{
    public void Configure(EntityTypeBuilder<PaymentPayout> builder)
    {
        builder.ToTable("Payouts");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount).IsRequired();
        builder.Property(x => x.Currency).IsRequired();
        builder.Property(x => x.StripeTransferId);
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.LastError).HasMaxLength(1000);

        builder.HasIndex(x => x.OrderId).IsUnique();
        builder.HasIndex(x => x.Status);
    }
}
