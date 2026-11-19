using System.Collections.Concurrent;
using MiniLipila.Models;

namespace MiniLipila.Services;

public interface IDisbursementStore
{
    void Save(DisbursementResponse response);
    DisbursementResponse? Get(string referenceId);
    void Update(string referenceId, Action<DisbursementResponse> updateAction);
}

public class InMemoryDisbursementStore : IDisbursementStore
{
    private readonly ConcurrentDictionary<string, DisbursementResponse> _transactions = new();

    public void Save(DisbursementResponse response)
    {
        _transactions[response.ReferenceId] = response;
    }

    public DisbursementResponse? Get(string referenceId)
    {
        _transactions.TryGetValue(referenceId, out var result);
        return result;
    }

    public void Update(string referenceId, Action<DisbursementResponse> updateAction)
    {
        if (_transactions.TryGetValue(referenceId, out var existing))
        {
            updateAction(existing);
        }
    }
}