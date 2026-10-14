using System.Data;
using Microsoft.Data.SqlClient;

namespace Sync.Common.Data
{
    /// <summary>
    /// Cliente mínimo para ejecutar procedimientos almacenados.
    /// Todas las operaciones son asíncronas y propagan las excepciones al llamador
    /// (nunca se "tragan" errores de base de datos).
    /// </summary>
    public sealed class SqlClient
    {
        private readonly string _connectionString;

        public SqlClient(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("La cadena de conexión no está configurada (ConnectionStrings:DefaultConnection).");
            _connectionString = connectionString;
        }

        /// <summary>Ejecuta un SP y devuelve su primer conjunto de resultados.</summary>
        public async Task<DataTable> QueryAsync(string procedureName, SqlParameter[]? parameters = null, CancellationToken ct = default)
        {
            await using var connection = new SqlConnection(_connectionString);
            await using var command = CreateCommand(procedureName, connection, parameters);
            await connection.OpenAsync(ct);
            await using var reader = await command.ExecuteReaderAsync(ct);
            var table = new DataTable();
            table.Load(reader);
            return table;
        }

        /// <summary>Ejecuta un SP sin resultados relevantes. Devuelve las filas afectadas.</summary>
        public async Task<int> ExecuteAsync(string procedureName, SqlParameter[]? parameters = null, CancellationToken ct = default)
        {
            await using var connection = new SqlConnection(_connectionString);
            await using var command = CreateCommand(procedureName, connection, parameters);
            await connection.OpenAsync(ct);
            return await command.ExecuteNonQueryAsync(ct);
        }

        /// <summary>Verifica conectividad (usado por /health).</summary>
        public async Task<bool> PingAsync(CancellationToken ct = default)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(ct);
            await using var command = new SqlCommand("SELECT 1", connection);
            return (int)(await command.ExecuteScalarAsync(ct) ?? 0) == 1;
        }

        private static SqlCommand CreateCommand(string procedureName, SqlConnection connection, SqlParameter[]? parameters)
        {
            var command = new SqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 30
            };
            if (parameters != null)
                command.Parameters.AddRange(parameters);
            return command;
        }
    }
}
