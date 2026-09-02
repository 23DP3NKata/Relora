namespace Relora.Orders.Application.Requests;

public sealed record ResolveOrderDisputeRequest(
    string Decision,
    string Reason);
