namespace Relora.Orders.Application.Requests;

public sealed record MarkOrderShippedRequest(string CarrierName, string TrackingNumber);
