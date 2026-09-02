using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relora.Orders.Infrastructure.Options;

/// <summary>
/// Represents the stripe options class.
/// </summary>
public sealed class StripeOptions
{
    public const string SectionName = "Stripe";
    /// <summary>
    /// Gets or sets the secret key used by this type.
    /// </summary>
    public string SecretKey { get; init; } = default!;
    /// <summary>
    /// Gets or sets the webhook secret used by this type.
    /// </summary>
    public string WebhookSecret { get; init; } = default!;
    /// <summary>
    /// Gets or sets the success url used by this type.
    /// </summary>
    public string SuccessUrl { get; init; } = default!;
    /// <summary>
    /// Gets or sets the cancel url used by this type.
    /// </summary>
    public string CancelUrl { get; init; } = default!;
    /// <summary>
    /// Gets or sets the publishable key used by this type.
    /// </summary>
    public string PublishableKey { get; set; } = default!;
}
