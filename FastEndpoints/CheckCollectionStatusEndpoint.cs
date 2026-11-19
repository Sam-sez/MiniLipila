using FastEndpoints;
using MiniLipila.Models;
using MiniLipila.Services;
using static MiniLipila.Endpoints.GetTransactionEndpoint;

namespace MiniLipila.Endpoints;

public class CheckCollectionStatusEndpoint : EndpointWithoutRequest<CollectionStatusResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITransactionStore _store;

    public CheckCollectionStatusEndpoint(IHttpClientFactory httpClientFactory, ITransactionStore store)
    {
        _httpClientFactory = httpClientFactory;
        _store = store;
    }

    public override void Configure()
    {
        Get("/minilipila/collections/{referenceId}/status");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var referenceId = Route<string>("referenceId");

        var client = _httpClientFactory.CreateClient("Lipila");

        HttpResponseMessage lipilaResponse;
        try
        {
            lipilaResponse = await client.GetAsync(
                $"/api/v1/collections/check-status?referenceId={referenceId}", ct);
        }
        catch (HttpRequestException)
        {
            // Lipila unreachable entirely (network down, DNS failure, etc.)
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        // Graceful handling of Lipila's documented error codes
        if (!lipilaResponse.IsSuccessStatusCode)
        {
            if (lipilaResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                await Send.NotFoundAsync(ct);
                return;
            }

            // 400, 401, 403, 429, 500 - something went wrong on Lipila's side
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var statusResult = await lipilaResponse.Content.ReadFromJsonAsync<CollectionStatusResponse>(
            cancellationToken: ct);

        if (statusResult is null)
        {
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        // THE DECISION STEP: react to what Lipila told us
        switch (statusResult.Status)
        {
            case "Successful":
                _store.Update(referenceId!, t =>
                {
                    t.Status = "Successful";
                    t.Message = statusResult.Message;
                });
                break;

            case "Failed":
                _store.Update(referenceId!, t =>
                {
                    t.Status = "Failed";
                    t.Message = statusResult.Message; // e.g. "User didn't enter the pin."
                });
                break;

            case "Pending":
                // still pending - nothing to update, this is expected, not an error
                break;
        }

        await Send.OkAsync(statusResult, cancellation: ct);
    }
}