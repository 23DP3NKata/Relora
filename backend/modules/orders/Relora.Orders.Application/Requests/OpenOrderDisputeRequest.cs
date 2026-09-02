namespace Relora.Orders.Application.Requests;

public sealed record OpenOrderDisputeRequest(
    string Reason,
    string Description,
    IReadOnlyList<string>? EvidenceKeys);
