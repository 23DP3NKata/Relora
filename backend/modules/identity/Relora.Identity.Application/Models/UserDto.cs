namespace Relora.Identity.Application.Models;

public sealed record UserDto (Guid userId, string username, string email);
