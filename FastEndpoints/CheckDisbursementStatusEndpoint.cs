using FastEndpoints;
using MiniLipila.Models;
using MiniLipila.Services;

namespace MiniLipila.Endpoints;

public class CheckDisbursementStatusEndpoint : EndpointWithoutRequest<DisbursementResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDisbursementStore _store;

    public CheckDisbursementStatusEndpoint(IHttpClientFactory httpClientFactory, IDisbursementStore store)
    {
        _httpClientFactory = httpClientFactory;
        _store = store;
    }

    public override void Configure()
    {
        Get("/minilipila/disbursements/{referenceId}/status");
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
                $"/api/v1/disbursements/check-status?referenceId={referenceId}", ct);
        }
        catch (HttpRequestException)
        {
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        if (!lipilaResponse.IsSuccessStatusCode)
        {
            if (lipilaResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                await Send.NotFoundAsync(ct);
                return;
            }
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var statusResult = await lipilaResponse.Content.ReadFromJsonAsync<DisbursementResponse>(
            cancellationToken: ct);

        if (statusResult is null)
        {
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

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
                    t.Message = statusResult.Message;
                });
                break;
        }

        await Send.OkAsync(statusResult, cancellation: ct);
    }
}