using System.ComponentModel.DataAnnotations;
using Relora.Identity.Application.Models;

using MediatR;

namespace Relora.Identity.Application.Commands;

/// <summary>
/// Represents the register command record.
/// </summary>
public record RegisterCommand
(
    [param: Required]
    [param: MinLength(3)]
    [param: MaxLength(50)]
    string username,

    [param: Required]
    [param: EmailAddress]
    [param: MaxLength(256)]
    string email,

    [param: Required]
    [param: MinLength(8)]
    [param: MaxLength(128)]
    string password,

    [param: Required]
    [param: MinLength(8)]
    [param: MaxLength(128)]
    string confirmPassword
) : IRequest<AuthResult>;
