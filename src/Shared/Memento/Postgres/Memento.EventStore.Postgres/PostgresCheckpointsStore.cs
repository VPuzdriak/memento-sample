using Dapper;

namespace Memento.EventStore.Postgres;

internal sealed class PostgresCheckpointsStore(DbConnectionProvider dbConnectionProvider) : ICheckpointsStore
{
    public async Task SaveCheckpointAsync(ProjectionSpecs projectionSpecs, long position)
    {
        await using var dbConnection = await dbConnectionProvider.GetConnectionAsync();
        await dbConnection.ExecuteAsync(
            "insert into checkpoints (name, position) values (@Name, @Position) on conflict (name) do update set position = @Position",
            new { Name = projectionSpecs.Name, Position = position });
    }

    public async Task<long> GetCheckpointAsync(ProjectionSpecs projectionSpecs)
    {
        await using var dbConnection = await dbConnectionProvider.GetConnectionAsync();
        return await dbConnection.QuerySingleOrDefaultAsync<long?>(
            "select position from checkpoints where name = @Name",
            new { Name = projectionSpecs.Name }) ?? 0;
    }

    public async Task SaveCheckpointAsync(StreamingSpecs streamingSpecs, long position)
    {
        await using var dbConnection = await dbConnectionProvider.GetConnectionAsync();
        await dbConnection.ExecuteAsync(
            "insert into checkpoints (name, position) values (@Name, @Position) on conflict (name) do update set position = @Position",
            new { Name = streamingSpecs.Name, Position = position });
    }

    public async Task<long> GetCheckpointAsync(StreamingSpecs streamingSpecs)
    {
        await using var dbConnection = await dbConnectionProvider.GetConnectionAsync();
        return await dbConnection.QuerySingleOrDefaultAsync<long?>(
            "select position from checkpoints where name = @Name",
            new { Name = streamingSpecs.Name }) ?? 0;
    }
}