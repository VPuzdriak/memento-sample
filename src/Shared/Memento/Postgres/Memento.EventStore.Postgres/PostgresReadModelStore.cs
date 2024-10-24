using Dapper;

using Memento.EventStore.Postgres.Json;

using Newtonsoft.Json;

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Memento.EventStore.Postgres;

internal sealed class PostgresReadModelStore(DbConnectionProvider dbConnectionProvider, ProjectionRegistry projectionRegistry) : IReadModelStore
{
    public async Task SaveAsync<TModel>(TModel readModel, CancellationToken cancellationToken) where TModel : ReadModel
    {
        var projectionSpecs = GetProjectionSpecs<TModel>();

        await using var dbConnection = await dbConnectionProvider.GetConnectionAsync();
        await dbConnection.ExecuteAsync(
            $"insert into {projectionSpecs.Name} (id, body, body_type_name) values (@Id, @Body::jsonb, @BodyTypeName) on conflict (id) do update set body = @Body::jsonb, body_type_name = @BodyTypeName",
            new
            {
                Id = readModel.Id, Body = JsonSerializer.Serialize(readModel, readModel.GetType()), BodyTypeName = $"{readModel.GetType().FullName}, {readModel.GetType().Assembly.GetName().Name}"
            });
    }

    public async Task<TModel?> LoadAsync<TModel>(Guid id, CancellationToken cancellationToken) where TModel : ReadModel
    {
        var projectionSpecs = GetProjectionSpecs<TModel>();

        await using var dbConnection = await dbConnectionProvider.GetConnectionAsync();
        var postgresProjection = await dbConnection.QuerySingleOrDefaultAsync<PostgresProjection>(
            $"select id, body, body_type_name from {projectionSpecs.Name} where id = @Id",
            new { Id = id });

        if (postgresProjection is null)
        {
            return null;
        }

        var projectionType = Type.GetType(postgresProjection.BodyTypeName)!;
        return (TModel)JsonConvert.DeserializeObject(postgresProjection.Body, projectionType,
            new JsonSerializerSettings { ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor, ContractResolver = new PrivateResolver() })!;
    }

    public async Task<IReadOnlyList<TModel>> LoadAsync<TModel>(CancellationToken cancellationToken) where TModel : ReadModel
    {
        var projectionSpecs = GetProjectionSpecs<TModel>();

        await using var dbConnection = await dbConnectionProvider.GetConnectionAsync();
        var postgresProjections = await dbConnection.QueryAsync<PostgresProjection>(
            $"select id, body, body_type_name from {projectionSpecs.Name}");

        var list = postgresProjections.ToList();

        if (list.Count == 0)
        {
            return [];
        }

        var projectionType = Type.GetType(list[0].BodyTypeName)!;
        return list.Select(postgresProjection => (TModel)JsonConvert.DeserializeObject(postgresProjection.Body, projectionType,
            new JsonSerializerSettings { ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor, ContractResolver = new PrivateResolver() })!).ToList();
    }

    private ProjectionSpecs GetProjectionSpecs<T>()
    {
        var projectionSpecs = projectionRegistry.GetProjectionSpecs<T>();

        if (projectionSpecs is null)
        {
            throw new InvalidOperationException($"Projection specs for the read model '{typeof(T).FullName}' are not registered.");
        }

        return projectionSpecs;
    }
}