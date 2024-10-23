using System.Collections.Concurrent;

namespace Memento.EventStore.InMemory;

internal sealed class InMemoryReadModelStore : IReadModelStore
{
    private readonly ConcurrentDictionary<Guid, ReadModel> _readModels = [];

    public Task SaveAsync<TModel>(TModel readModel, CancellationToken cancellationToken) where TModel : ReadModel
    {
        _readModels.AddOrUpdate(readModel.Id, readModel, (_, _) => readModel);
        return Task.CompletedTask;
    }

    public Task<TModel?> LoadAsync<TModel>(Guid id, CancellationToken cancellationToken) where TModel : ReadModel =>
        Task.FromResult(_readModels.TryGetValue(id, out var readModel) ? (TModel)readModel : null);

    public Task<IReadOnlyList<TModel>> LoadAsync<TModel>(CancellationToken cancellationToken) where TModel : ReadModel =>
        Task.FromResult<IReadOnlyList<TModel>>(_readModels.Values.OfType<TModel>().ToList());
}