using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Relora.Shared.Domain.ValueObjects;
/// <summary>
/// Represents the money class.
/// </summary>
public sealed class Money : IEquatable<Money>
{
    /// <summary>
    /// Gets or sets the amount used by this type.
    /// </summary>
    public decimal Amount { get; }
    /// <summary>
    /// Gets or sets the currency used by this type.
    /// </summary>
    public string Currency { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Money"/> class.
    /// </summary>
    /// <param name="amount">Amount.</param>
    /// <param name="currency">Currency.</param>
    public Money(decimal amount, string currency)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Currency cannot be null or empty.", nameof(currency));
        }

        Amount = amount;
        Currency = currency;
    }

    /// <summary>
    /// Adds the operation.
    /// </summary>
    /// <param name="moneyToAdd">Money to add.</param>
    /// <returns>The operation result.</returns>
    public Money Add(Money moneyToAdd)
    {
        if (moneyToAdd.Currency != Currency)
        {
            throw new ArgumentException("Cannot add amounts with different currencies.", nameof(moneyToAdd));
        }
        return new Money(Amount + moneyToAdd.Amount, Currency);
    }

    /// <summary>
    /// Performs the subtract operation.
    /// </summary>
    /// <param name="moneyToSusbstract">Money to susbstract.</param>
    /// <returns>The operation result.</returns>
    public Money Subtract(Money moneyToSusbstract)
    {
        if (moneyToSusbstract.Currency != Currency)
        {
            throw new ArgumentException("Cannot subtract amounts with different currencies.", nameof(moneyToSusbstract));
        }

        if (Amount < moneyToSusbstract.Amount)
        {
            throw new ArgumentException("Resulting amount cannot be negative.", nameof(moneyToSusbstract));
        }

        return new Money(Amount - moneyToSusbstract.Amount, Currency);
    }

    /// <summary>
    /// Performs the equals operation.
    /// </summary>
    /// <param name="other">Other.</param>
    /// <returns>True if the operation succeeds; otherwise, false.</returns>
    public bool Equals(Money? other)
    {
        if (other is null)
        {
            return false;
        }

        return Amount == other.Amount && Currency == other.Currency;
    }

    /// <summary>
    /// Performs the greather than operation.
    /// </summary>
    /// <param name="amount">Amount.</param>
    /// <returns>True if the operation succeeds; otherwise, false.</returns>
    public bool GreatherThan(Money amount)
    {
        if (Amount <= amount.Amount)
        {
            return false;
        }

        return true;
    }
}
