using Dapper;

using Microsoft.Extensions.DependencyInjection;

namespace Memento.EventStore.Postgres;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgresEventStore(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<DbConnectionProvider>(_ => new DbConnectionProvider(connectionString));
        services.AddSingleton<ProjectionRegistry>();
        
        services.AddSingleton<IEventStore, PostgresEventStore>();
        services.AddSingleton<ISnapshotStore, PostgresSnapshotStore>();
        services.AddSingleton<ICheckpointsStore, PostgresCheckpointsStore>();

        DefaultTypeMap.MatchNamesWithUnderscores = true;

        MapSqlTypes();

        return services;
    }

    private static void MapSqlTypes()
    {
    }
}