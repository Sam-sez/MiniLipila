using System.Collections.Concurrent;
using MiniLipila.Models;

namespace MiniLipila.Services;

public interface ITransactionStore
{
    void Save(CollectionResponse response);
    CollectionResponse? Get(string referenceId);
    void Update(string referenceId, Action<CollectionResponse> updateAction);
}

public class InMemoryTransactionStore : ITransactionStore
{
    private readonly ConcurrentDictionary<string, CollectionResponse> _transactions = new();

    public void Save(CollectionResponse response)
    {
        _transactions[response.ReferenceId] = response;
    }

    public CollectionResponse? Get(string referenceId)
    {
        _transactions.TryGetValue(referenceId, out var result);
        return result;
    }

    public void Update(string referenceId, Action<CollectionResponse> updateAction)
    {
        if (_transactions.TryGetValue(referenceId, out var existing))
        {
            updateAction(existing);
        }
    }
}