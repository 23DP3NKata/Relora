using System.ComponentModel.DataAnnotations;

using Relora.Identity.Application.Models;

using MediatR;

namespace Relora.Identity.Application.Commands;

/// <summary>
/// Represents the login command record.
/// </summary>
public record LoginCommand(
    [param: Required]
    [param: EmailAddress]
    [param: MaxLength(256)]
    string email,

    [param: Required]
    [param: MinLength(8)]
    [param: MaxLength(128)]
    string password
) : IRequest<AuthResult>;
