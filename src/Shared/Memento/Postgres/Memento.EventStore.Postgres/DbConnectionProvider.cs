using Npgsql;

namespace Memento.EventStore.Postgres;

internal sealed class DbConnectionProvider(string connectionString)
{
    public async Task<NpgsqlConnection> GetConnectionAsync()
    {
        var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        return connection;
    }
}