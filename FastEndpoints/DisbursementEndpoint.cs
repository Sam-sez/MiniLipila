using FastEndpoints;
using MiniLipila.Models;
using MiniLipila.Services;

namespace MiniLipila.Endpoints;

public class InitiateDisbursementEndpoint : Endpoint<DisbursementRequest, DisbursementResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDisbursementStore _store;

    public InitiateDisbursementEndpoint(IHttpClientFactory httpClientFactory, IDisbursementStore store)
    {
        _httpClientFactory = httpClientFactory;
        _store = store;
    }

    public override void Configure()
    {
        Post("/minilipila/disbursements/mobile-money");
        AllowAnonymous();
    }

    public override async Task HandleAsync(DisbursementRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.ReferenceId))
        {
            req.ReferenceId = Guid.NewGuid().ToString("N")[..12];
        }

        var client = _httpClientFactory.CreateClient("Lipila");

        var lipilaResponse = await client.PostAsJsonAsync(
            "/api/v1/disbursements/mobile-money", req, ct);

        var result = await lipilaResponse.Content.ReadFromJsonAsync<DisbursementResponse>(
            cancellationToken: ct);

        _store.Save(result!);

        await Send.OkAsync(result!, cancellation: ct);
    }
}