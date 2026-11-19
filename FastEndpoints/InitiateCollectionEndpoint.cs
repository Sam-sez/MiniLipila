using FastEndpoints;
using MiniLipila.Models;
using MiniLipila.Services;

namespace MiniLipila.Endpoints;

public class InitiateCollectionEndpoint : Endpoint<CollectionRequest, CollectionResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITransactionStore _store;

    public InitiateCollectionEndpoint(IHttpClientFactory httpClientFactory, ITransactionStore store)
    {
        _httpClientFactory = httpClientFactory;
        _store = store;
    }

    public override void Configure()
    {
        Post("/minilipila/collections/mobile-money");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CollectionRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.ReferenceId))
        {
            req.ReferenceId = Guid.NewGuid().ToString("N")[..12];
        }

        var client = _httpClientFactory.CreateClient("Lipila");

        var lipilaResponse = await client.PostAsJsonAsync(
            "/api/v1/collections/mobile-money", req, ct);

        var result = await lipilaResponse.Content.ReadFromJsonAsync<CollectionResponse>(
            cancellationToken: ct);

        // NEW: save it before responding
        _store.Save(result!);

        await Send.OkAsync(result!, cancellation: ct);
    }
}