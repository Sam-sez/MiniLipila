using FastEndpoints;
using MiniLipila.Models;
using MiniLipila.Services;

namespace MiniLipila.Endpoints;

public class GetTransactionEndpoint : EndpointWithoutRequest<CollectionResponse>
{
    private readonly ITransactionStore _store;

    public GetTransactionEndpoint(ITransactionStore store)
    {
        _store = store;
    }

    public override void Configure()
    {
        Get("/minilipila/collections/{referenceId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var referenceId = Route<string>("referenceId");
        var result = _store.Get(referenceId!);

        if (result is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(result, cancellation: ct);
    }

    public class CollectionStatusResponse
{
    public string ReferenceId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? PaymentType { get; set; }
    public string? Type { get; set; }
    public string? IpAddress { get; set; }
    public string? Identifier { get; set; }
    public string? ExternalId { get; set; }
    public string? Message { get; set; }
}
}