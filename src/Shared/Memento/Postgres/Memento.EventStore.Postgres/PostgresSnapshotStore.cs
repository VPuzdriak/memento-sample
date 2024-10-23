using System.Text.Json;

using Dapper;

using Memento.Aggregate;

namespace Memento.EventStore.Postgres;

internal sealed class PostgresSnapshotStore(DbConnectionProvider dbConnectionProvider, ProjectionRegistry registry) : ISnapshotStore
{
    public async Task SaveAsync<T>(T aggregate, CancellationToken cancellationToken) where T : AggregateRoot
    {
        ProjectionSpecs projectionSpecs = GetProjectionSpecs<T>();

        await using var dbConnection = await dbConnectionProvider.GetConnectionAsync();
        await dbConnection.ExecuteAsync(
            $"insert into {projectionSpecs.Name} (id, body, body_type_name) values (@Id, @Body::jsonb, @BodyTypeName) on conflict (id) do update set body = @Body::jsonb, body_type_name = @BodyTypeName",
            new
            {
                Id = aggregate.Id, Body = JsonSerializer.Serialize(aggregate, aggregate.GetType()), BodyTypeName = $"{aggregate.GetType().FullName}, {aggregate.GetType().Assembly.GetName().Name}"
            });
    }

    public async Task<T?> LoadAsync<T>(Guid streamId, CancellationToken cancellationToken) where T : AggregateRoot
    {
        ProjectionSpecs projectionSpecs = GetProjectionSpecs<T>();

        await using var dbConnection = await dbConnectionProvider.GetConnectionAsync();
        return await dbConnection.QuerySingleOrDefaultAsync<T>(
            $"select * from {projectionSpecs.Name} where id = @Id",
            new { Id = streamId });
    }

    public Task<IReadOnlyList<T>> LoadAsync<T>(CancellationToken cancellationToken) where T : AggregateRoot
    {
        throw new NotImplementedException();
    }

    private ProjectionSpecs GetProjectionSpecs<T>() where T : AggregateRoot
    {
        var projectionSpecs = registry.GetProjectionSpecs<T>();

        if (projectionSpecs is null)
        {
            throw new InvalidOperationException($"Projection specs for the aggregate '{typeof(T).FullName}' are not registered.");
        }

        return projectionSpecs;
    }
}