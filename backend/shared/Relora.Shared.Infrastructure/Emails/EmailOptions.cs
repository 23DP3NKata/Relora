using System.ComponentModel.DataAnnotations;

namespace Relora.Shared.Infrastructure.Emails;

public sealed class EmailOptions
{
    public const string SectionName = "Email";

    [Required]
    public string Host { get; init; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; init; }

    [Required, EmailAddress]
    public string FromEmail { get; init; } = string.Empty;

    public string FromName { get; init; } = "Relora";

    [Required]
    public string Username { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;

    public bool EnableSsl { get; init; } = true;
}
