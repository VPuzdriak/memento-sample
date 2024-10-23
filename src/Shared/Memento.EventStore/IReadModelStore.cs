namespace Memento.EventStore;

public interface IReadModelStore
{
    Task SaveAsync<TModel>(TModel readModel, CancellationToken cancellationToken) where TModel : ReadModel;
    Task<TModel?> LoadAsync<TModel>(Guid id, CancellationToken cancellationToken) where TModel : ReadModel;
    Task<IReadOnlyList<TModel>> LoadAsync<TModel>(CancellationToken cancellationToken) where TModel : ReadModel;
}