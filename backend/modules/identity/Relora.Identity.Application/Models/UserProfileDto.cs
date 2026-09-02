namespace Relora.Identity.Application.Models;

public sealed class UserProfileDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string Name { get; set; } = default!;

    public UserProfileStatsDto Stats { get; set; } = new();

    public List<UserProfileListingDto> ActiveListings { get; set; } = [];
    public List<UserProfileListingDto> SoldListings { get; set; } = [];
}
