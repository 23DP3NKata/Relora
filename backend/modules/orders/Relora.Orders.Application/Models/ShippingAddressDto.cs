namespace Relora.Orders.Application.Models;

public sealed class ShippingAddressDto
{
    public string? FullName { get; init; }
    public string? CountryCode { get; init; }
    public string? Country { get; init; }
    public string? City { get; init; }
    public string? PostalCode { get; init; }
    public string? AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public string? Phone { get; init; }
}
