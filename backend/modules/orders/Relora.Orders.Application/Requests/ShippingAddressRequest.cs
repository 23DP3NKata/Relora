namespace Relora.Orders.Application.Requests;

public sealed record ShippingAddressRequest(
    string FullName,
    string CountryCode,
    string Country,
    string City,
    string PostalCode,
    string AddressLine1,
    string? AddressLine2,
    string? Phone);
